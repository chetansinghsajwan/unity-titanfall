using System;
using UnityEngine;
using GameFramework.Extensions;
using System.Diagnostics.Contracts;

struct CharacterCapsule
{
    public const float COLLISION_OFFSET = 0f;
    public const float RECALCULATE_NORMAL_FALLBACK = .01f;
    public const float RECALCULATE_NORMAL_ADDON = .001f;

    public bool CapsuleCast(Vector3 move, out RaycastHit hit)
    {
        bool result = CapsuleCast(move, out RaycastHit innerHit, out RaycastHit outerHit);
        hit = innerHit.collider ? innerHit : outerHit;

        return result;
    }

    public bool CapsuleCast(Vector3 move, out RaycastHit innerHit, out RaycastHit outerHit)
    {
        if (skinWidth > 0f)
        {
            VirtualCapsule outerCapsule = GetOuterCapsule();
            outerCapsule.CapsuleCast(move, out outerHit);

            if (outerHit.collider)
            {
                move = move.normalized * outerHit.distance;
            }

            capsule.CapsuleCast(move, out innerHit);
        }
        else
        {
            capsule.CapsuleCast(move, out outerHit);
            innerHit = outerHit;
        }

        return innerHit.collider || outerHit.collider;
    }

    public bool BaseSphereCast(Vector3 move, out RaycastHit hit)
    {
        bool result = BaseSphereCast(move, out RaycastHit innerHit, out RaycastHit outerHit);
        hit = innerHit.collider ? innerHit : outerHit;

        return result;
    }

    public bool BaseSphereCast(Vector3 move, out RaycastHit hit, out Vector3 hitNormal)
    {
        bool didHit = BaseSphereCast(move, out hit);

        if (hit.collider == null)
        {
            hitNormal = hit.normal;
        }
        else
        {
            RecalculateNormal(hit, move.normalized, out hitNormal);
        }

        return didHit;
    }

    public bool BaseSphereCast(Vector3 move, out RaycastHit innerHit, out RaycastHit outerHit)
    {
        if (skinWidth > 0f)
        {
            VirtualCapsule outerCapsule = GetOuterCapsule();
            outerCapsule.BaseSphereCast(move, out outerHit);

            if (outerHit.collider)
            {
                move = move.normalized * outerHit.distance;
            }

            capsule.BaseSphereCast(move, out innerHit);
        }
        else
        {
            capsule.BaseSphereCast(move, out outerHit);
            innerHit = outerHit;
        }

        return innerHit.collider || outerHit.collider;
    }

    public Vector3 CapsuleMove(Vector3 move)
    {
        return CapsuleMove(move, out RaycastHit innerHit, out RaycastHit outerHit);
    }

    public Vector3 CapsuleMove(Vector3 move, out RaycastHit hit)
    {
        Vector3 moved = CapsuleMove(move, out RaycastHit innerHit, out RaycastHit outerHit);
        hit = innerHit.collider ? innerHit : outerHit;

        return moved;
    }

    public Vector3 CapsuleMove(Vector3 move, out RaycastHit hit, out Vector3 hitNormal)
    {
        var moved = CapsuleMove(move, out RaycastHit innerHit, out RaycastHit outerHit, out hitNormal);
        hit = innerHit.collider ? innerHit : outerHit;

        return moved;
    }

    public Vector3 CapsuleMove(Vector3 move, out RaycastHit innerHit, out RaycastHit outerHit)
    {
        if (move.magnitude == 0f)
        {
            innerHit = new RaycastHit();
            outerHit = new RaycastHit();

            return Vector3.zero;
        }

        CapsuleCast(move, out innerHit, out outerHit);

        Vector3 direction = move.normalized;
        float distance = move.magnitude;

        if (innerHit.collider)
        {
            distance = innerHit.distance - skinWidth - COLLISION_OFFSET;
        }
        else if (outerHit.collider)
        {
            distance = outerHit.distance - COLLISION_OFFSET;
        }

        capsule.position += direction * distance;

        return direction * Math.Max(0f, distance);
    }

    public Vector3 CapsuleMove(Vector3 move, out RaycastHit innerHit, out RaycastHit outerHit, out Vector3 hitNormal)
    {
        Vector3 moved = CapsuleMove(move, out innerHit, out outerHit);
        RaycastHit hit = innerHit.collider != null ? innerHit : outerHit;

        if (hit.collider != null)
        {
            RecalculateNormal(hit, move.normalized, out hitNormal);
        }
        else
        {
            hitNormal = hit.normal;
        }

        return moved;
    }

    public Vector3 CapsuleResolvePenetration()
    {
        Vector3 resolve = Vector3.zero;

        if (skinWidth > 0f)
        {
            VirtualCapsule outerCapsule = GetOuterCapsule();
            resolve = outerCapsule.ResolvePenetration(collider, COLLISION_OFFSET);
        }
        else
        {
            resolve = capsule.ResolvePenetration(collider, COLLISION_OFFSET);
        }

        return resolve;
    }

    public void TeleportTo(Vector3 pos, Quaternion rot)
    {
        capsule.position = pos;
        capsule.rotation = rot;
    }

    public bool RecalculateNormal(RaycastHit hit, out Vector3 normal)
    {
        return hit.RecalculateNormalUsingRaycast(out normal,
            capsule.layerMask, capsule.triggerInteraction);
    }

    public bool RecalculateNormal(RaycastHit hit, Vector3 direction, out Vector3 normal)
    {
        Contract.Assert(hit.collider != null);

        if (direction == Vector3.zero)
        {
            normal = hit.normal;
            return false;
        }

        Vector3 origin = hit.point + (-direction * RECALCULATE_NORMAL_FALLBACK);
        const float rayDistance = RECALCULATE_NORMAL_FALLBACK + RECALCULATE_NORMAL_ADDON;

        Physics.Raycast(origin, direction, out RaycastHit rayHit,
            rayDistance, capsule.layerMask, capsule.triggerInteraction);

        if (rayHit.collider == null || rayHit.collider != hit.collider)
        {
            normal = hit.normal;
            return false;
        }

        normal = rayHit.normal;
        return true;
    }

    public bool RecalculateNormalIfZero(RaycastHit hit, ref Vector3 normal)
    {
        if (normal != Vector3.zero)
        {
            return true;
        }

        return RecalculateNormal(hit, out normal);
    }

    public VirtualCapsule GetOuterCapsule()
    {
        VirtualCapsule outerCapsule = capsule;
        outerCapsule.radius += skinWidth;
        outerCapsule.height += skinWidth + skinWidth;
        return outerCapsule;
    }

    public VirtualCapsule capsule;
    public float skinWidth;
    public CapsuleCollider collider;
}

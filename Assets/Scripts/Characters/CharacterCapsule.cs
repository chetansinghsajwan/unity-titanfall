using System;
using UnityEngine;
using GameFramework.Extensions;

struct CharacterCapsule
{
    public const float COLLISION_OFFSET = .001f;
    public const float RECALCULATE_NORMAL_FALLBACK = .01f;
    public const float RECALCULATE_NORMAL_ADDON = .001f;

    public bool CapsuleCast(Vector3 move, out RaycastHit hit)
    {
        bool result = CapsuleCast(move, out RaycastHit smallHit, out RaycastHit bigHit);
        hit = smallHit.collider ? smallHit : bigHit;

        return result;
    }

    public bool CapsuleCast(Vector3 move, out RaycastHit smallHit, out RaycastHit bigHit)
    {
        if (skinWidth > 0f)
        {
            VirtualCapsule bigCapsule = capsule;
            bigCapsule.radius += skinWidth;
            bigCapsule.CapsuleCast(move, out bigHit);

            if (bigHit.collider)
            {
                move = move.normalized * bigHit.distance;
            }

            capsule.CapsuleCast(move, out smallHit);
        }
        else
        {
            capsule.CapsuleCast(move, out bigHit);
            smallHit = bigHit;
        }

        return smallHit.collider || bigHit.collider;
    }

    public bool BaseSphereCast(Vector3 move, out RaycastHit hit)
    {
        bool result = BaseSphereCast(move, out RaycastHit smallHit, out RaycastHit bigHit);
        hit = smallHit.collider ? smallHit : bigHit;

        return result;
    }

    public bool BaseSphereCast(Vector3 move, out RaycastHit hit, out Vector3 hitNormal)
    {
        bool didHit = BaseSphereCast(move, out hit);
        RecalculateNormal(hit, move.normalized, out hitNormal);

        return didHit;
    }

    public bool BaseSphereCast(Vector3 move, out RaycastHit smallHit, out RaycastHit bigHit)
    {
        if (skinWidth > 0f)
        {
            VirtualCapsule bigCapsule = capsule;
            bigCapsule.radius += skinWidth;
            bigCapsule.BaseSphereCast(move, out bigHit);

            if (bigHit.collider)
            {
                move = move.normalized * bigHit.distance;
            }

            capsule.BaseSphereCast(move, out smallHit);
        }
        else
        {
            capsule.BaseSphereCast(move, out bigHit);
            smallHit = bigHit;
        }

        return smallHit.collider || bigHit.collider;
    }

    public Vector3 CapsuleMove(Vector3 move)
    {
        return CapsuleMove(move, out RaycastHit smallHit, out RaycastHit bigHit);
    }

    public Vector3 CapsuleMove(Vector3 move, out RaycastHit hit)
    {
        Vector3 moved = CapsuleMove(move, out RaycastHit smallHit, out RaycastHit bigHit);
        hit = smallHit.collider ? smallHit : bigHit;

        return moved;
    }

    public Vector3 CapsuleMove(Vector3 move, out RaycastHit hit, out Vector3 hitNormal)
    {
        var moved = CapsuleMove(move, out RaycastHit smallHit, out RaycastHit bigHit, out hitNormal);
        hit = smallHit.collider ? smallHit : bigHit;

        return moved;
    }

    public Vector3 CapsuleMove(Vector3 move, out RaycastHit smallHit, out RaycastHit bigHit)
    {
        if (move.magnitude == 0f)
        {
            smallHit = new RaycastHit();
            bigHit = new RaycastHit();

            // CapsuleResolvePenetration();

            return Vector3.zero;
        }

        CapsuleCast(move, out smallHit, out bigHit);

        Vector3 direction = move.normalized;
        float distance = move.magnitude;

        if (smallHit.collider)
        {
            distance = smallHit.distance - skinWidth - COLLISION_OFFSET;
        }
        else if (bigHit.collider)
        {
            distance = bigHit.distance - COLLISION_OFFSET;
        }

        capsule.position += direction * distance;

        // CapsuleResolvePenetration();

        return direction * Math.Max(0f, distance);
    }

    public Vector3 CapsuleMove(Vector3 move, out RaycastHit smallHit, out RaycastHit bigHit, out Vector3 hitNormal)
    {
        Vector3 moved = CapsuleMove(move, out smallHit, out bigHit);
        RaycastHit hit = smallHit.collider ? smallHit : bigHit;

        RecalculateNormal(hit, move.normalized, out hitNormal);

        return moved;
    }

    public Vector3 CapsuleResolvePenetration()
    {
        Vector3 resolve = Vector3.zero;

        if (skinWidth > 0f)
        {
            VirtualCapsule bigCapsule = capsule;
            bigCapsule.radius += skinWidth;

            resolve = bigCapsule.ResolvePenetration(collider, COLLISION_OFFSET);
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
        if (hit.collider && direction != Vector3.zero)
        {
            Vector3 origin = hit.point + (-direction * RECALCULATE_NORMAL_FALLBACK);
            const float rayDistance = RECALCULATE_NORMAL_FALLBACK + RECALCULATE_NORMAL_ADDON;

            Physics.Raycast(origin, direction, out RaycastHit rayHit,
                rayDistance, capsule.layerMask, capsule.triggerInteraction);

            if (rayHit.collider && rayHit.collider == hit.collider)
            {
                normal = rayHit.normal;
                return true;
            }
        }

        if (RecalculateNormal(hit, out normal))
        {
            return true;
        }

        normal = Vector3.zero;
        return false;
    }

    public bool RecalculateNormalIfZero(RaycastHit hit, ref Vector3 normal)
    {
        if (normal == Vector3.zero)
        {
            return RecalculateNormal(hit, out normal);
        }

        return true;
    }

    public VirtualCapsule capsule;
    public float skinWidth;
    public CapsuleCollider collider;
}

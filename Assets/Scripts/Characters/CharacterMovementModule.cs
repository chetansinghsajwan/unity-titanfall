using System;
using UnityEngine;
using GameFramework.Extensions;
using System.Diagnostics.Contracts;

abstract class CharacterMovementModule
{
    protected const float COLLISION_OFFSET = .001f;
    protected const float RECALCULATE_NORMAL_FALLBACK = .01f;
    protected const float RECALCULATE_NORMAL_ADDON = .001f;

    public virtual void OnLoaded(CharacterMovement charMovement)
    {
        Contract.Requires(charMovement != null);

        _charMovement = charMovement;
        _character = _charMovement.character;
    }

    public virtual void OnUnloaded()
    {
        Contract.Requires(_charMovement != null);
    }

    public virtual void Update() { }

    public abstract bool ShouldRun();

    public virtual void StartPhysics() { }

    public abstract void RunPhysics(out VirtualCapsule result);

    public virtual void StopPhysics() { }

    public virtual void PostUpdate() { }

    protected bool CapsuleCast(Vector3 move, out RaycastHit hit)
    {
        bool result = CapsuleCast(move, out RaycastHit smallHit, out RaycastHit bigHit);
        hit = smallHit.collider ? smallHit : bigHit;

        return result;
    }

    protected bool CapsuleCast(Vector3 move, out RaycastHit smallHit, out RaycastHit bigHit)
    {
        if (_skinWidth > 0f)
        {
            VirtualCapsule bigCapsule = _capsule;
            bigCapsule.radius += _skinWidth;
            bigCapsule.CapsuleCast(move, out bigHit);

            if (bigHit.collider)
            {
                move = move.normalized * bigHit.distance;
            }

            _capsule.CapsuleCast(move, out smallHit);
        }
        else
        {
            _capsule.CapsuleCast(move, out bigHit);
            smallHit = bigHit;
        }

        return smallHit.collider || bigHit.collider;
    }

    protected bool BaseSphereCast(Vector3 move, out RaycastHit hit)
    {
        bool result = BaseSphereCast(move, out RaycastHit smallHit, out RaycastHit bigHit);
        hit = smallHit.collider ? smallHit : bigHit;

        return result;
    }

    protected bool BaseSphereCast(Vector3 move, out RaycastHit hit, out Vector3 hitNormal)
    {
        bool didHit = BaseSphereCast(move, out hit);
        RecalculateNormal(hit, move.normalized, out hitNormal);

        return didHit;
    }

    protected bool BaseSphereCast(Vector3 move, out RaycastHit smallHit, out RaycastHit bigHit)
    {
        if (_skinWidth > 0f)
        {
            VirtualCapsule bigCapsule = _capsule;
            bigCapsule.radius += _skinWidth;
            bigCapsule.BaseSphereCast(move, out bigHit);

            if (bigHit.collider)
            {
                move = move.normalized * bigHit.distance;
            }

            _capsule.BaseSphereCast(move, out smallHit);
        }
        else
        {
            _capsule.BaseSphereCast(move, out bigHit);
            smallHit = bigHit;
        }

        return smallHit.collider || bigHit.collider;
    }

    protected Vector3 CapsuleMove(Vector3 move)
    {
        return CapsuleMove(move, out RaycastHit smallHit, out RaycastHit bigHit);
    }

    protected Vector3 CapsuleMove(Vector3 move, out RaycastHit hit)
    {
        Vector3 moved = CapsuleMove(move, out RaycastHit smallHit, out RaycastHit bigHit);
        hit = smallHit.collider ? smallHit : bigHit;

        return moved;
    }

    protected Vector3 CapsuleMove(Vector3 move, out RaycastHit hit, out Vector3 hitNormal)
    {
        var moved = CapsuleMove(move, out RaycastHit smallHit, out RaycastHit bigHit, out hitNormal);
        hit = smallHit.collider ? smallHit : bigHit;

        return moved;
    }

    protected Vector3 CapsuleMove(Vector3 move, out RaycastHit smallHit, out RaycastHit bigHit)
    {
        if (move.magnitude == 0f)
        {
            smallHit = new RaycastHit();
            bigHit = new RaycastHit();

            CapsuleResolvePenetration();

            return Vector3.zero;
        }

        CapsuleCast(move, out smallHit, out bigHit);

        Vector3 direction = move.normalized;
        float distance = move.magnitude;

        if (smallHit.collider)
        {
            float skinWidth = Mathf.Max(0, _skinWidth);
            distance = smallHit.distance - skinWidth - COLLISION_OFFSET;
        }
        else if (bigHit.collider)
        {
            distance = bigHit.distance - COLLISION_OFFSET;
        }

        _capsule.position += direction * distance;

        CapsuleResolvePenetration();

        return direction * Math.Max(0f, distance);
    }

    protected Vector3 CapsuleMove(Vector3 move, out RaycastHit smallHit, out RaycastHit bigHit, out Vector3 hitNormal)
    {
        Vector3 moved = CapsuleMove(move, out smallHit, out bigHit);
        RaycastHit hit = smallHit.collider ? smallHit : bigHit;

        RecalculateNormal(hit, move.normalized, out hitNormal);

        return moved;
    }

    protected Vector3 CapsuleResolvePenetration()
    {
        Vector3 resolve = Vector3.zero;

        if (_skinWidth > 0f)
        {
            VirtualCapsule bigCapsule = _capsule;
            bigCapsule.radius += _skinWidth;

            resolve = bigCapsule.ResolvePenetration(_capsuleCollider, COLLISION_OFFSET);
        }
        else
        {
            resolve = _capsule.ResolvePenetration(_capsuleCollider, COLLISION_OFFSET);
        }

        return resolve;
    }

    protected void TeleportTo(Vector3 pos, Quaternion rot)
    {
        _capsule.position = pos;
        _capsule.rotation = rot;
    }

    protected bool RecalculateNormal(RaycastHit hit, out Vector3 normal)
    {
        return hit.RecalculateNormalUsingRaycast(out normal,
            _capsule.layerMask, _capsule.queryTrigger);
    }

    protected bool RecalculateNormal(RaycastHit hit, Vector3 direction, out Vector3 normal)
    {
        if (hit.collider && direction != Vector3.zero)
        {
            Vector3 origin = hit.point + (-direction * RECALCULATE_NORMAL_FALLBACK);
            const float rayDistance = RECALCULATE_NORMAL_FALLBACK + RECALCULATE_NORMAL_ADDON;

            Physics.Raycast(origin, direction, out RaycastHit rayHit,
                rayDistance, _capsule.layerMask, _capsule.queryTrigger);

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

    protected bool RecalculateNormalIfZero(RaycastHit hit, ref Vector3 normal)
    {
        if (normal == Vector3.zero)
        {
            return RecalculateNormal(hit, out normal);
        }

        return true;
    }

    protected Character _character;
    protected CharacterMovement _charMovement;
    protected CapsuleCollider _capsuleCollider;

    protected float _skinWidth;
    protected VirtualCapsule _capsule;
}

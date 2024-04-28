using System.Diagnostics.Contracts;
using UnityEngine;

class CharacterMovementAirModule : CharacterMovementModule
{
    public const uint MAX_MOVE_ITERATIONS = 5;
    public const float GRAVITY_MULTIPLIER = .05f;

    public CharacterMovementAirModule(CharacterAsset charAsset)
    {
        Contract.Assert(charAsset != null);

        _gravityAcceleration = charAsset.airGravityAcceleration;
        _gravityMaxSpeed = charAsset.airGravityMaxSpeed;
        _minMoveDistance = charAsset.airMinMoveDistance;
        _moveSpeed = charAsset.airMoveSpeed;
        _moveAcceleration = charAsset.airMoveAcceleration;
    }

    public override void OnLoaded(CharacterMovement charMovement)
    {
        base.OnLoaded(charMovement);

        _groundModule = null;
        if (_charMovement != null)
        {
            foreach (CharacterMovementModule module in _charMovement.modules)
            {
                if (module is CharacterMovementGroundModule)
                {
                    _groundModule = module as CharacterMovementGroundModule;
                    break;
                }
            }
        }
    }

    public override bool ShouldRun()
    {
        _charUp = _character.up;
        _charRight = _character.right;
        _charForward = _character.forward;
        _velocity = _charMovement.velocity;
        _charCapsule.capsule = _charMovement.capsule;
        _charCapsule.skinWidth = _charMovement.skinWidth;
        _charCapsule.collider = _charMovement.collider;
        _deltaTime = Time.deltaTime;

        return true;
    }

    public override void RunPhysics(out VirtualCapsule result)
    {
        float gravitySpeed = _gravityAcceleration * _character.mass * _deltaTime * _deltaTime * GRAVITY_MULTIPLIER;
        Vector3 gravityVelocity = _charUp * gravitySpeed;
        Vector3 move = _velocity * _deltaTime + gravityVelocity;

        _PerformMove(move);
        result = _charCapsule.capsule;
    }

    protected void _PerformMove(Vector3 move)
    {
        Vector3 remainingMove = move;
        for (int it = 0; it < MAX_MOVE_ITERATIONS; it++)
        {
            remainingMove -= _charCapsule.CapsuleMove(remainingMove, out RaycastHit hit, out Vector3 hitNormal);
            if (hitNormal == Vector3.zero)
            {
                hitNormal = hit.normal;
            }

            if (hit.collider == null)
            {
                break;
            }

            bool canStandOnGround = _groundModule.CanStandOnGround(hit, hitNormal, out _);
            if (canStandOnGround)
            {
                break;
            }

            remainingMove = Vector3.ProjectOnPlane(remainingMove, hit.normal);
        }
    }

    protected string _debugModuleName = "Air Module";

    protected CharacterMovementGroundModule _groundModule;
    protected CharacterCapsule _charCapsule;

    protected Vector3 _charUp;
    protected Vector3 _charRight;
    protected Vector3 _charForward;
    protected Vector3 _velocity;
    protected float _deltaTime;

    protected readonly float _gravityAcceleration;
    protected readonly float _gravityMaxSpeed;
    protected readonly float _minMoveDistance;
    protected readonly float _moveSpeed;
    protected readonly float _moveAcceleration;
}
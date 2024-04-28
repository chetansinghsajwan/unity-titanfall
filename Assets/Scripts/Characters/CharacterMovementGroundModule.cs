using System;
using System.Diagnostics.Contracts;
using GameFramework.Extensions;
using UnityEngine;

[Serializable]
class CharacterMovementGroundModule : CharacterMovementModule
{
    [Serializable]
    protected struct GroundResult
    {
        public static readonly GroundResult Invalid = new GroundResult();

        // the raycast hit during check
        public RaycastHit hit;

        // the angle of ground respective to character up direction during check
        public float groundAngle;

        // the position of ground during check
        public Vector3 groundPosition;

        // the rotation of ground during check
        public Vector3 groundRotation;

        public bool canStand;
    }

    public enum MovementState : byte
    {
        StandIdle,
        StandWalk,
        StandRun,
        StandSprint,

        CrouchIdle,
        CrouchWalk,
        CrouchRun,

        ProneIdle,
        ProneWalk,
    }

    public CharacterMovementGroundModule(CharacterAsset charAsset)
    {
        Contract.Assume(charAsset is not null);

        _groundResult = GroundResult.Invalid;
        _groundCheckDepth = charAsset.groundCheckDepth;
        _groundLayer = charAsset.groundLayer;
        _minMoveDistance = charAsset.groundMinMoveDistance;

        _standDeacceleration = charAsset.groundStandIdleAcceleration;
        _standWalkSpeed = charAsset.groundStandWalkSpeed;
        _standWalkAcceleration = charAsset.groundStandWalkAcceleration;
        _standRunSpeed = charAsset.groundStandRunSpeed;
        _standRunAcceleration = charAsset.groundStandRunAcceleration;
        _standSprintSpeed = charAsset.groundStandSprintSpeed;
        _standSprintAcceleration = charAsset.groundStandSprintAcceleration;
        _standSprintLeftAngleMax = charAsset.groundStandSprintLeftAngleMax;
        _standSprintRightAngleMax = charAsset.groundStandSprintRightAngleMax;
        _standJumpForce = charAsset.groundStandJumpForce;
        _standStepUpPercent = charAsset.groundStandStepUpPercent;
        _standStepUpHeight = charAsset.groundStandCapsuleHeight * _standStepUpPercent / 100f;
        _standStepDownPercent = charAsset.groundStandStepDownPercent;
        _standStepDownDepth = charAsset.groundStandCapsuleHeight * _standStepDownPercent / 100f;
        _standSlopeUpAngle = Math.Clamp(charAsset.groundStandSlopeUpAngle, _MIN_SLOPE_ANGLE, _MAX_SLOPE_ANGLE);
        _standSlopeDownAngle = charAsset.groundStandSlopeDownAngle;
        _standMaintainVelocityOnSurface = charAsset.groundStandMaintainVelocityOnSurface;
        _standMaintainVelocityAlongSurface = charAsset.groundStandMaintainVelocityAlongSurface;
        _standCapsuleCenter = charAsset.groundStandCapsuleCenter;
        _standCapsuleHeight = charAsset.groundStandCapsuleHeight;
        _standCapsuleRadius = charAsset.groundStandCapsuleRadius;
        _standToCrouchResizeSpeed = charAsset.groundStandToCrouchResizeSpeed;

        _crouchDeacceleration = charAsset.groundCrouchIdleAcceleration;
        _crouchWalkSpeed = charAsset.groundCrouchWalkSpeed;
        _crouchWalkAcceleration = charAsset.groundCrouchWalkAcceleration;
        _crouchRunSpeed = charAsset.groundCrouchRunSpeed;
        _crouchRunAcceleration = charAsset.groundCrouchRunAcceleration;
        _crouchJumpForce = charAsset.groundCrouchJumpForce;
        _crouchStepUpPercent = charAsset.groundCrouchStepUpPercent;
        _crouchStepUpHeight = charAsset.groundCrouchCapsuleHeight * _crouchStepUpPercent / 100f;
        _crouchStepDownPercent = charAsset.groundCrouchStepDownPercent;
        _crouchStepDownDepth = charAsset.groundCrouchCapsuleHeight * _crouchStepDownPercent / 100f;
        _crouchSlopeUpAngle = Math.Clamp(charAsset.groundCrouchSlopeUpAngle, _MIN_SLOPE_ANGLE, _MAX_SLOPE_ANGLE);
        _crouchSlopeDownAngle = charAsset.groundCrouchSlopeDownAngle;
        _crouchMaintainVelocityOnSurface = charAsset.groundCrouchMaintainVelocityOnSurface;
        _crouchMaintainVelocityAlongSurface = charAsset.groundCrouchMaintainVelocityAlongSurface;
        _crouchCapsuleCenter = charAsset.groundCrouchCapsuleCenter;
        _crouchCapsuleHeight = charAsset.groundCrouchCapsuleHeight;
        _crouchCapsuleRadius = charAsset.groundCrouchCapsuleRadius;
        _crouchToStandResizeSpeed = charAsset.groundCrouchToStandResizeSpeed;
    }

    //// -------------------------------------------------------------------------------------------
    //// Commands to control ground movement of character.
    //// -------------------------------------------------------------------------------------------

    public void SetMoveVector(Vector2 move)
    {
        _moveVector = move;
    }

    public void SwitchToStandIdle()
    {
        _movementState = MovementState.StandIdle;
        _stepDownDepth = _standStepDownDepth;
        _stepUpHeight = _standStepUpHeight;
        _maxSlopeUpAngle = _standSlopeUpAngle;
        _slopeDownAngle = _standSlopeDownAngle;
        _maintainVelocityOnSurface = _standMaintainVelocityOnSurface;
        _maintainVelocityAlongSurface = _standMaintainVelocityAlongSurface;
        _moveAccel = _standDeacceleration;
        _moveSpeed = 0;
        _jumpPower = _standJumpForce;
        _targetCapsuleCenter = _standCapsuleCenter;
        _targetCapsuleHeight = _standCapsuleHeight;
        _targetCapsuleRadius = _standCapsuleRadius;
    }

    public void SwitchToStandWalk()
    {
        _movementState = MovementState.StandWalk;
        _stepDownDepth = _standStepDownDepth;
        _stepUpHeight = _standStepUpHeight;
        _maxSlopeUpAngle = _standSlopeUpAngle;
        _slopeDownAngle = _standSlopeDownAngle;
        _maintainVelocityOnSurface = _standMaintainVelocityOnSurface;
        _maintainVelocityAlongSurface = _standMaintainVelocityAlongSurface;
        _moveAccel = _standWalkAcceleration;
        _moveSpeed = _standWalkSpeed;
        _jumpPower = _standJumpForce;
        _targetCapsuleCenter = _standCapsuleCenter;
        _targetCapsuleHeight = _standCapsuleHeight;
        _targetCapsuleRadius = _standCapsuleRadius;
    }

    public void SwitchToStandRun()
    {
        _movementState = MovementState.StandRun;
        _stepDownDepth = _standStepDownDepth;
        _stepUpHeight = _standStepUpHeight;
        _maxSlopeUpAngle = _standSlopeUpAngle;
        _slopeDownAngle = _standSlopeDownAngle;
        _maintainVelocityOnSurface = _standMaintainVelocityOnSurface;
        _maintainVelocityAlongSurface = _standMaintainVelocityAlongSurface;
        _moveAccel = _standRunAcceleration;
        _moveSpeed = _standRunSpeed;
        _jumpPower = _standJumpForce;
        _targetCapsuleCenter = _standCapsuleCenter;
        _targetCapsuleHeight = _standCapsuleHeight;
        _targetCapsuleRadius = _standCapsuleRadius;
    }

    public void SwitchToStandSprint()
    {
        _movementState = MovementState.StandSprint;
        _stepDownDepth = _standStepDownDepth;
        _stepUpHeight = _standStepUpHeight;
        _maxSlopeUpAngle = _standSlopeUpAngle;
        _slopeDownAngle = _standSlopeDownAngle;
        _maintainVelocityOnSurface = _standMaintainVelocityOnSurface;
        _maintainVelocityAlongSurface = _standMaintainVelocityAlongSurface;
        _moveAccel = _standSprintAcceleration;
        _moveSpeed = _standSprintSpeed;
        _jumpPower = _standJumpForce;
        _targetCapsuleCenter = _standCapsuleCenter;
        _targetCapsuleHeight = _standCapsuleHeight;
        _targetCapsuleRadius = _standCapsuleRadius;
    }

    public void SwitchToCrouchIdle()
    {
        _movementState = MovementState.CrouchIdle;
        _stepDownDepth = _crouchStepDownDepth;
        _stepUpHeight = _crouchStepUpHeight;
        _maxSlopeUpAngle = _crouchSlopeUpAngle;
        _slopeDownAngle = _crouchSlopeDownAngle;
        _maintainVelocityOnSurface = _crouchMaintainVelocityOnSurface;
        _maintainVelocityAlongSurface = _crouchMaintainVelocityAlongSurface;
        _moveAccel = _crouchDeacceleration;
        _moveSpeed = 0;
        _jumpPower = _crouchJumpForce;
        _targetCapsuleCenter = _crouchCapsuleCenter;
        _targetCapsuleHeight = _crouchCapsuleHeight;
        _targetCapsuleRadius = _crouchCapsuleRadius;
    }

    public void SwitchToCrouchWalk()
    {
        _movementState = MovementState.CrouchWalk;
        _stepDownDepth = _crouchStepDownDepth;
        _stepUpHeight = _crouchStepUpHeight;
        _maxSlopeUpAngle = _crouchSlopeUpAngle;
        _slopeDownAngle = _crouchSlopeDownAngle;
        _maintainVelocityOnSurface = _crouchMaintainVelocityOnSurface;
        _maintainVelocityAlongSurface = _crouchMaintainVelocityAlongSurface;
        _moveAccel = _crouchWalkAcceleration;
        _moveSpeed = _crouchWalkSpeed;
        _jumpPower = _crouchJumpForce;
        _targetCapsuleCenter = _crouchCapsuleCenter;
        _targetCapsuleHeight = _crouchCapsuleHeight;
        _targetCapsuleRadius = _crouchCapsuleRadius;
    }

    public void SwitchToCrouchRun()
    {
        _movementState = MovementState.CrouchRun;
        _stepDownDepth = _crouchStepDownDepth;
        _stepUpHeight = _crouchStepUpHeight;
        _maxSlopeUpAngle = _crouchSlopeUpAngle;
        _slopeDownAngle = _crouchSlopeDownAngle;
        _maintainVelocityOnSurface = _crouchMaintainVelocityOnSurface;
        _maintainVelocityAlongSurface = _crouchMaintainVelocityAlongSurface;
        _moveAccel = _crouchRunAcceleration;
        _moveSpeed = _crouchRunSpeed;
        _jumpPower = _crouchJumpForce;
        _targetCapsuleCenter = _crouchCapsuleCenter;
        _targetCapsuleHeight = _crouchCapsuleHeight;
        _targetCapsuleRadius = _crouchCapsuleRadius;
    }

    public void Jump()
    {
        _doJump = true;
    }

    public bool CanStandOnGround(RaycastHit hit, Vector3 hitNormal, out float groundAngle)
    {
        return _CanStandOn(hit, hitNormal, out groundAngle);
    }

    //// -------------------------------------------------------------------------------------------
    //// CharacterBehaviour events
    //// -------------------------------------------------------------------------------------------

    public override void OnLoaded(CharacterMovement charMovement)
    {
        base.OnLoaded(charMovement);

        _charView = _character.charView;
    }

    public override void OnUnloaded()
    {
        base.OnUnloaded();

        _charView = null;
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

        // if we jumped before, we only run this module if we are coming down now.
        if (_didJump)
        {
            // get vertical velocity respective to character orientation.
            Vector3 upVelocity = Vector3.Project(_velocity, _charUp);
            float angle = Vector3.Angle(upVelocity, _charUp);

            // if angle is 0, we are going in the same direction as the `_charUp`.
            if (angle == 0)
            {
                return false;
            }

            _didJump = false;
        }

        // check if the ground that we are standing on, moved since the last move.
        Collider groundCollider = _groundResult.hit.collider;
        if (groundCollider != null)
        {
            _groundDeltaPosition = groundCollider.transform.position - _groundResult.groundPosition;
            _groundDeltaRotation = groundCollider.transform.rotation.eulerAngles - _groundResult.groundRotation;
        }
        else
        {
            _groundDeltaPosition = Vector3.zero;
            _groundDeltaRotation = Vector3.zero;
        }

        // if the ground moved, then we'll run this module
        if (_groundDeltaPosition != Vector3.zero || _groundDeltaRotation != Vector3.zero)
        {
            return true;
        }

        _UpdateGroundResult();
        return _groundResult.canStand;
    }

    public override void RunPhysics(out VirtualCapsule result)
    {
        _RecoverFromGroundMove();

        Vector3 moveInput = new Vector3(_moveVector.x, 0, _moveVector.y);
        moveInput = Quaternion.Euler(0, _charView.turnAngle, 0) * moveInput.normalized;
        moveInput = _character.rotation * moveInput;

        _velocity = Vector3.ProjectOnPlane(_velocity, _charUp);

        Vector3 move = moveInput * _moveSpeed * _deltaTime;
        move = Vector3.MoveTowards(_velocity * _deltaTime, move, _moveAccel * _deltaTime);

        if (_doJump)
        {
            _doJump = false;
            _didJump = true;
            move += _charUp * _jumpPower * _deltaTime;
        }

        _UpdateCapsuleSize();
        _PerformMove(move);

        result = _charCapsule.capsule;
    }

    //// -------------------------------------------------------------------------------------------
    //// Physics
    //// -------------------------------------------------------------------------------------------

    protected void _PerformMove(Vector3 move)
    {
        Vector3 moveH = Vector3.ProjectOnPlane(move, _charUp);
        Vector3 moveV = move - moveH;
        Vector3 moveNormalized = move.normalized;

        _CapsuleResolvePenetration();

        // perform the vertical move (usually jump)
        if (moveV != Vector3.zero)
        {
            _MoveCapsule(moveV, out _);
        }

        // perform the horizontal move
        Vector3 remainingMove = moveH;
        if (remainingMove.magnitude > _minMoveDistance)
        {
            var stepUpHeight = 0f;                      // the height by which we stepped up, used during step up recover
            var canStepUp = moveV == Vector3.zero;      // allow step up if there is no vertical move (possibly jump)
            var didStepUp = false;                      // did we step up in previous iterations
            var didStepUpRecover = false;               // did we recover the previous step up
            var positionBeforeStepUp = Vector3.zero;    // used to revert the step up move
            var moveBeforeStepUp = Vector3.zero;        // used to revert the step up move

            for (uint it = 0; it < _MAX_MOVE_ITERATIONS; it++)
            {
                _MoveCapsule(remainingMove, out Vector3 moved, out RaycastHit moveHit, out Vector3 moveHitNormal);
                remainingMove -= moved;

                // if we stepped up previous iteration, perform step up recover
                if (didStepUp && !didStepUpRecover)
                {
                    didStepUp = false;
                    didStepUpRecover = true;

                    _MoveCapsule(-_charUp * stepUpHeight, out _, out RaycastHit stepUpRecoverHit, out Vector3 stepUpRecoverHitNormal);

                    if (stepUpRecoverHit.collider != null)
                    {
                        // if we cannot step on this _ground, revert the step up
                        // and continue the loop without stepping up this time
                        if (_CanStandOn(stepUpRecoverHit, stepUpRecoverHitNormal, out float groundAngle) == false)
                        {
                            _charCapsule.capsule.position = positionBeforeStepUp;
                            remainingMove = moveBeforeStepUp;
                            canStepUp = false;
                            continue;
                        }
                    }
                }

                // if there is no collision, end the move
                if (moveHit.collider == null)
                {
                    break;
                }

                // try moving up on the slope
                if (_CalculateMoveOnSlope(remainingMove, out Vector3 slopeMove, moveHit, moveHitNormal))
                {
                    remainingMove = slopeMove;
                    continue;
                }

                // step up
                if (canStepUp && !didStepUp)
                {
                    canStepUp = false;
                    didStepUp = true;
                    didStepUpRecover = false;
                    positionBeforeStepUp = _charCapsule.capsule.position;
                    moveBeforeStepUp = remainingMove;

                    _MoveCapsule(_charUp * _stepUpHeight, out Vector3 stepUpMoved);
                    stepUpHeight = stepUpMoved.magnitude;
                    continue;
                }

                // treat the obstacle as a wall now
                Vector3 wallHitProject = Vector3.ProjectOnPlane(moveHitNormal, _charUp);
                remainingMove = Vector3.ProjectOnPlane(moveNormalized * remainingMove.magnitude, wallHitProject);

                // avoid sliding along perpendicular surface for very small values which 
                // could be a result of small miscalculations
                if (_maintainVelocityAlongSurface &&
                    remainingMove.magnitude > _MIN_MOVE_ALONG_SURFACE_TO_MAINTAIN_VELOCITY)
                {
                    remainingMove = remainingMove.normalized * remainingMove.magnitude;
                }
            }
        }

        // step down, but if we jumped previously, don't step down.
        if (!_didJump && _stepDownDepth > 0)
        {
            _MoveCapsule(-_charUp * _stepDownDepth, out Vector3 moved, out RaycastHit hit, out Vector3 hitNormal);
            if (hit.collider != null && !_CanStandOn(hit, hitNormal, out _))
            {
                _charCapsule.capsule.position -= moved;
            }
        }
    }

    protected bool _CalculateMoveOnSlope(Vector3 move, out Vector3 slopeMove, RaycastHit hit, Vector3 hitNormal)
    {
        Contract.Assert(hit.collider != null);

        if (!_CanStandOn(hit, hitNormal, out float slopeAngle))
        {
            slopeMove = Vector3.zero;
            return false;
        }

        // We don't just project here, because that gives undesired results for horizontal moves.
        Plane plane = new Plane(hitNormal, hit.point);
        Ray ray = new Ray(hit.point + move, _charUp);
        plane.Raycast(ray, out float up);
        slopeMove = move + (_charUp * up);
        slopeMove = slopeMove.normalized * move.magnitude;

        return true;
    }

    protected void _RecoverFromGroundMove()
    {
        if (_groundDeltaPosition != Vector3.zero)
        {
            _charCapsule.capsule.position += _groundDeltaPosition;
            _groundDeltaPosition = Vector3.zero;
        }

        // TODO: update position for ground rotation also
    }

    protected void _UpdateCapsuleSize()
    {
        bool isCrouching = _movementState == MovementState.CrouchIdle
            || _movementState == MovementState.CrouchWalk
            || _movementState == MovementState.CrouchRun;

        float resizeSpeed = isCrouching ? _standToCrouchResizeSpeed : _crouchToStandResizeSpeed;
        resizeSpeed *= _deltaTime;

        float heightBeforeResize = _charCapsule.capsule.actualHeight;
        _charCapsule.capsule.height = Mathf.MoveTowards(_charCapsule.capsule.height, _targetCapsuleHeight, resizeSpeed);
        _charCapsule.capsule.radius = Mathf.MoveTowards(_charCapsule.capsule.radius, _targetCapsuleRadius, resizeSpeed);
        float heightAfterResize = _charCapsule.capsule.actualHeight;

        float deltaHeight = heightAfterResize - heightBeforeResize;
        _charCapsule.capsule.position += _charUp * (deltaHeight / 2);
    }

    protected void _UpdateGroundResult()
    {
        _CheckForGround(_groundCheckDepth, out _groundResult);
    }

    protected bool _CheckForGround(float depth, out GroundResult result)
    {
        _charCapsule.BaseSphereCast(-_charUp * depth, out RaycastHit hit, out Vector3 hitNormal);
        if (hitNormal == Vector3.zero)
        {
            hitNormal = hit.normal;
        }

        result = new GroundResult
        {
            hit = hit
        };

        if (hit.collider == null)
        {
            return false;
        }

        bool canStand = _CanStandOn(hit, hitNormal, out float groundAngle);

        result.groundAngle = groundAngle;
        result.groundPosition = hit.collider.transform.position;
        result.groundRotation = hit.collider.transform.rotation.eulerAngles;

        return canStand;
    }

    protected bool _CheckIsGround(Collider collider)
    {
        Contract.Assert(collider != null);

        return _groundLayer.Contains(collider.gameObject.layer);
    }

    protected bool _CanStandOn(RaycastHit hit, Vector3 hitNormal, out float groundAngle)
    {
        Contract.Assert(hit.collider != null);
        Contract.Assert(hitNormal != Vector3.zero);

        if (!_CheckIsGround(hit.collider))
        {
            groundAngle = 0f;
            return false;
        }

        groundAngle = Vector3.Angle(_charUp, hitNormal);
        if (groundAngle < _MIN_SLOPE_ANGLE || groundAngle > _maxSlopeUpAngle)
        {
            return false;
        }

        return true;
    }

    protected void _MoveCapsule(Vector3 move, out Vector3 moved)
    {
        moved = _charCapsule.CapsuleMove(move);
    }

    protected void _MoveCapsule(Vector3 move, out Vector3 moved, out RaycastHit innerHit, out RaycastHit outerHit)
    {
        moved = _charCapsule.CapsuleMove(move, out innerHit, out outerHit);
    }

    protected void _MoveCapsule(Vector3 move, out Vector3 moved, out RaycastHit hit, out Vector3 normal)
    {
        moved = _charCapsule.CapsuleMove(move, out hit, out normal);
        if (normal == Vector3.zero)
        {
            normal = hit.normal;
        }
    }

    protected void _CapsuleResolvePenetration()
    {
        _charCapsule.CapsuleResolvePenetration();
    }

    protected void _RecalculateNormalIfZero(RaycastHit hit, ref Vector3 normal)
    {
        _charCapsule.RecalculateNormalIfZero(hit, ref normal);
    }

    //// -------------------------------------------------------------------------------------------
    //// Fields
    //// -------------------------------------------------------------------------------------------

    protected const string _debugModuleName = "Ground Module";

    protected const uint _MAX_MOVE_ITERATIONS = 6;
    protected const float _MIN_MOVE_ALONG_SURFACE_TO_MAINTAIN_VELOCITY = .0001f;
    protected const float _MIN_SLOPE_ANGLE = 0f;
    protected const float _MAX_SLOPE_ANGLE = 89.9f;

    protected CharacterMovementGroundModuleAsset _moduleAsset;
    protected CharacterView _charView;
    protected CharacterCapsule _charCapsule;
    protected GroundResult _groundResult;
    protected Vector3 _groundDeltaPosition;
    protected Vector3 _groundDeltaRotation;
    protected MovementState _movementState;

    protected Vector3 _moveVector = Vector3.zero;
    protected bool _doJump = false;
    protected bool _didJump = false;
    protected float _moveSpeed = 0;
    protected float _moveAccel = 0;
    protected float _jumpPower = 0;
    protected float _stepUpHeight = 0;
    protected float _stepDownDepth = 0;
    protected float _maxSlopeUpAngle = 0;
    protected float _slopeDownAngle = 0;
    protected bool _maintainVelocityOnSurface = true;
    protected bool _maintainVelocityAlongSurface = true;
    protected Vector3 _targetCapsuleCenter = Vector3.zero;
    protected float _targetCapsuleHeight = 0;
    protected float _targetCapsuleRadius = 0;

    protected Vector3 _charUp = Vector3.zero;
    protected Vector3 _charRight = Vector3.zero;
    protected Vector3 _charForward = Vector3.zero;
    protected Vector3 _velocity = Vector3.zero;
    protected float _deltaTime;

    protected readonly LayerMask _groundLayer;
    protected readonly float _minMoveDistance;
    protected readonly float _groundCheckDepth;

    protected readonly float _standDeacceleration;
    protected readonly float _standWalkSpeed;
    protected readonly float _standWalkAcceleration;
    protected readonly float _standRunSpeed;
    protected readonly float _standRunAcceleration;
    protected readonly float _standSprintSpeed;
    protected readonly float _standSprintAcceleration;
    protected readonly float _standSprintLeftAngleMax;
    protected readonly float _standSprintRightAngleMax;
    protected readonly float _standJumpForce;
    protected readonly float _standStepUpPercent;
    protected readonly float _standStepDownPercent;
    protected readonly float _standStepUpHeight;
    protected readonly float _standStepDownDepth;
    protected readonly float _standSlopeUpAngle;
    protected readonly float _standSlopeDownAngle;
    protected readonly float _standCapsuleHeight;
    protected readonly float _standCapsuleRadius;
    protected readonly float _standToCrouchResizeSpeed;
    protected readonly bool _standMaintainVelocityOnSurface;
    protected readonly bool _standMaintainVelocityAlongSurface;
    protected readonly Vector3 _standCapsuleCenter;

    protected readonly float _crouchDeacceleration;
    protected readonly float _crouchWalkSpeed;
    protected readonly float _crouchWalkAcceleration;
    protected readonly float _crouchRunSpeed;
    protected readonly float _crouchRunAcceleration;
    protected readonly float _crouchJumpForce;
    protected readonly float _crouchStepUpPercent;
    protected readonly float _crouchStepDownPercent;
    protected readonly float _crouchStepUpHeight;
    protected readonly float _crouchStepDownDepth;
    protected readonly float _crouchSlopeUpAngle;
    protected readonly float _crouchSlopeDownAngle;
    protected readonly float _crouchCapsuleHeight;
    protected readonly float _crouchCapsuleRadius;
    protected readonly float _crouchToStandResizeSpeed;
    protected readonly bool _crouchMaintainVelocityOnSurface;
    protected readonly bool _crouchMaintainVelocityAlongSurface;
    protected readonly Vector3 _crouchCapsuleCenter;
}

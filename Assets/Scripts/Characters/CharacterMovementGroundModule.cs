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

        public GameObject gameObject => collider ? collider.gameObject : null;
        public Collider collider;
        public int layer => gameObject ? gameObject.layer : 0;

        public Vector3 direction;
        public float distance;
        public float angle;
        public float edgeDistance;

        public Vector3 basePosition;
        public Quaternion baseRotation;

        public bool isValid
        {
            get => collider != null;
        }
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

        _prevGroundResult = GroundResult.Invalid;
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

    public bool CanStandOnGround(RaycastHit hit, Vector3 slopeNormal, out float slopeAngle)
    {
        return _CanStandOn(hit, slopeNormal, out slopeAngle);
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

        _UpdateGroundResult();

        _baseDeltaPosition = Vector3.zero;
        _baseDeltaRotation = Vector3.zero;

        if (_groundResult.isValid)
        {
            _baseDeltaPosition = _groundResult.collider.transform.position - _groundResult.basePosition;
            _baseDeltaRotation = _groundResult.collider.transform.rotation.eulerAngles - _groundResult.baseRotation.eulerAngles;
        }

        if (_baseDeltaPosition != Vector3.zero || _baseDeltaRotation != Vector3.zero)
        {
            return true;
        }

        return _groundResult.isValid;
    }

    public override void RunPhysics(out VirtualCapsule result)
    {
        _RecoverFromBaseMove();

        Vector3 moveInput = new Vector3(_moveVector.x, 0, _moveVector.y);
        moveInput = Quaternion.Euler(0, _charView.turnAngle, 0) * moveInput.normalized;
        moveInput = _character.rotation * moveInput;

        _velocity = Vector3.ProjectOnPlane(_velocity, _charUp);

        Vector3 move = moveInput * _moveSpeed * _deltaTime;
        move = Vector3.MoveTowards(_velocity * _deltaTime, move, _moveAccel * _deltaTime);

        if (_doJump)
        {
            _doJump = false;
            move += _charUp * _jumpPower * _deltaTime;
        }

        _UpdateCapsuleSize();
        _PerformMove(move);
        _lastMovementState = _movementState;

        result = _charCapsule.capsule;
    }

    //// -------------------------------------------------------------------------------------------
    //// Physics
    //// -------------------------------------------------------------------------------------------

    protected void _PerformMove(Vector3 originalMove)
    {
        Vector3 moveH = Vector3.ProjectOnPlane(originalMove, _charUp);
        Vector3 moveV = originalMove - moveH;
        Vector3 remainingMove = moveH;

        // perform the vertical move (usually jump)
        if (moveV != Vector3.zero)
        {
            _charCapsule.CapsuleMove(moveV);
        }

        if (remainingMove.magnitude > _minMoveDistance)
        {
            var stepUpHeight = 0f;
            var canStepUp = moveV.magnitude == 0f;
            var didStepUp = false;
            var didStepUpRecover = false;
            var positionBeforeStepUp = Vector3.zero;
            var moveBeforeStepUp = Vector3.zero;

            _charCapsule.CapsuleResolvePenetration();

            for (uint it = 0; it < _MAX_MOVE_ITERATIONS; it++)
            {
                remainingMove -= _charCapsule.CapsuleMove(remainingMove, out RaycastHit moveHit, out Vector3 moveHitNormal);

                // perform step up recover
                if (didStepUp && !didStepUpRecover)
                {
                    didStepUp = false;
                    didStepUpRecover = true;

                    _charCapsule.CapsuleMove(_character.down * stepUpHeight, out RaycastHit stepUpRecoverHit, out Vector3 stepUpRecoverHitNormal);

                    if (stepUpRecoverHit.collider)
                    {
                        // if we cannot step on this _ground, revert the step up
                        // and continue the loop without stepping up this time
                        if (_CanStandOn(stepUpRecoverHit, stepUpRecoverHitNormal, out float baseAngle) == false)
                        {
                            if (baseAngle < 90f)
                            {
                                _charCapsule.capsule.position = positionBeforeStepUp;
                                remainingMove = moveBeforeStepUp;
                                canStepUp = false;

                                continue;
                            }
                        }
                    }
                }

                // if there is no collision (no obstacle or remainingMove == 0)
                // break the loop
                if (moveHit.collider is null)
                {
                    break;
                }

                // try sliding on the obstacle
                if (_TrySlideOnSurface(originalMove, ref remainingMove, moveHit, moveHitNormal))
                {
                    continue;
                }

                // step up the first time, we hit an obstacle
                if (canStepUp && didStepUp == false)
                {
                    canStepUp = false;
                    didStepUp = true;
                    didStepUpRecover = false;
                    positionBeforeStepUp = _charCapsule.capsule.position;
                    moveBeforeStepUp = remainingMove;

                    stepUpHeight = _charCapsule.CapsuleMove(_charUp * _stepUpHeight).magnitude;

                    continue;
                }

                // try sliding along the obstacle
                if (_SlideAlongSurface(originalMove, ref remainingMove, moveHit, moveHitNormal))
                {
                    continue;
                }

                // there's nothing we can do now, so stop the move
                remainingMove = Vector3.zero;
            }
        }

        _StepDown(originalMove);
        _charCapsule.CapsuleResolvePenetration();
    }

    protected void _RecoverFromBaseMove()
    {
        if (_baseDeltaPosition != Vector3.zero)
        {
            _charCapsule.capsule.position += _baseDeltaPosition;
            _baseDeltaPosition = Vector3.zero;
        }

        // TODO: update position for base rotation also
    }

    protected void _UpdateCapsuleSize()
    {
        bool isCrouching = _movementState == MovementState.CrouchIdle
            || _movementState == MovementState.CrouchWalk
            || _movementState == MovementState.CrouchRun;

        float resizeSpeed = isCrouching ? _standToCrouchResizeSpeed : _crouchToStandResizeSpeed;
        resizeSpeed *= _deltaTime;

        // charCapsule.localPosition += charCapsule.up * Mathf.MoveTowards(charCapsule.localHeight, _targetCapsuleHeight, resizeSpeed);
        // _charCapsule.capsule.center = Vector3.Lerp(mCapsule.center, _targetCapsuleCenter, resizeSpeed);
        _charCapsule.capsule.height = Mathf.Lerp(_charCapsule.capsule.height, _targetCapsuleHeight, resizeSpeed);
        _charCapsule.capsule.radius = Mathf.Lerp(_charCapsule.capsule.radius, _targetCapsuleRadius, resizeSpeed);

        float weight = 0;
        weight = Mathf.Lerp(weight, 1f, resizeSpeed);
        // _movementStateWeight = weight;
    }

    protected bool _TrySlideOnSurface(Vector3 originalMove, ref Vector3 remainingMove, RaycastHit hit, Vector3 hitNormal)
    {
        if (remainingMove == Vector3.zero)
            return false;

        if (_CanStandOn(hit, hitNormal, out float slopeAngle) is false)
        {
            return false;
        }

            if (slopeAngle == 0f)
            {
                return false;
            }

            Plane plane = new Plane(hitNormal, hit.point);
            Ray ray = new Ray(hit.point + remainingMove, _charUp);
            plane.Raycast(ray, out float enter);

            Vector3 slopeMove = remainingMove + (_charUp * enter);

            if (_maintainVelocityOnSurface == false)
            {
                slopeMove = slopeMove.normalized * remainingMove.magnitude;
            }

            remainingMove = slopeMove;
            return true;
    }

    protected bool _SlideAlongSurface(Vector3 originalMove, ref Vector3 remainingMove, RaycastHit hit, Vector3 hitNormal)
    {
        float remainingMoveSize = remainingMove.magnitude;

        if (hit.collider is null || remainingMoveSize == 0f)
            return false;

        _charCapsule.RecalculateNormalIfZero(hit, ref hitNormal);

        Vector3 hitProject = Vector3.ProjectOnPlane(hitNormal, _charUp);
        Vector3 slideMove = Vector3.ProjectOnPlane(originalMove.normalized * remainingMoveSize, hitProject);
        if (_maintainVelocityAlongSurface)
        {
            // to avoid sliding along perpendicular surface for very small values,
            // may be a result of small miscalculations
            if (slideMove.magnitude > _MIN_MOVE_ALONG_SURFACE_TO_MAINTAIN_VELOCITY)
            {
                slideMove = slideMove.normalized * remainingMoveSize;
            }
        }

        remainingMove = slideMove;
        return true;
    }

    protected bool _StepDown(Vector3 originalMove)
    {
        var verticalMove = Vector3.Project(originalMove, _charUp).magnitude;
        if (verticalMove != 0f || _stepDownDepth <= 0)
            return false;

        _charCapsule.CapsuleResolvePenetration();

        var moved = _charCapsule.CapsuleMove(_character.down * _stepDownDepth, out RaycastHit hit, out Vector3 hitNormal);

        if (_CanStandOn(hit, hitNormal) == false)
        {
            _charCapsule.capsule.position -= moved;
            return false;
        }

        return true;
    }

    protected bool _CheckIsGround(Collider collider)
    {
        if (collider == null)
        {
            return false;
        }

        return _groundLayer.Contains(collider.gameObject.layer);
    }

    protected bool _CanStandOn(RaycastHit hit)
    {
        return _CanStandOn(hit, Vector3.zero);
    }

    protected bool _CanStandOn(RaycastHit hit, Vector3 slopeNormal)
    {
        return _CanStandOn(hit, slopeNormal, out float slopeAngle);
    }

    protected bool _CanStandOn(RaycastHit hit, Vector3 slopeNormal, out float slopeAngle)
    {
        slopeAngle = 0f;

        if (hit.collider == null)
            return false;

        if (_CheckIsGround(hit.collider) is false)
        {
            return false;
        }

        _charCapsule.RecalculateNormalIfZero(hit, ref slopeNormal);

        slopeAngle = Vector3.Angle(_charUp, slopeNormal);
        if (slopeAngle < _MIN_SLOPE_ANGLE || slopeAngle > _maxSlopeUpAngle)
        {
            return false;
        }

        return true;
    }

    protected bool _CastForGround(float depth, out GroundResult result)
    {
        _charCapsule.BaseSphereCast(_character.down * depth, out RaycastHit hit, out Vector3 hitNormal);
        if (hitNormal == Vector3.zero)
        {
            hitNormal = hit.normal;
        }

        result = new GroundResult();

        if (_CanStandOn(hit, hit.normal, out float slopeAngle) is false)
        {
            if (slopeAngle < 90f)
            {
                return false;
            }
        }

        result.collider = hit.collider;
        result.direction = _character.down;
        result.distance = hit.distance;
        result.angle = slopeAngle;
        result.basePosition = result.collider.transform.position;
        result.baseRotation = result.collider.transform.rotation;
        result.edgeDistance = default;
        return true;
    }

    protected void _UpdateGroundResult()
    {
        _prevGroundResult = _groundResult;
        _CastForGround(_groundCheckDepth, out _groundResult);
    }

    //// -------------------------------------------------------------------------------------------
    //// Fields
    //// -------------------------------------------------------------------------------------------

    protected const uint _MAX_MOVE_ITERATIONS = 10;
    protected const float _MIN_MOVE_ALONG_SURFACE_TO_MAINTAIN_VELOCITY = .0001f;
    protected const float _MIN_SLOPE_ANGLE = 0f;
    protected const float _MAX_SLOPE_ANGLE = 89.9f;

    protected CharacterMovementGroundModuleAsset _moduleAsset;
    protected CharacterView _charView;
    protected CharacterCapsule _charCapsule;
    protected GroundResult _groundResult;
    protected GroundResult _prevGroundResult;
    protected Vector3 _baseDeltaPosition;
    protected Vector3 _baseDeltaRotation;

    protected MovementState _movementState;             // current state to process
    protected MovementState _lastMovementState;         // last processed state, could be same as current state
    protected MovementState _prevMovementState;         // previous state, different from current state

    protected Vector3 _moveVector = Vector3.zero;
    protected bool _doJump = false;
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
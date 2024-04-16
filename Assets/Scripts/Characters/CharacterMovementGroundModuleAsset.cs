using UnityEngine;

[CreateAssetMenu(menuName = MENU_PATH + "Ground Module")]
class CharacterMovementGroundModuleAsset : CharacterMovementModuleAsset
{
    public float checkDepth;
    public LayerMask groundLayer;
    public float minMoveDistance;

    public float standIdleSpeed;
    public float standIdleAcceleration;
    public float standWalkSpeed;
    public float standWalkAcceleration;
    public float standRunSpeed;
    public float standRunAcceleration;
    public float standSprintSpeed;
    public float standSprintAcceleration;
    public float standSprintLeftAngleMax;
    public float standSprintRightAngleMax;
    public float standJumpForce;
    public float standStepUpPercent;
    public float standStepDownPercent;
    public float standSlopeUpAngle;
    public float standSlopeDownAngle;
    public Vector3 standCapsuleCenter;
    public float standCapsuleHeight;
    public float standCapsuleRadius;
    public float standToCrouchTransitionSpeed;
    public bool standMaintainVelocityOnSurface;
    public bool standMaintainVelocityAlongSurface;

    public float crouchIdleSpeed;
    public float crouchIdleAcceleration;
    public float crouchWalkSpeed;
    public float crouchWalkAcceleration;
    public float crouchRunSpeed;
    public float crouchRunAcceleration;
    public bool crouchAutoRiseToStandSprint;
    public float crouchJumpForce;
    public float crouchStepUpPercent;
    public float crouchStepDownPercent;
    public float crouchSlopeUpAngle;
    public float crouchSlopeDownAngle;
    public Vector3 crouchCapsuleCenter;
    public float crouchCapsuleHeight;
    public float crouchCapsuleRadius;
    public float crouchToStandTransitionSpeed;
    public bool crouchMaintainVelocityOnSurface;
    public bool crouchMaintainVelocityAlongSurface;

    public override CharacterMovementModule GetModule()
    {
        return new CharacterMovementGroundModule(null);
    }
}
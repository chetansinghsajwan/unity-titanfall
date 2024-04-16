using System;
using UnityEngine;

[CreateAssetMenu(menuName = MENU_PATH + MENU_NAME, fileName = FILE_NAME)]
partial class CharacterAsset : DataAsset
{
    public const string MENU_PATH = "Character/";
    public const string MENU_NAME = "Character Asset";
    public const string FILE_NAME = "Character Asset";

    public CharacterAsset()
    {
        _instanceHandler = new InstanceHandler(this);
    }

    public override void OnLoad()
    {
        base.OnLoad();

        _instanceHandler.CreatePool();
    }

    public override void OnUnload()
    {
        base.OnUnload();

        _instanceHandler.DisposePool();
    }

    [Space]

    public string characterName;
    public GameObject tppBody;
    public GameObject fppBody;
    public GameObject tppPrefab;
    public GameObject fppPrefab;

    [Min(0)]
    public float characterMass;

    /// --------------------------------------------------------------------------------------------
    /// Movement
    /// --------------------------------------------------------------------------------------------

    [Space, Header("Movement")]

    public float groundCheckDepth;
    public LayerMask groundLayer;
    public float groundMinMoveDistance;

    public float groundStandIdleSpeed;
    public float groundStandIdleAcceleration;
    public float groundStandWalkSpeed;
    public float groundStandWalkAcceleration;
    public float groundStandRunSpeed;
    public float groundStandRunAcceleration;
    public float groundStandSprintSpeed;
    public float groundStandSprintAcceleration;
    public float groundStandSprintLeftAngleMax;
    public float groundStandSprintRightAngleMax;
    public float groundStandJumpForce;
    public float groundStandStepUpPercent;
    public float groundStandStepDownPercent;
    public float groundStandSlopeUpAngle;
    public float groundStandSlopeDownAngle;
    public Vector3 groundStandCapsuleCenter;
    public float groundStandCapsuleHeight;
    public float groundStandCapsuleRadius;
    public float groundStandToCrouchTransitionSpeed;
    public bool groundStandMaintainVelocityOnSurface;
    public bool groundStandMaintainVelocityAlongSurface;

    public float groundCrouchIdleSpeed;
    public float groundCrouchIdleAcceleration;
    public float groundCrouchWalkSpeed;
    public float groundCrouchWalkAcceleration;
    public float groundCrouchRunSpeed;
    public float groundCrouchRunAcceleration;
    public bool groundCrouchAutoRiseToStandSprint;
    public float groundCrouchJumpForce;
    public float groundCrouchStepUpPercent;
    public float groundCrouchStepDownPercent;
    public float groundCrouchSlopeUpAngle;
    public float groundCrouchSlopeDownAngle;
    public Vector3 groundCrouchCapsuleCenter;
    public float groundCrouchCapsuleHeight;
    public float groundCrouchCapsuleRadius;
    public float groundCrouchToStandTransitionSpeed;
    public bool groundCrouchMaintainVelocityOnSurface;
    public bool groundCrouchMaintainVelocityAlongSurface;

    public float airGravityAcceleration;
    public float airGravityMaxSpeed;
    public float airMinMoveDistance;
    public float airMoveSpeed;
    public float airMoveAcceleration;
    public float airJumpPower;
    public uint airMaxJumpCount;

    /// --------------------------------------------------------------------------------------------
    /// Animations
    /// --------------------------------------------------------------------------------------------

    [Space, Header("Animation")]

    public Avatar avatar;
    public Avatar avatarMaskUpperBody;
    public Avatar avatarMaskLowerBody;

    public AnimationClip animGroundStandIdle;
    public AnimationClip animGroundStandWalkForward;
    public AnimationClip animGroundStandWalkForwardLeft;
    public AnimationClip animGroundStandWalkForwardRight;
    public AnimationClip animGroundStandWalkLeft;
    public AnimationClip animGroundStandWalkRight;
    public AnimationClip animGroundStandWalkBackward;
    public AnimationClip animGroundStandWalkBackwardLeft;
    public AnimationClip animGroundStandWalkBackwardRight;
    public AnimationClip animGroundStandRunForward;
    public AnimationClip animGroundStandRunForwardLeft;
    public AnimationClip animGroundStandRunForwardRight;
    public AnimationClip animGroundStandRunLeft;
    public AnimationClip animGroundStandRunRight;
    public AnimationClip animGroundStandRunBackward;
    public AnimationClip animGroundStandRunBackwardLeft;
    public AnimationClip animGroundStandRunBackwardRight;
    public AnimationClip animGroundStandSprintForward;

    public AnimationClip animGroundCrouchIdle;
    public AnimationClip animGroundCrouchWalkForward;
    public AnimationClip animGroundCrouchWalkForwardLeft;
    public AnimationClip animGroundCrouchWalkForwardRight;
    public AnimationClip animGroundCrouchWalkLeft;
    public AnimationClip animGroundCrouchWalkRight;
    public AnimationClip animGroundCrouchWalkBackward;
    public AnimationClip animGroundCrouchWalkBackwardLeft;
    public AnimationClip animGroundCrouchWalkBackwardRight;
    public AnimationClip animGroundCrouchRunForward;
    public AnimationClip animGroundCrouchRunForwardLeft;
    public AnimationClip animGroundCrouchRunForwardRight;
    public AnimationClip animGroundCrouchRunLeft;
    public AnimationClip animGroundCrouchRunRight;
    public AnimationClip animGroundCrouchRunBackward;
    public AnimationClip animGroundCrouchRunBackwardLeft;
    public AnimationClip animGroundCrouchRunBackwardRight;
}
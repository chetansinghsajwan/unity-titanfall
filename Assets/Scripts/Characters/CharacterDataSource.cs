﻿using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Asset")]
public class CharacterDataSource : ScriptableObject
{
    [Space]

    [Label("Character Name"), SerializeField]
    protected string _characterName;
    public string characterName => _characterName;

    [Label("Character TPP Prefab"), SerializeField]
    protected GameObject _tppPrefab;
    public GameObject tppPrefab => _tppPrefab;

    [Label("Character FPP Prefab"), SerializeField]
    protected GameObject _fppPrefab;
    public GameObject fppPrefab => _fppPrefab;

    [Label("Character Mass"), SerializeField, Min(0)]
    protected float _characterMass;
    public float characterMass => _characterMass;

    [Space, Header("CHARACTER MOVEMENT")]

    #region GROUND DATA

    [Header("# GROUND DATA"), Space]

    //////////////////////////////////////////////////////////////////
    // Ground Check Depth
    [Tooltip("Distance below character to check for ground")]
    [Label("Check Depth"), SerializeField, Range(0.01f, 0.5f)]
    protected float m_groundCheckDepth;
    public float groundCheckDepth => m_groundCheckDepth;

    //////////////////////////////////////////////////////////////////
    // Ground Layer Mask
    [Tooltip("Layer Mask to determine if the collider is ground")]
    [Label("Layer"), SerializeField]
    protected LayerMask m_groundLayer;
    public LayerMask groundLayer => m_groundLayer;

    //////////////////////////////////////////////////////////////////
    // Ground Min Move Distance
    [Tooltip("Minimum distance the character should move")]
    [Label("Min Move Distance"), SerializeField, Min(0f)]
    protected float m_groundMinMoveDistance;
    public float groundMinMoveDistance => m_groundMinMoveDistance;

    #region GROUND STAND DATA

    [Header("## GROUND STAND DATA"), Space]

    //////////////////////////////////////////////////////////////////
    // Ground Stand Idle Speed & Acceleration
    [Tooltip("Speed when there is no movement input")]
    [Label("Idle Speed"), SerializeField, Min(0)]
    protected float m_groundStandIdleSpeed;
    public float groundStandIdleSpeed => m_groundStandIdleSpeed;

    [Tooltip("Acceleration (BrakingAcceleration if IdleSpeed = 0) when there is no movement input")]
    [Label("Idle Acceleration"), SerializeField, Min(0)]
    protected float m_groundStandIdleAcceleration;
    public float groundStandIdleAcceleration => m_groundStandIdleAcceleration;

    //////////////////////////////////////////////////////////////////
    // Ground Stand Walk Speed & Acceleration
    [Label("Walk Speed"), SerializeField, Min(0), Space]
    protected float m_groundStandWalkSpeed;
    public float groundStandWalkSpeed => m_groundStandWalkSpeed;

    [Label("Walk Acceleration"), SerializeField, Min(0)]
    protected float m_groundStandWalkAcceleration;
    public float groundStandWalkAcceleration => m_groundStandWalkAcceleration;

    //////////////////////////////////////////////////////////////////
    // Ground Stand Run Speed & Acceleration
    [Label("Run Speed"), SerializeField, Min(0), Space]
    protected float m_groundStandRunSpeed;
    public float groundStandRunSpeed => m_groundStandRunSpeed;

    [Label("Run Acceleration"), SerializeField, Min(0)]
    protected float m_groundStandRunAcceleration;
    public float groundStandRunAcceleration => m_groundStandRunAcceleration;

    //////////////////////////////////////////////////////////////////
    // Ground Stand Sprint Speed & Acceleration & Angle (Left-Right)
    [Label("Sprint Speed"), SerializeField, Min(0), Space]
    protected float m_groundStandSprintSpeed;
    public float groundStandSprintSpeed => m_groundStandSprintSpeed;

    [Label("Sprint Acceleration"), SerializeField, Min(0)]
    protected float m_groundStandSprintAcceleration;
    public float groundStandSprintAcceleration => m_groundStandSprintAcceleration;

    [Label("Sprint Angle (Left)"), SerializeField, Range(-90f, 00f)]
    protected float m_groundStandSprintLeftAngleMax;
    public float groundStandSprintLeftAngleMax => m_groundStandSprintLeftAngleMax;

    [Label("Sprint Angle (Right)"), SerializeField, Range(000f, 90f)]
    protected float m_groundStandSprintRightAngleMax;
    public float groundStandSprintRightAngleMax => m_groundStandSprintRightAngleMax;

    //////////////////////////////////////////////////////////////////
    // Ground Stand Jump Force
    [Label("Jump Force"), SerializeField, Min(0), Space]
    protected float m_groundStandJumpForce;
    public float groundStandJumpForce => m_groundStandJumpForce;

    //////////////////////////////////////////////////////////////////
    // Ground Stand StepUp Percent
    [Label("Step Up (%)"), SerializeField, Min(0), Space]
    protected float m_groundStandStepUpPercent;
    public float groundStandStepUpPercent => m_groundStandStepUpPercent;

    //////////////////////////////////////////////////////////////////
    // Ground Stand StepDown Percent
    [Label("Step Down (%)"), SerializeField, Min(0)]
    protected float m_groundStandStepDownPercent;
    public float groundStandStepDownPercent => m_groundStandStepDownPercent;

    //////////////////////////////////////////////////////////////////
    // Ground Stand Slope Up Angle
    [Label("Slope Up Angle"), SerializeField, Range(0, 89f)]
    protected float m_groundStandSlopeUpAngle;
    public float groundStandSlopeUpAngle => m_groundStandSlopeUpAngle;

    //////////////////////////////////////////////////////////////////
    // Ground Stand Slope Down Angle
    [Label("Slope Down Angle"), SerializeField, Range(0, -89)]
    protected float m_groundStandSlopeDownAngle;
    public float groundStandSlopeDownAngle => m_groundStandSlopeDownAngle;

    //////////////////////////////////////////////////////////////////
    // Ground Stand Maintain Velocity (On & Along) Surface
    [Label("Maintain Velocity On Surface"), SerializeField]
    protected bool m_groundStandMaintainVelocityOnSurface;
    public bool groundStandMaintainVelocityOnSurface => m_groundStandMaintainVelocityOnSurface;

    [Label("Maintain Velocity Along Surface"), SerializeField]
    protected bool m_groundStandMaintainVelocityAlongSurface;
    public bool groundStandMaintainVelocityAlongSurface => m_groundStandMaintainVelocityAlongSurface;

    //////////////////////////////////////////////////////////////////
    // Ground Stand Capsule Properties
    [Label("Capsule Center"), SerializeField, Min(0), Space]
    protected Vector3 m_groundStandCapsuleCenter;
    public Vector3 groundStandCapsuleCenter => m_groundStandCapsuleCenter;

    [Label("Capsule Height"), SerializeField, Min(0)]
    protected float m_groundStandCapsuleHeight;
    public float groundStandCapsuleHeight => m_groundStandCapsuleHeight;

    [Label("Capsule Radius"), SerializeField, Min(0)]
    protected float m_groundStandCapsuleRadius;
    public float groundStandCapsuleRadius => m_groundStandCapsuleRadius;

    //////////////////////////////////////////////////////////////////
    // Ground Stand To Crouch Transition
    [Label("Crouch Transition Speed"), SerializeField, Min(0), Space]
    protected float m_groundStandToCrouchTransitionSpeed;
    public float groundStandToCrouchTransitionSpeed => m_groundStandToCrouchTransitionSpeed;

    #endregion

    #region GROUND CROUCH DATA

    [Header("GROUND CROUCH DATA"), Space]

    //////////////////////////////////////////////////////////////////
    // Ground Crouch Idle Speed & Acceleration
    [Label("Idle Speed"), SerializeField, Min(0)]
    protected float m_groundCrouchIdleSpeed;
    public float groundCrouchIdleSpeed => m_groundCrouchIdleSpeed;

    [Label("Idle Acceleration"), SerializeField, Min(0)]
    protected float m_groundCrouchIdleAcceleration;
    public float groundCrouchIdleAcceleration => m_groundCrouchIdleAcceleration;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch Walk Speed & Acceleration
    [Label("Walk Speed"), SerializeField, Min(0), Space]
    protected float m_groundCrouchWalkSpeed;
    public float groundCrouchWalkSpeed => m_groundCrouchWalkSpeed;

    [Label("Walk Acceleration"), SerializeField, Min(0)]
    protected float m_groundCrouchWalkAcceleration;
    public float groundCrouchWalkAcceleration => m_groundCrouchWalkAcceleration;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch Run Speed & Acceleration
    [Label("Run Speed"), SerializeField, Min(0), Space]
    protected float m_groundCrouchRunSpeed;
    public float groundCrouchRunSpeed => m_groundCrouchRunSpeed;

    [Label("Run Acceleration"), SerializeField, Min(0)]
    protected float m_groundCrouchRunAcceleration;
    public float groundCrouchRunAcceleration => m_groundCrouchRunAcceleration;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch To Stand Sprint
    [Label("AutoRise to StandSprint"), SerializeField, Space]
    protected bool m_groundCrouchAutoRiseToStandSprint;
    public bool groundCrouchAutoRiseToStandSprint => m_groundCrouchAutoRiseToStandSprint;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch Jump Force
    [Label("Jump Force"), SerializeField, Min(0), Space]
    protected float m_groundCrouchJumpForce;
    public float groundCrouchJumpForce => m_groundCrouchJumpForce;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch StepUp Percent
    [Label("Step Up (%)"), SerializeField, Min(0), Space]
    protected float m_groundCrouchStepUpPercent;
    public float groundCrouchStepUpPercent => m_groundCrouchStepUpPercent;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch StepDown Percent
    [Label("Step Down (%)"), SerializeField, Min(0)]
    protected float m_groundCrouchStepDownPercent;
    public float groundCrouchStepDownPercent => m_groundCrouchStepDownPercent;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch Slope Up Angle
    [Label("Slope Up Angle"), SerializeField, Range(0, 89f)]
    protected float m_groundCrouchSlopeUpAngle;
    public float groundCrouchSlopeUpAngle => m_groundCrouchSlopeUpAngle;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch Slope Down Angle
    [Label("Slope Down Angle"), SerializeField, Range(0, -89)]
    protected float m_groundCrouchSlopeDownAngle;
    public float groundCrouchSlopeDownAngle => m_groundCrouchSlopeDownAngle;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch Maintain Velocity (On & Along) Surface
    [Label("Maintain Velocity On Surface"), SerializeField]
    protected bool m_groundCrouchMaintainVelocityOnSurface;
    public bool groundCrouchMaintainVelocityOnSurface => m_groundCrouchMaintainVelocityOnSurface;

    [Label("Maintain Velocity Along Surface"), SerializeField]
    protected bool m_groundCrouchMaintainVelocityAlongSurface;
    public bool groundCrouchMaintainVelocityAlongSurface => m_groundCrouchMaintainVelocityAlongSurface;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch Capsule Properties
    [Label("Capsule Center"), SerializeField, Min(0), Space]
    protected Vector3 m_groundCrouchCapsuleCenter;
    public Vector3 groundCrouchCapsuleCenter => m_groundCrouchCapsuleCenter;

    [Label("Capsule Height"), SerializeField, Min(0)]
    protected float m_groundCrouchCapsuleHeight;
    public float groundCrouchCapsuleHeight => m_groundCrouchCapsuleHeight;

    [Label("Capsule Radius"), SerializeField, Min(0)]
    protected float m_groundCrouchCapsuleRadius;
    public float groundCrouchCapsuleRadius => m_groundCrouchCapsuleRadius;

    //////////////////////////////////////////////////////////////////
    // Ground Crouch To Stand Transition
    [Label("Stand Transition Speed"), SerializeField, Min(0), Space]
    protected float m_groundCrouchToStandTransitionSpeed;
    public float groundCrouchToStandTransitionSpeed => m_groundCrouchToStandTransitionSpeed;

    #endregion

    #endregion

    #region AIR DATA

    [Header("# AIR DATA"), Space]

    [Space]
    //////////////////////////////////////////////////////////////////
    // Air Gravity Acceleration & MaxSpeed
    [Label("Gravity Acceleration"), SerializeField]
    protected float m_airGravityAcceleration;
    public float airGravityAcceleration => m_airGravityAcceleration;

    [Label("Gravity Max Speed"), SerializeField, Min(0f)]
    protected float m_airGravityMaxSpeed;
    public float airGravityMaxSpeed => m_airGravityMaxSpeed;

    [Space]
    //////////////////////////////////////////////////////////////////
    // Air Min Move Distance
    [Label("Min Move Distance"), SerializeField, Min(0f)]
    protected float m_airMinMoveDistance;
    public float airMinMoveDistance => m_airMinMoveDistance;

    [Space]
    //////////////////////////////////////////////////////////////////
    // Air Move Speed & Acceleration
    [Label("Move Speed"), SerializeField, Min(0f)]
    protected float m_airMoveSpeed;
    public float airMoveSpeed => m_airMoveSpeed;

    [Label("Move Acceleration"), SerializeField, Min(0f)]
    protected float m_airMoveAcceleration;
    public float airMoveAcceleration => m_airMoveAcceleration;

    [Space]
    //////////////////////////////////////////////////////////////////
    // Air Jump Power and MaxCount
    [Label("Jump Power"), SerializeField]
    protected uint m_airJumpPower;
    public float airJumpPower => m_airJumpPower;

    [Label("Max Jump Count"), SerializeField]
    protected uint m_airMaxJumpCount;
    public uint airMaxJumpCount => m_airMaxJumpCount;

    #endregion

    public CharacterDataSource()
    {
        _characterName = "UNNAMED";
        _tppPrefab = null;
        _fppPrefab = null;
        _characterMass = 80f;

        /// GroundData
        m_groundCheckDepth = 0.05f;
        m_groundLayer = 0;
        m_groundMinMoveDistance = 0.001f;

        /// Ground Stand Data
        m_groundStandWalkSpeed = 10;
        m_groundStandRunSpeed = 15;
        m_groundStandSprintSpeed = 25;
        m_groundStandSprintLeftAngleMax = -46;
        m_groundStandSprintRightAngleMax = 46;
        m_groundStandJumpForce = 40;
        m_groundStandStepUpPercent = 10;
        m_groundStandStepDownPercent = 10;
        m_groundStandSlopeUpAngle = 45;
        m_groundStandSlopeDownAngle = 55;
        m_groundStandMaintainVelocityOnSurface = false;
        m_groundStandMaintainVelocityAlongSurface = false;
        m_groundStandToCrouchTransitionSpeed = 10;

        /// Ground Crouch Data
        m_groundCrouchWalkSpeed = 8;
        m_groundCrouchRunSpeed = 12;
        m_groundCrouchJumpForce = 50;
        m_groundCrouchStepUpPercent = 5;
        m_groundCrouchStepDownPercent = 5;
        m_groundCrouchSlopeUpAngle = 55;
        m_groundCrouchSlopeDownAngle = 65;
        m_groundCrouchMaintainVelocityOnSurface = false;
        m_groundCrouchMaintainVelocityAlongSurface = false;
        m_groundCrouchAutoRiseToStandSprint = true;
        m_groundCrouchToStandTransitionSpeed = 10;

        m_airGravityAcceleration = -9.8f;
        m_airGravityMaxSpeed = float.PositiveInfinity;
        m_airMinMoveDistance = 0f;
        m_airMoveSpeed = .1f;
        m_airMoveAcceleration = .1f;
        m_airJumpPower = 10;
        m_airMaxJumpCount = 1;
    }

    public Character InstantiateTPP(Vector3 pos, Quaternion rot)
    {
        if (_tppPrefab == null)
        {
            return null;
        }

        GameObject go = GameObject.Instantiate(_tppPrefab, pos, rot);
        Character character = go.GetComponent<Character>();

        // add the character initializer
        CharacterInitializer initializer = go.AddComponent<CharacterInitializer>();
        initializer.destroyOnUse = true;
        initializer.source = this;

        // init the character
        character.Init();

        return character;
    }
}
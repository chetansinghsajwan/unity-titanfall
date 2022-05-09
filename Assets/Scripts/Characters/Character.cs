﻿using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterInputs))]
[RequireComponent(typeof(CharacterMovement))]
[RequireComponent(typeof(CharacterInteraction))]
[RequireComponent(typeof(CharacterWeapon))]
[RequireComponent(typeof(CharacterCamera))]
[RequireComponent(typeof(CharacterCapsule))]
[RequireComponent(typeof(CharacterAnimation))]
public class Character : MonoBehaviour
{
    //////////////////////////////////////////////////////////////////
    /// Variables
    //////////////////////////////////////////////////////////////////

    public CharacterInputs CharacterInputs { get; protected set; }
    public CharacterMovement CharacterMovement { get; protected set; }
    public CharacterInteraction CharacterInteraction { get; protected set; }
    public CharacterWeapon CharacterWeapon { get; protected set; }
    public CharacterCamera CharacterCamera { get; protected set; }
    public CharacterCapsule CharacterCapsule { get; protected set; }
    public CharacterAnimation CharacterAnimation { get; protected set; }

    public float Mass { get; set; } = 80f;
    public float ScaledMass
    {
        get => transform.lossyScale.magnitude * Mass;
        set => Mass = value / transform.lossyScale.magnitude;
    }

    public Quaternion GetRotation => transform.rotation;
    public Vector3 GetForward => transform.forward;
    public Vector3 GetBack => -transform.forward;
    public Vector3 GetRight => transform.right;
    public Vector3 GetLeft => -transform.right;
    public Vector3 GetUp => transform.up;
    public Vector3 GetDown => -transform.up;

    void Awake()
    {
        CharacterInputs = GetComponent<CharacterInputs>();
        CharacterMovement = GetComponent<CharacterMovement>();
        CharacterInteraction = GetComponent<CharacterInteraction>();
        CharacterWeapon = GetComponent<CharacterWeapon>();
        CharacterCamera = GetComponent<CharacterCamera>();
        CharacterCapsule = GetComponent<CharacterCapsule>();
        CharacterAnimation = GetComponent<CharacterAnimation>();

        CharacterInputs.Init(this);
        CharacterMovement.Init(this);
        CharacterCamera.Init(this);
        CharacterCapsule.Init(this);
        CharacterInteraction.Init(this);
        CharacterWeapon.Init(this);
        CharacterAnimation.Init(this);
    }

    void Start()
    {
    }

    void Update()
    {
        CharacterInputs.UpdateImpl();
        CharacterMovement.UpdateImpl();
        CharacterCapsule.UpdateImpl();
        CharacterCamera.UpdateImpl();
        CharacterInteraction.UpdateImpl();
        CharacterWeapon.UpdateImpl();
        CharacterAnimation.UpdateImpl();
    }

    void FixedUpdate()
    {
        CharacterMovement.FixedUpdateImpl();
    }

    public void OnPossessed(Player Player)
    {
        if (Player == null)
            return;

        CharacterInputs.PlayerInputs = Player.PlayerInputs;
    }

    public void OnUnPossessed()
    {
        CharacterInputs.PlayerInputs = null;
    }
}
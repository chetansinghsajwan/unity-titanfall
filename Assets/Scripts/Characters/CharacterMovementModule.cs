using System;
using UnityEngine;
using GameFramework.Extensions;
using System.Diagnostics.Contracts;

abstract class CharacterMovementModule
{
    public virtual void OnLoaded(CharacterMovement charMovement)
    {
        Contract.Requires(charMovement != null);

        _charMovement = charMovement;
        _character = _charMovement.character;
    }

    public virtual void OnUnloaded()
    {
        Contract.Requires(_charMovement != null);

        _character = null;
        _charMovement = null;
    }

    public virtual void Update() { }

    public abstract bool ShouldRun();

    public virtual void StartPhysics() { }

    public abstract void RunPhysics(out VirtualCapsule result);

    public virtual void StopPhysics() { }

    public virtual void PostUpdate() { }

    protected Character _character;
    protected CharacterMovement _charMovement;
}

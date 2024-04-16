using UnityEngine;
using System.Diagnostics.Contracts;

class CharacterInteractionModuleForReloadableWeapon: CharacterInteractionModule
{
    public CharacterInteractionModuleForReloadableWeapon(ReloadableWeapon weapon)
    {
        Contract.Requires(weapon != null);

        _weapon = weapon;
    }

    public override void OnActivate(CharacterInteraction charInteraction)
    {
        base.OnActivate(charInteraction);

        _charInteration = charInteraction;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        _charInteration = null;
    }

    public override void OnUpdate()
    {
        bool fire = _inputFire && !_isReloading;
        bool startReload = _inputReload && !_inputFire && !_inputStopReload && !_isReloading;
        bool stopReload = _inputStopReload && _isReloading;

        if (startReload)
        {
            _StartReload();
        }
        else if (stopReload)
        {
            _StopReload();
        }
        else if (fire)
        {
            if (!_isFiring)
            {
                _isFiring = true;
                _weapon.OnTriggerDown();
            }
        }
        else
        {
            if (_isFiring)
            {
                _isFiring = false;
                _weapon.OnTriggerUp();
            }
        }
    }

    protected void _StartReload()
    {
        _isReloading = true;
    }

    protected void _StopReload()
    {
        _isReloading = false;
    }
    
    public void StartReload()
    {
        _inputReload = true;
    }

    public void StopReload()
    {
        _inputStopReload = true;
    }

    public void Fire()
    {
        _inputFire = true;
    }

    public void ScopeIn()
    {
        _inputScopeIn = true;
    }

    public void ScopeOut()
    {
        _inputScopeIn = false;
    }

    protected CharacterInteraction _charInteration;
    protected ReloadableWeapon _weapon = null;
    protected bool _isFiring = false;
    protected bool _isReloading = false;
    protected bool _canStopReloading = false;

    protected bool _inputFire = false;
    protected bool _inputReload = false;
    protected bool _inputStopReload = false;
    protected bool _inputScopeIn = false;
}

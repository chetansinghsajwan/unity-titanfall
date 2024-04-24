using System.Diagnostics.Contracts;
using UnityEngine;

class PlayerInputs : PlayerBehaviour
{
    public override void OnPlayerPossess(Character character)
    {
        if (character is null)
        {
            _character = null;
            _charMovement = null;
            return;
        }

        _character = character;
        _charMovement = _character.charMovement;
        _charInteration = _character.charInteraction;
    }

    public override void OnPlayerUpdate()
    {
        if (_character == null)
            return;

        _ProcessInputs();
        _HandleCharacterMovement();
        _HandleCharacterView();
        _HandleCharacterInteraction();
    }

    protected void _ProcessInputs()
    {
        _move = _GetMoveVector();
        _look = _GetLookVector();
        _walk = Input.GetKey(KeyCode.LeftControl);
        _sprint = Input.GetKey(KeyCode.LeftShift);
        _crouch = Input.GetKey(KeyCode.C);
        _prone = Input.GetKey(KeyCode.V);
        _jump = Input.GetKeyDown(KeyCode.Space);

        _weapon1 = Input.GetKeyDown(KeyCode.Alpha1);
        _weapon2 = Input.GetKeyDown(KeyCode.Alpha2);
        _weapon3 = Input.GetKeyDown(KeyCode.Alpha3);
        _grenade1 = Input.GetKeyDown(KeyCode.Alpha4);
        _grenade2 = Input.GetKeyDown(KeyCode.Alpha5);
        _reload = Input.GetKeyDown(KeyCode.R);
        _leftFire = Input.GetMouseButton(2);
        _rightFire = Input.GetMouseButton(0);
        _sight = Input.GetMouseButton(1);
        _interact = Input.GetKeyDown(KeyCode.E);
    }

    protected Vector2 _GetMoveVector()
    {
        Vector2 move = Vector2.zero;
        move.y += Input.GetKey(KeyCode.W) ? +1 : 0;
        move.x += Input.GetKey(KeyCode.A) ? -1 : 0;
        move.y += Input.GetKey(KeyCode.S) ? -1 : 0;
        move.x += Input.GetKey(KeyCode.D) ? +1 : 0;
        return move;
    }

    protected Vector2 _GetLookVector()
    {
        Vector2 mousePosition = new Vector2(-Input.mousePosition.x, Input.mousePosition.y);
        Vector2 deltaMove = _lastMousePosition - mousePosition;
        _lastMousePosition = mousePosition;
        return deltaMove;
    }

    protected void _HandleCharacterMovement()
    {
        CharacterMovementModule activeModule = _charMovement.activeModule;
        if (activeModule is CharacterMovementGroundModule)
        {
            _HandleCharacterMovementGroundModule(activeModule as CharacterMovementGroundModule);
        }
        else if (activeModule is CharacterMovementAirModule)
        {
            _HandleCharacterMovementAirModule(activeModule as CharacterMovementAirModule);
        }
    }

    protected void _HandleCharacterMovementGroundModule(CharacterMovementGroundModule module)
    {
        Contract.Assert(module is not null);

        module.SetMoveVector(_move);

        if (_crouch)
        {
            if (_walk) module.SwitchToCrouchWalk();
            else module.SwitchToCrouchRun();
        }
        else
        {
            if (_walk) module.SwitchToStandWalk();
            else if (_sprint) module.SwitchToStandSprint();
            else module.SwitchToStandRun();

            if (_jump) module.Jump();
        }
    }

    protected void _HandleCharacterMovementAirModule(CharacterMovementAirModule module)
    {
    }

    protected void _HandleCharacterView()
    {
        CharacterView charView = _character.charView;
        if (charView is not null)
        {
            charView.SetLookVector(_look);
        }
    }

    protected void _HandleCharacterInteraction()
    {
        // if (_weapon1) _charInteration.SwitchToWeapon1();
        // else if (_weapon2) _charInteration.SwitchToWeapon2();
        // else if (_weapon3) _charInteration.SwitchToWeapon3();

        // if (_grenade1) _charInteration.SwitchToGrenade1();
        // else if (_grenade2) _charInteration.SwitchToGrenade2();

        // if (_interact)
        // {
        //     Interactable interactable = _playerInteraction.GetInteractable();
        //     if (interactable is not null)
        //     {
        //         Equipable equipable = interactable as Equipable;
        //         if (equipable is not null)
        //         {
        //             _charInteration.Pick(equipable);
        //         }
        //     }
        // }

        var interactionModule = _charInteration.GetActiveModule()
            as CharacterInteractionModuleForReloadableWeapon;
        if (interactionModule is not null)
        {
            if (_leftFire) interactionModule.Fire();

            if (_rightFire) interactionModule.ScopeIn();
            else interactionModule.ScopeOut();

            if (_reload) interactionModule.StartReload();
        }
    }

    protected Character _character = null;
    protected CharacterMovement _charMovement = null;
    protected CharacterInteraction _charInteration = null;
    protected PlayerInteraction _playerInteraction;

    protected Vector2 _move = Vector2.zero;
    protected Vector2 _look = Vector2.zero;
    protected Vector2 _lastMousePosition = Vector2.zero;
    protected bool _walk = false;
    protected bool _sprint = false;
    protected bool _crouch = false;
    protected bool _prone = false;
    protected bool _jump = false;
    protected bool _weapon1;
    protected bool _weapon2;
    protected bool _weapon3;
    protected bool _grenade1;
    protected bool _grenade2;
    protected bool _reload;
    protected bool _leftFire;
    protected bool _rightFire;
    protected bool _sight;
    protected bool _interact;
}
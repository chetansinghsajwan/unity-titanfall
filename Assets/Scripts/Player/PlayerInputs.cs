using UnityEngine;

class PlayerInputs : PlayerBehaviour
{
    protected Character _character = null;
    protected CharacterMovement _characterMovement = null;
    protected CharacterMovementGroundModule _characterMovementGroundModule = null;
    protected PlayerInteraction _playerInteraction;

    protected Vector2 _move = Vector2.zero;
    protected Vector2 _look = Vector2.zero;
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

    public override void OnPlayerPossess(Character character)
    {
        if (character is null)
        {
            _character = null;
            _characterMovement = null;
            _characterMovementGroundModule = null;
            return;
        }
        
        _character = character;
        _characterMovement = _character.charMovement;

        foreach (var module in _characterMovement.modules)
        {
            if (module is CharacterMovementGroundModule)
            {
                _characterMovementGroundModule = module as CharacterMovementGroundModule;
                break;
            }
        }
    }

    public override void OnPlayerUpdate()
    {
        if (_character is not null)
        {
            ProcessInputs();

            if (_characterMovementGroundModule is not null)
            {
                _characterMovementGroundModule.SetMoveVector(_move);

                if (_sprint) _characterMovementGroundModule.Sprint();
                if (_walk) _characterMovementGroundModule.Walk();
                if (_jump) _characterMovementGroundModule.Jump();
                if (_crouch) _characterMovementGroundModule.Crouch();
                if (_prone) _characterMovementGroundModule.Prone();
            }

            CharacterView charView = _character.charView;
            if (charView is not null)
            {
                charView.SetLookVector(_look);
            }

            CharacterObjectHandler charObjHandler = _character.charObjectHandler;
            if (charObjHandler is not null)
            {
                if (_weapon1) charObjHandler.SwitchToWeapon1();
                else if (_weapon2) charObjHandler.SwitchToWeapon2();
                else if (_weapon3) charObjHandler.SwitchToWeapon3();

                if (_grenade1) charObjHandler.SwitchToGrenade1();
                else if (_grenade2) charObjHandler.SwitchToGrenade2();

                if (_leftFire) charObjHandler.FireLeftWeapon();
                if (_rightFire) charObjHandler.FireRightWeapon();

                if (_reload) charObjHandler.ReloadRightWeapon();

                if (_interact)
                {
                    Interactable interactable = _playerInteraction.GetInteractable();
                    if (interactable is not null)
                    {
                        Equipable equipable = interactable as Equipable;
                        if (equipable is not null)
                        {
                            charObjHandler.Pick(equipable);
                        }
                    }
                }
            }
        }
    }

    protected virtual void ProcessInputs()
    {
        _move = GetMoveVector();
        _look = GetLookVector();
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

    protected Vector2 GetMoveVector()
    {
        Vector2 move = Vector2.zero;
        move.y += Input.GetKey(KeyCode.W) ? +1 : 0;
        move.x += Input.GetKey(KeyCode.A) ? -1 : 0;
        move.y += Input.GetKey(KeyCode.S) ? -1 : 0;
        move.x += Input.GetKey(KeyCode.D) ? +1 : 0;
        return move;
    }

    protected Vector2 GetLookVector()
    {
        Vector2 look = Vector2.zero;
        look.x = Input.GetAxis("look x");
        look.y = Input.GetAxis("look y");
        return look;
    }
}
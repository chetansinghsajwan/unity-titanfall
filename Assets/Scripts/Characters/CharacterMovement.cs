using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(CapsuleCollider))]
class CharacterMovement : CharacterBehaviour
{
    public override void OnCharacterCreate(Character character, CharacterInitializer initializer)
    {
        base.OnCharacterCreate(character, initializer);
        CharacterAsset charAsset = _character.charAsset;

        _capsule = new VirtualCapsule
        {
            position = Vector3.zero,
            rotation = Quaternion.identity,
            height = 2f,
            radius = .5f,
            layerMask = charAsset.groundLayer,
            triggerInteraction = QueryTriggerInteraction.Ignore
        };
        _skinWidth = .02f;
        _collider = GetComponent<CapsuleCollider>();
        _velocity = Vector3.zero;

        var moduleList = new List<CharacterMovementModule>();
        var groundModule = new CharacterMovementGroundModule(charAsset);
        var airModule = new CharacterMovementAirModule(charAsset);
        moduleList.Add(groundModule);
        moduleList.Add(airModule);

        _modules = moduleList.ToArray();
        foreach (var module in _modules)
        {
            module.OnLoaded(this);
        }
    }

    public override void OnCharacterSpawn()
    {
        base.OnCharacterSpawn();

        _capsule.position = transform.position;
        _capsule.rotation = transform.rotation;
    }

    public override void OnCharacterUpdate()
    {
        _deltaTime = Time.deltaTime;
        if (_deltaTime <= 0)
        {
            return;
        }

        base.OnCharacterUpdate();

        UpdateModules();

        _lastPosition = _capsule.position;
        RunModulePhysics();

        Vector3 moved = _capsule.position - _lastPosition;
        _velocity = moved / _deltaTime;

        PostUpdateModules();
    }

    protected virtual void UpdateModules()
    {
        foreach (var module in _modules)
        {
            module.Update();
        }
    }

    protected virtual void RunModulePhysics()
    {
        _previousModule = _activeModule;
        _activeModule = null;
        foreach (var module in _modules)
        {
            if (module.ShouldRun())
            {
                _activeModule = module;
                break;
            }
        }

        if (_previousModule != _activeModule)
        {
            if (_previousModule is not null)
            {
                _previousModule.StopPhysics();
            }

            if (_activeModule is not null)
            {
                _activeModule.StartPhysics();
            }
        }

        if (_activeModule is not null)
        {
        _activeModule.RunPhysics(out _capsule);
        _capsule.WriteValuesTo(_collider);
        }
    }

    protected virtual void PostUpdateModules()
    {
        foreach (var module in _modules)
        {
            module.PostUpdate();
        }
    }

    public IReadOnlyCollection<CharacterMovementModule> modules => _modules;
    public CharacterMovementModule activeModule => _activeModule;
    public CharacterMovementModule previousModule => _previousModule;
    public new CapsuleCollider collider => _collider;
    public float skinWidth => _skinWidth;
    public VirtualCapsule capsule => _capsule;
    public Vector3 velocity => _velocity;

    protected CharacterMovementModule[] _modules;
    
    [SerializeReference]
    protected CharacterMovementModule _activeModule;
    protected CharacterMovementModule _previousModule;
    protected CapsuleCollider _collider;
    protected float _skinWidth;
    protected VirtualCapsule _capsule;
    protected Vector3 _velocity;
    protected Vector3 _lastPosition;
    protected float _deltaTime = 0f;
}
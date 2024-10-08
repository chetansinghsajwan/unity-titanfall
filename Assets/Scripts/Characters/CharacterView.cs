﻿using System;
using UnityEngine;

[DisallowMultipleComponent]
class CharacterView : CharacterBehaviour
{
    public enum Mode
    {
        None,
        Eyes,
        FirstPerson,
        ThirdPerson
    }

    [Serializable]
    protected struct EyesData
    {
        public Transform eyes;
    }

    [Serializable]
    protected struct FirstPersonData
    {
        public Vector3 standPos;
        [Label("Stand-Crouch Transition")] public AnimationClip standCrouchTransition;
        public Vector3 crouchPos;
        public Vector3 minRotation;
        public Vector3 maxRotation;
    }

    [Serializable]
    protected struct ThirdPersonData
    {
        public bool alwaysUpdate;
        public GameObject lookAtSource;
        public Vector3 lookAtSourceOffset;
        public Vector3 acceleration;
        public Vector3 deceleration;
        public Vector3 minRotation;
        public Vector3 maxRotation;
        public float minDistanceFromLookSource;
        public float maxDistanceFromLookSource;
    }

    public CharacterView()
    {
        _stateMachine = new StateMachine<Mode>();
        _stateMachine.Add(Mode.None, null, null, null);
        _stateMachine.Add(Mode.Eyes, _EnterViewMode_Eyes, _UpdateViewMode_Eyes, _ExitViewMode_Eyes);
        _stateMachine.Add(Mode.FirstPerson, _EnterViewMode_FirstPerson, _UpdateViewMode_FirstPerson, _ExitViewMode_FirstPerson);
        _stateMachine.Add(Mode.ThirdPerson, _EnterViewMode_ThirdPerson, _UpdateViewMode_ThirdPerson, _ExitViewMode_ThirdPerson);
        _stateMachine.Switch(Mode.None);
    }

    public override void OnCharacterCreate(Character character, CharacterInitializer initializer)
    {
        base.OnCharacterCreate(character, initializer);

        _charMovement = character.charMovement;
    }

    public override void OnCharacterUpdate()
    {
        base.OnCharacterUpdate();

        _stateMachine.Update();
    }

    public void SetLookVector(Vector2 look)
    {
        _inputLookVector = look;
    }

    public void SwitchView(Mode mode)
    {
        _stateMachine.Switch(mode);
    }

    protected void _EnterViewMode_Eyes()
    {
    }

    protected void _UpdateViewMode_Eyes()
    {
        if (_camera is null || _eyesData.eyes is null)
            return;

        _camera.transform.position = _eyesData.eyes.transform.position;
        _camera.transform.rotation = _eyesData.eyes.transform.rotation;
    }
    
    protected void _ExitViewMode_Eyes()
    {
    }

    protected void _EnterViewMode_FirstPerson()
    {
    }

    protected void _UpdateViewMode_FirstPerson()
    {
        if (_camera is null)
        {
            return;
        }

        _lookVector += new Vector3(_inputLookVector.x, _inputLookVector.y, 0f);
        _lookVector.x = _lookVector.x < -180 || _lookVector.x > 180 ? -_lookVector.x : _lookVector.x;
        _lookVector.y = _lookVector.y < -180 || _lookVector.y > 180 ? -_lookVector.y : _lookVector.y;
        _lookVector.z = _lookVector.z < -180 || _lookVector.z > 180 ? -_lookVector.z : _lookVector.z;

        _lookVector.x = Math.Clamp(_lookVector.x, _firstPersonData.minRotation.x, _firstPersonData.maxRotation.x);
        _lookVector.y = Math.Clamp(_lookVector.y, _firstPersonData.minRotation.y, _firstPersonData.maxRotation.y);
        _lookVector.z = Math.Clamp(_lookVector.z, _firstPersonData.minRotation.z, _firstPersonData.maxRotation.z);

        Vector3 camPos = Vector3.zero;
        Quaternion camRot = Quaternion.identity;

        // calculate position
        camPos = _firstPersonData.standPos;
        camPos = _charMovement.capsule.ClampPositionInsideVolumeRelative(camPos);

        // calculate rotation
        camRot = Quaternion.Euler(new Vector3(_lookVector.y, _lookVector.x, _lookVector.z));

        // apply position and rotation
        _camera.transform.position = camPos;
        _camera.transform.rotation = camRot;
    }

    protected void _ExitViewMode_FirstPerson()
    {
    }

    protected void _EnterViewMode_ThirdPerson()
    {
    }

    protected void _UpdateViewMode_ThirdPerson()
    {
        // var data = _thirdPersonData;
        // if (_camera is null || data.lookAtSource is null)
        // {
        //     SwitchView(Mode.None);
        //     return;
        // }

        // Vector3 lookAt = data.lookAtSource.transform.position + data.lookAtSourceOffset;
        // Vector3 lookDir = _charCapsule.forward;
        // Vector3 camPos = _camera.transform.position;
        // Quaternion camRot = Quaternion.LookRotation(lookDir, _charCapsule.up);

        // bool hit = Physics.Raycast(lookAt, lookDir * -1, out RaycastHit hitInfo, data.maxDistanceFromLookSource);
        // if (hit)
        // {
        //     Vector3 targetCamPos = Math.Max(hitInfo.distance, data.minDistanceFromLookSource) * lookDir * -1;

        //     Vector3 lerpSpeed = Vector3.Distance(lookAt, camPos) > Vector3.Distance(lookAt, targetCamPos) ? data.deceleration : data.acceleration;
        //     if (lerpSpeed == Vector3.zero)
        //     {
        //         lerpSpeed = Vector3.one;
        //     }
        //     else
        //     {
        //         lerpSpeed = lerpSpeed * delta_time;
        //     }

        //     camPos.x = Mathf.Lerp(camPos.x, targetCamPos.x, lerpSpeed.x);
        //     camPos.y = Mathf.Lerp(camPos.y, targetCamPos.y, lerpSpeed.y);
        //     camPos.z = Mathf.Lerp(camPos.z, targetCamPos.z, lerpSpeed.z);

        //     _camera.transform.position = camPos;
        //     _camera.transform.rotation = camRot;
        // }
    }

    protected void _ExitViewMode_ThirdPerson()
    {
    }

    public new Camera camera
    {
        get => _camera;
        set => _camera = value;
    }

    public Mode mode
    {
        get => _stateMachine.currentState;
        set => _stateMachine.Switch(value);
    }

    public float turnAngle => _lookVector.x;
    public Vector2 lookVector => _lookVector;

    protected CharacterMovement _charMovement;

    [SerializeField]
    protected Camera _camera;

    [SerializeField, ReadOnly]
    protected Vector3 _lookVector;

    [Header("View Modes Data"), Space]
    [SerializeField]
    protected EyesData _eyesData;

    [SerializeField]
    protected FirstPersonData _firstPersonData;

    [SerializeField]
    protected ThirdPersonData _thirdPersonData;

    protected StateMachine<Mode> _stateMachine;
    protected Vector2 _inputLookVector;
}
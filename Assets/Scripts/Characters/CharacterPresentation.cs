using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class CharacterPresentation : CharacterBehaviour
{
    public override void OnCharacterCreate(Character character, CharacterInitializer initializer)
    {
        base.OnCharacterCreate(character, initializer);

        _charMovement = _character.charMovement;
        _animator = GetComponent<Animator>();
        _charAsset = initializer.charAsset;

        _CreateAnimGraph();
    }

    public override void OnCharacterUpdate()
    {
        base.OnCharacterUpdate();

        _UpdateAnimGraph();
    }

    public override void OnCharacterDestroy()
    {
        base.OnCharacterDestroy();

        _DestroyAnimGraph();
    }

    protected void _CreateAnimGraph()
    {
        _animGraph = PlayableGraph.Create();
        _animGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        _animOutput = AnimationPlayableOutput.Create(_animGraph, "Animation", _animator);

        _animBaseTree = new AnimationBlendTree1D(_animGraph);
        _animGroundStandTree = new AnimationBlendTree1D(_animGraph);
        _animGroundStandWalkTree = new AnimationBlendTree2DSimpleDirectional(_animGraph);
        _animGroundStandRunTree = new AnimationBlendTree2DSimpleDirectional(_animGraph);
        _animGroundCrouchTree = new AnimationBlendTree1D(_animGraph);
        _animGroundCrouchWalkTree = new AnimationBlendTree2DSimpleDirectional(_animGraph);
        _animGroundCrouchRunTree = new AnimationBlendTree2DSimpleDirectional(_animGraph);

        Vector2 center = new Vector2(0.00f, 0.00f);
        Vector2 front = new Vector2(0.00f, 1.00f);
        Vector2 frontLeft = new Vector2(-0.70f, 0.70f);
        Vector2 frontRight = new Vector2(-0.70f, 0.70f);
        Vector2 left = new Vector2(-0.10f, 0.00f);
        Vector2 right = new Vector2(0.10f, 0.00f);
        Vector2 back = new Vector2(0.00f, -1.00f);
        Vector2 backLeft = new Vector2(-0.70f, -0.70f);
        Vector2 backRight = new Vector2(0.70f, -0.70f);

        // -----------------------------------------------------------------------------------------
        // Stand Tree
        // -----------------------------------------------------------------------------------------

        _animGroundStandWalkTree.Reserve(9);
        _animGroundStandWalkTree.AddElement(_charAsset.animGroundStandIdle, center);
        _animGroundStandWalkTree.AddElement(_charAsset.animGroundStandWalkForward, front);
        _animGroundStandWalkTree.AddElement(_charAsset.animGroundStandWalkForwardLeft, frontLeft);
        _animGroundStandWalkTree.AddElement(_charAsset.animGroundStandWalkForwardRight, frontRight);
        _animGroundStandWalkTree.AddElement(_charAsset.animGroundStandWalkLeft, left);
        _animGroundStandWalkTree.AddElement(_charAsset.animGroundStandWalkRight, right);
        _animGroundStandWalkTree.AddElement(_charAsset.animGroundStandWalkBackward, back);
        _animGroundStandWalkTree.AddElement(_charAsset.animGroundStandWalkBackwardLeft, backLeft);
        _animGroundStandWalkTree.AddElement(_charAsset.animGroundStandWalkBackwardRight, backRight);
        _animGroundStandWalkTree.EnableFootIk();
        _animGroundStandWalkTree.BuildGraph(true);
        _animGroundStandWalkTree.UpdateGraph(true);

        _animGroundStandRunTree.Reserve(9);
        _animGroundStandRunTree.AddElement(_charAsset.animGroundStandIdle, center * 2f);
        _animGroundStandRunTree.AddElement(_charAsset.animGroundStandRunForward, front * 2f);
        _animGroundStandRunTree.AddElement(_charAsset.animGroundStandRunForwardLeft, frontLeft * 2f);
        _animGroundStandRunTree.AddElement(_charAsset.animGroundStandRunForwardRight, frontRight * 2f);
        _animGroundStandRunTree.AddElement(_charAsset.animGroundStandRunLeft, left * 2f);
        _animGroundStandRunTree.AddElement(_charAsset.animGroundStandRunRight, right * 2f);
        _animGroundStandRunTree.AddElement(_charAsset.animGroundStandRunBackward, back * 2f);
        _animGroundStandRunTree.AddElement(_charAsset.animGroundStandRunBackwardLeft, backLeft * 2f);
        _animGroundStandRunTree.AddElement(_charAsset.animGroundStandRunBackwardRight, backRight * 2f);
        _animGroundStandRunTree.EnableFootIk();
        _animGroundStandRunTree.BuildGraph(true);
        _animGroundStandRunTree.UpdateGraph(true);

        _animGroundStandTree.Reserve(4);
        _animGroundStandTree.AddElement(_charAsset.animGroundStandIdle, 0f);
        _animGroundStandTree.AddElement(_animGroundStandWalkTree, 1f);
        _animGroundStandTree.AddElement(_animGroundStandRunTree, 2f);
        _animGroundStandTree.AddElement(_charAsset.animGroundStandSprintForward, 3f);
        _animGroundStandTree.EnableFootIk();
        _animGroundStandTree.BuildGraph(true);
        _animGroundStandTree.UpdateGraph(true);

        // -----------------------------------------------------------------------------------------
        // Crouch Tree
        // -----------------------------------------------------------------------------------------

        _animGroundCrouchWalkTree.Reserve(9);
        _animGroundCrouchWalkTree.AddElement(_charAsset.animGroundCrouchIdle, center);
        _animGroundCrouchWalkTree.AddElement(_charAsset.animGroundCrouchWalkForward, front);
        _animGroundCrouchWalkTree.AddElement(_charAsset.animGroundCrouchWalkForwardLeft, frontLeft);
        _animGroundCrouchWalkTree.AddElement(_charAsset.animGroundCrouchWalkForwardRight, frontRight);
        _animGroundCrouchWalkTree.AddElement(_charAsset.animGroundCrouchWalkLeft, left);
        _animGroundCrouchWalkTree.AddElement(_charAsset.animGroundCrouchWalkRight, right);
        _animGroundCrouchWalkTree.AddElement(_charAsset.animGroundCrouchWalkBackward, back);
        _animGroundCrouchWalkTree.AddElement(_charAsset.animGroundCrouchWalkBackwardLeft, backLeft);
        _animGroundCrouchWalkTree.AddElement(_charAsset.animGroundCrouchWalkBackwardRight, backRight);
        _animGroundCrouchWalkTree.EnableFootIk();
        _animGroundCrouchWalkTree.BuildGraph(true);
        _animGroundCrouchWalkTree.UpdateGraph(true);

        _animGroundCrouchRunTree.Reserve(9);
        _animGroundCrouchRunTree.AddElement(_charAsset.animGroundCrouchIdle, center * 2f);
        _animGroundCrouchRunTree.AddElement(_charAsset.animGroundCrouchRunForward, front * 2f);
        _animGroundCrouchRunTree.AddElement(_charAsset.animGroundCrouchRunForwardLeft, frontLeft * 2f);
        _animGroundCrouchRunTree.AddElement(_charAsset.animGroundCrouchRunForwardRight, frontRight * 2f);
        _animGroundCrouchRunTree.AddElement(_charAsset.animGroundCrouchRunLeft, left * 2f);
        _animGroundCrouchRunTree.AddElement(_charAsset.animGroundCrouchRunRight, right * 2f);
        _animGroundCrouchRunTree.AddElement(_charAsset.animGroundCrouchRunBackward, back * 2f);
        _animGroundCrouchRunTree.AddElement(_charAsset.animGroundCrouchRunBackwardLeft, backLeft * 2f);
        _animGroundCrouchRunTree.AddElement(_charAsset.animGroundCrouchRunBackwardRight, backRight * 2f);
        _animGroundCrouchRunTree.EnableFootIk();
        _animGroundCrouchRunTree.BuildGraph(true);
        _animGroundCrouchRunTree.UpdateGraph(true);

        _animGroundCrouchTree.Reserve(3);
        _animGroundCrouchTree.AddElement(_charAsset.animGroundCrouchIdle, 0f);
        _animGroundCrouchTree.AddElement(_animGroundCrouchWalkTree, 1f);
        _animGroundCrouchTree.AddElement(_animGroundCrouchRunTree, 2f);
        _animGroundCrouchTree.EnableFootIk();
        _animGroundCrouchTree.BuildGraph(true);
        _animGroundCrouchTree.UpdateGraph(true);

        // -----------------------------------------------------------------------------------------
        // Base Tree
        // -----------------------------------------------------------------------------------------

        _animBaseTree.Reserve(2);
        _animBaseTree.AddElement(_animGroundStandTree, 0f);
        _animBaseTree.AddElement(_animGroundCrouchTree, 1f);
        _animBaseTree.EnableFootIk();
        _animBaseTree.BuildGraph(true);
        _animBaseTree.UpdateGraph(true);

        _animGraph.Play();
    }

    protected void _UpdateAnimGraph()
    {
        // float walkSpeed = _standWalkSpeed;
        // float runSpeed = _standRunSpeed;
        // float sprintSpeed = _standSprintSpeed;

        // Vector2 velocity = new Vector2(_velocity.x, _velocity.z);

        float walkSpeed = 2;
        float runSpeed = 4;
        float sprintSpeed = 6;

        Vector2 velocity = new Vector2(0, 0);


        float speed = velocity.magnitude;
        speed = MathF.Round(speed, 2);

        if (speed <= walkSpeed)
        {
            speed = speed / walkSpeed;
        }
        else if (speed <= runSpeed)
        {
            speed = 1f + (speed - walkSpeed) / (runSpeed - walkSpeed);
        }
        else
        {
            speed = 2f + (speed - runSpeed) / (sprintSpeed - runSpeed);
        }

        velocity = velocity.normalized * Mathf.Clamp(speed, 0f, 2f);

        _animGroundStandWalkTree.SetBlendPosition(velocity);
        _animGroundStandRunTree.SetBlendPosition(velocity);
        _animGroundCrouchWalkTree.SetBlendPosition(velocity);
        _animGroundCrouchRunTree.SetBlendPosition(velocity);

        _animGroundStandTree.SetBlendPosition(speed);
        _animGroundCrouchTree.SetBlendPosition(speed);
        _animBaseTree.SetBlendPosition(0f);
    }

    protected void _DestroyAnimGraph()
    {
        if (_animGraph.IsValid())
        {
            _animGraph.Destroy();
        }
    }

    protected CharacterAsset _charAsset;
    protected CharacterMovement _charMovement;

    protected Animator _animator;
    protected AnimationPlayableOutput _animOutput;
    protected PlayableGraph _animGraph;
    protected AnimationBlendTree1D _animBaseTree;
    protected AnimationBlendTree1D _animGroundStandTree;
    protected AnimationBlendTree2D _animGroundStandWalkTree;
    protected AnimationBlendTree2D _animGroundStandRunTree;
    protected AnimationBlendTree1D _animGroundCrouchTree;
    protected AnimationBlendTree2D _animGroundCrouchWalkTree;
    protected AnimationBlendTree2D _animGroundCrouchRunTree;
}

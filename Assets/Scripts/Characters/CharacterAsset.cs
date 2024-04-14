using System;
using UnityEngine;

[CreateAssetMenu(menuName = MENU_PATH + MENU_NAME, fileName = FILE_NAME)]
public partial class CharacterAsset : DataAsset
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

    [SerializeField]
    public string characterName;

    [SerializeField]
    public GameObject tppBody;

    [SerializeField]
    public GameObject fppBody;

    [SerializeField]
    public GameObject tppPrefab;

    [SerializeField]
    public GameObject fppPrefab;

    [SerializeField, Min(0)]
    public float characterMass;

    [Space, Header("CHARACTER MOVEMENT")]

    [SerializeField]
    public CharacterMovementGroundModuleSource groundModuleSource;

    [SerializeField]
    public CharacterMovementAirModuleSource airModuleSource;

    //////////////////////////////////////////////////////////////////
    /// Animations | BEGIN

    [Space, Header("CHARACTER ANIMATION")]

    [SerializeField]
    public Avatar avatar;

    [SerializeField]
    public Avatar AvatarMaskUpperBody;

    [SerializeField]
    public Avatar AvatarMaskLowerBody;
}
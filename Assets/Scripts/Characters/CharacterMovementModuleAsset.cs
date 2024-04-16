using UnityEngine;

abstract class CharacterMovementModuleAsset : ScriptableObject
{
    public const string MENU_PATH = "Character/Movement/";

    public abstract CharacterMovementModule GetModule();
}
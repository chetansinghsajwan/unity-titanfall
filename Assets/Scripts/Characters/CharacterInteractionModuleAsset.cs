using UnityEngine;

abstract class CharacterInteractionModuleAsset: ScriptableObject
{
    public const string MENU_PATH = "Character/Interaction Modules/";

    public abstract bool CanHandle(Interactable interactable);
    public abstract CharacterInteractionModule CreateModuleFor(Interactable interactable);
}

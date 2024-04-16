using UnityEngine;

[CreateAssetMenu(menuName = MENU_PATH + MENU_NAME, fileName = FILE_NAME)]
class CharacterInteractionModuleForReloadableWeaponAsset: CharacterInteractionModuleAsset
{
    public const string MENU_NAME = "ReloadableWeapon";
    public const string FILE_NAME = "CharacterInteractionModule-ReloadableWeapon";

    public override bool CanHandle(Interactable interactable)
    {
        if (interactable is ReloadableWeapon)
            return true;

        return false;
    }

    public override CharacterInteractionModule CreateModuleFor(Interactable interactable)
    {
        ReloadableWeapon weapon = interactable as ReloadableWeapon;
        if (weapon is not null)
            return new CharacterInteractionModuleForReloadableWeapon(weapon);

        return null;
    }
}

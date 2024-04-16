using UnityEngine;

[CreateAssetMenu(menuName = MENU_PATH + "ReloadableWeapon", fileName = "CharacterInteractionModule-ReloadableWeapon")]
class CharacterInteractionModuleForReloadableWeaponAsset: CharacterInteractionModuleAsset
{
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

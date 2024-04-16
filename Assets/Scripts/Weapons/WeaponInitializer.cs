using UnityEngine;

[DisallowMultipleComponent]
class WeaponInitializer : MonoBehaviour
{
    public bool destroyAfterUse = false;
    public WeaponAsset weaponAsset = null;
    public WeaponBulletAsset bulletDataAsset = null;
}
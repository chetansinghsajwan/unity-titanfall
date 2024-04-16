using UnityEngine;

[DisallowMultipleComponent]
class WeaponInitializer : MonoBehaviour
{
    public bool destroyAfterUse;
    public WeaponAsset source;
    public WeaponBulletAsset bulletDataSource;

    public WeaponInitializer()
    {
        destroyAfterUse = false;
        source = null;
    }
}
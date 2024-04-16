using UnityEngine;

class WeaponBullet : BulletProjectile
{
}

sealed class WeaponBulletLight : WeaponBullet { }
sealed class WeaponBulletHeavy : WeaponBullet { }
sealed class WeaponBulletSniper : WeaponBullet { }
sealed class WeaponBulletShotgun : WeaponBullet { }
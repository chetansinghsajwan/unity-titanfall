using System;
using UnityEngine.AddressableAssets;
using GameFramework.LevelManagement;

[Serializable]
class AssetReferenceLevel : AssetReferenceT<LevelAsset>
{
    public AssetReferenceLevel(string guid) : base(guid) { }
}
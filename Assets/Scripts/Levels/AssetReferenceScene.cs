using System;
using UnityEditor;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR

[Serializable]
class AssetReferenceScene : AssetReferenceT<SceneAsset>
{
    public AssetReferenceScene(string guid) : base(guid) { }
}

#else

[Serializable]
class AssetReferenceScene : AssetReference { }

#endif
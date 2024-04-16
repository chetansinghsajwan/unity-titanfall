using System;
using UnityEngine;

class CharacterAssetRegistry : AssetRegistry<CharacterAsset>
{
    public static readonly CharacterAssetRegistry Instance = new CharacterAssetRegistry();
}
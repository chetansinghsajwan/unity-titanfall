using UnityEngine;

[DisallowMultipleComponent]
class CharacterInitializer : MonoBehaviour
{
    public CharacterAsset source;
    public CharacterAsset charAsset => source;
    public bool destroyOnUse;
}
using UnityEngine;

[DisallowMultipleComponent]
public class CharacterInitializer : MonoBehaviour
{
    public CharacterAsset source;
    public CharacterAsset charAsset => source;
    public bool destroyOnUse;
}
using UnityEngine;
using GameFramework.LevelManagement;
using GameFramework;

class EditorLevelInitializer : MonoBehaviour
{
    static bool isInitDone = false;

    private void Awake()
    {
        if (isInitDone)
            return;

        isInitDone = true;

        Debug.Log("Initializing PlayerManagerSystem...");
        PlayerManager.System = new PlayerManagerSystem();

        Debug.Log($"Loading level{_level.name}...");
        _level.PerformLoad();
    }

    [SerializeField]
    private LevelAsset _level;
}
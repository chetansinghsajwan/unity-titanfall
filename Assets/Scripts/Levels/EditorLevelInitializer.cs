using UnityEngine;
using System.Linq;

class EditorLevelInitializer : MonoBehaviour
{
    public void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        PlayerManager.System = new PlayerManagerSystem();

        Transform spawnPoint = sceneObject.playerSpawnPoints.
            FirstOrDefault(trans => trans is not null);

        if (spawnPoint == null)
        {
            return;
        }

        var localPlayer = PlayerManager.CreateLocalPlayer();
        if (localPlayer == null)
        {
            Debug.LogError("could not create local player.");
            return;
        }

        var charFactory = charAsset.factory;
        charFactory.SetMaxReserve(5);
        charFactory.Reserve(4);

        Character character = charFactory.Create(spawnPoint.position, spawnPoint.rotation);

        Debug.Log("created character.");
        localPlayer.Possess(character);
    }

    public CharacterAsset charAsset;
    public SceneObject sceneObject;
}
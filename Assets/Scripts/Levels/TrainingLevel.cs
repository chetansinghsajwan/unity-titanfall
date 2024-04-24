using System.Linq;
using UnityEngine;
using GameFramework.Logging;
using GameFramework.LevelManagement;
using GameFramework;

[CreateAssetMenu(menuName = MENU_PATH + "TrainingLevel", fileName = "TrainingLevel")]
class TrainingLevel : BasicLevelAsset
{
    public override LevelAsyncOperation PerformLoad()
    {
        LevelAsyncOperation levelOp = base.PerformLoad();
        var op = new AsyncOperationSource();
        levelOp.AddOperation(op);

        _PerformLoad(op);
        return levelOp;
    }

    private async void _PerformLoad(AsyncOperationSource op)
    {
        IGameLogger logger = GameLog.System.CreateLogger("TrainingLevel");

        var sceneObject = SceneManager.System.FindSceneObjectFor(_scenes[0]) as SceneObject;
        Transform spawnPoint = sceneObject.playerSpawnPoints.
            FirstOrDefault(trans => trans is not null);

        if (spawnPoint == null)
        {
            return;
        }

        var localPlayer = PlayerManager.CreateLocalPlayer();
        if (localPlayer == null)
        {
            logger.Error("Could not create Local Player");
            return;
        }

        var charFactory = charAsset.factory;
        charFactory.SetMaxReserve(5);
        charFactory.Reserve(4);

        Character character = charFactory.Create(spawnPoint.position, spawnPoint.rotation);

        logger.Information("Created Character");
        localPlayer.Possess(character);

        op.SetCompleted();
    }

    public CharacterAsset charAsset;
}
using UnityEngine;

[CreateAssetMenu(menuName = MENU_PATH + "Air Module")]
class CharacterMovementAirModuleAsset : CharacterMovementModuleAsset
{
    public float gravityAcceleration;
    public float gravityMaxSpeed;
    public float minMoveDistance;
    public float moveSpeed;
    public float moveAcceleration;
    public float jumpPower;
    public uint maxJumpCount;

    public override CharacterMovementModule GetModule()
    {
        return new CharacterMovementAirModule(null);
    }
}
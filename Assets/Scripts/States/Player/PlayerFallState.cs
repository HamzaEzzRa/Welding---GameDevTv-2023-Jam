using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
    {
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Fall State...");
    }

    protected override void UpdateState()
    {

    }

    protected override void FixedUpdateState()
    {

    }

    protected override void ExitState()
    {
        //Debug.Log("Exiting Fall State...");
    }
}

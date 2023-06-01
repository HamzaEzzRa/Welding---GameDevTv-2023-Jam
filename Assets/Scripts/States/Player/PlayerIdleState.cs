using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
    {
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Idle State...");
    }

    protected override void UpdateState()
    {

    }

    protected override void FixedUpdateState()
    {
        // Set velocity to 0 for snappy movement
        parentFactory.Context.PlayerRigidbody.velocity = Vector3.zero;
    }

    protected override void ExitState()
    {
        //Debug.Log("Exiting Idle State...");
    }
}

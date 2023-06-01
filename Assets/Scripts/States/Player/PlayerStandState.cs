using UnityEngine;

public class PlayerStandState : PlayerBaseState
{
    public PlayerStandState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
    {
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Stand State...");
    }

    protected override void UpdateState()
    {
        if (!parentFactory.Context.IsMoving)
        {
            SetChildState(parentFactory.GetIdleState());
        }
        else if (!parentFactory.Context.IsRunning)
        {
            SetChildState(parentFactory.GetWalkState());
        }
        else
        {
            SetChildState(parentFactory.GetRunState());
        }
    }

    protected override void FixedUpdateState()
    {
    }

    protected override void ExitState()
    {
        //Debug.Log("Exiting Stand State...");
    }
}

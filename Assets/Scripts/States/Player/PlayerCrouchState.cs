using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    public PlayerCrouchState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
    {
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Crouch State...");
    }

    protected override void UpdateState()
    {
        if (!parentFactory.Context.IsMoving)
        {
            SetChildState(parentFactory.GetIdleState());
        }
        else
        {
            SetChildState(parentFactory.GetWalkState());
        }
    }

    protected override void FixedUpdateState()
    {
    }

    protected override void ExitState()
    {
        //Debug.Log("Exiting Crouch State...");
    }
}

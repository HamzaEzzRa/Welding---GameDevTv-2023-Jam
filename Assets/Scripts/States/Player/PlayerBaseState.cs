public abstract class PlayerBaseState
{
    protected PlayerStateFactory parentFactory;
    protected PlayerBaseState currentChildState;

    public PlayerBaseState(PlayerStateFactory playerStateFactory)
    {
        parentFactory = playerStateFactory;
    }

    public abstract void EnterState();

    protected abstract void UpdateState();

    protected abstract void FixedUpdateState();

    protected abstract void ExitState();

    public void UpdateChildStates()
    {
        UpdateState();
        if (currentChildState != null)
        {
            currentChildState.UpdateChildStates();
        }
    }

    public void FixedUpdateChildStates()
    {
        FixedUpdateState();
        if (currentChildState != null)
        {
            currentChildState.FixedUpdateChildStates();
        }
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        ExitState();
        if (currentChildState != null)
        {
            currentChildState.ExitState();
            currentChildState = null;
        }

        parentFactory.Context.ChangeState(newState);
    }

    protected void SetChildState(PlayerBaseState newChildState)
    {
        if (currentChildState != null)
        {
            if (currentChildState == newChildState)
            {
                return;
            }

            currentChildState.ExitState();
        }

        currentChildState = newChildState;
        currentChildState.EnterState();
    }
}

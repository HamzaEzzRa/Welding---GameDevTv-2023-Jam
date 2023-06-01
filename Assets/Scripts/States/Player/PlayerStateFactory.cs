public class PlayerStateFactory
{
    public FPController Context { get; private set; }

    private PlayerGroundedState cachedGroundedState;
    private PlayerCrouchState cachedCrouchState;
    private PlayerStandState cachedStandState;
    private PlayerIdleState cachedIdleState;
    private PlayerWalkState cachedWalkState;
    private PlayerRunState cachedRunState;

    private PlayerAirborneState cachedAirborneState;
    private PlayerJumpState cachedJumpState;
    private PlayerFallState cachedFallState;

    public PlayerStateFactory(FPController currentContext)
    {
        Context = currentContext;
    }

    public PlayerBaseState GetGroundedState()
    {
        if (cachedGroundedState == null)
        {
            cachedGroundedState = new PlayerGroundedState(this);
        }
        return cachedGroundedState;
    }

    public PlayerBaseState GetAirborneState()
    {
        if (cachedAirborneState == null)
        {
            cachedAirborneState = new PlayerAirborneState(this);
        }
        return cachedAirborneState;
    }
    
    public PlayerBaseState GetCrouchState()
    {
        if (cachedCrouchState == null)
        {
            cachedCrouchState = new PlayerCrouchState(this);
        }
        return cachedCrouchState;
    }

    public PlayerBaseState GetStandState()
    {
        if (cachedStandState == null)
        {
            cachedStandState = new PlayerStandState(this);
        }
        return cachedStandState;
    }

    public PlayerBaseState GetIdleState()
    {
        if (cachedIdleState == null)
        {
            cachedIdleState = new PlayerIdleState(this);
        }
        return cachedIdleState;
    }

    public PlayerBaseState GetWalkState()
    {
        if (cachedWalkState == null)
        {
            cachedWalkState = new PlayerWalkState(this);
        }
        return cachedWalkState;
    }

    public PlayerBaseState GetRunState()
    {
        if (cachedRunState == null)
        {
            cachedRunState = new PlayerRunState(this);
        }
        return cachedRunState;
    }
    
    public PlayerBaseState GetJumpState()
    {
        if (cachedJumpState == null)
        {
            cachedJumpState = new PlayerJumpState(this);
        }
        return cachedJumpState;
    }

    public PlayerBaseState GetFallState()
    {
        if (cachedFallState == null)
        {
            cachedFallState = new PlayerFallState(this);
        }
        return cachedFallState;
    }
}

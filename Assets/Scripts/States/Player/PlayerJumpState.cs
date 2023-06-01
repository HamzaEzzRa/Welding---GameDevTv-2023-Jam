using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
    {
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Jump State...");

        if (parentFactory.Context.IsCrouching)
        {
            parentFactory.Context.ToggleCrouch();
        }

        float jumpSpeed = Mathf.Sqrt(-2.0f * Physics.gravity.y * parentFactory.Context.PlayerSettings.JumpHeight);
        Vector3 newVelocity = parentFactory.Context.PlayerRigidbody.velocity * parentFactory.Context.PlayerSettings.AirSpeedFactor;
        newVelocity.y = jumpSpeed;
        parentFactory.Context.PlayerRigidbody.velocity = newVelocity;

        parentFactory.Context.JumpStarted = false;
        parentFactory.Context.RemainingCoyoteTime = 0f;

        //parentFactory.Context.IsRunning = false;

        AudioManager.Instance.Play("Player_Jump");
    }

    protected override void UpdateState()
    {

    }

    protected override void FixedUpdateState()
    {

    }

    protected override void ExitState()
    {
        //Debug.Log("Exiting Jump State...");
    }
}

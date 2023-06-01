using UnityEngine;

public class PlayerAirborneState : PlayerBaseState
{
    public PlayerAirborneState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
    {
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Airborne State...");

        if (parentFactory.Context.JumpStarted)
        {
            SetChildState(parentFactory.GetJumpState());
        }
    }

    protected override void UpdateState()
    {
        parentFactory.Context.LastAirTime += Time.deltaTime;

        if (!parentFactory.Context.IsJumping)
        {
            if (!parentFactory.Context.IsGrounded)
            {
                if (parentFactory.Context.RemainingCoyoteTime <= 0f)
                {
                    SetChildState(parentFactory.GetFallState());
                }
                else if (parentFactory.Context.JumpStarted)
                {
                    SetChildState(parentFactory.GetJumpState());
                }
            }
            else
            {
                SwitchState(parentFactory.GetGroundedState());
            }
        }
    }

    protected override void FixedUpdateState()
    {
        parentFactory.Context.PlayerRigidbody.velocity += parentFactory.Context.PlayerSettings.PlayerGravity * Time.fixedDeltaTime;

        if (parentFactory.Context.CanMove)
        {
            Transform transform = parentFactory.Context.transform;
            Vector2 moveInput = parentFactory.Context.MoveInput.normalized;
            Vector3 movement = (transform.forward * moveInput.y + transform.right * moveInput.x) * parentFactory.Context.CurrentMaxSpeed;

            // Handling steps and slopes
            if (!HandleSteps(parentFactory.Context, ref movement))
            {
                // if not a step, check if slope
                if (parentFactory.Context.IsGrounded)
                {
                    movement = Vector3.ProjectOnPlane(movement, parentFactory.Context.GroundHit.normal);

                    float angle = Vector3.Angle(Vector3.up, parentFactory.Context.GroundHit.normal);
                    if (movement.y > 0f && angle > parentFactory.Context.PlayerSettings.SlopeLimit)
                    {
                        Vector3 stopVelocity = transform.InverseTransformVector(movement);
                        stopVelocity.z = 0f;
                        parentFactory.Context.PlayerRigidbody.velocity = stopVelocity;
                        return;
                    }
                }
            }

            Vector3 velocity = Vector3.MoveTowards(parentFactory.Context.PlayerRigidbody.velocity, movement, parentFactory.Context.PlayerSettings.SpeedBlend);
            velocity.y = parentFactory.Context.PlayerRigidbody.velocity.y;
            parentFactory.Context.PlayerRigidbody.velocity = velocity;
        }
    }

    private bool HandleSteps(FPController controller, ref Vector3 movement)
    {
        Vector3 lookAheadStart = controller.PlayerRigidbody.position + Vector3.up * controller.PlayerSettings.StepHeightRange.Min;
        Vector3 lookAheadDirection = movement.normalized;
        float lookAheadDistance = controller.PlayerSettings.PlayerRadius + controller.PlayerSettings.StepLookAhead;

        Debug.DrawRay(lookAheadStart, lookAheadDirection * lookAheadDistance, Color.red, 0.1f);

        // Check for potential step
        if (Physics.Raycast(lookAheadStart, lookAheadDirection, out RaycastHit stairHit, lookAheadDistance, controller.PlayerSettings.GroundMask, QueryTriggerInteraction.Ignore))
        {
            // Check if stair is clear to step on
            lookAheadStart = stairHit.point;
            lookAheadStart.y += controller.PlayerSettings.StepHeightRange.Max;

            Debug.DrawRay(lookAheadStart, lookAheadDirection * lookAheadDistance, Color.green, 0.1f);

            if (!Physics.Raycast(lookAheadStart, lookAheadDirection, controller.PlayerSettings.PlayerRadius, controller.PlayerSettings.GroundMask, QueryTriggerInteraction.Ignore))
            {
                lookAheadStart += lookAheadDirection * controller.PlayerSettings.PlayerRadius;

                Debug.DrawRay(lookAheadStart, Vector3.down * parentFactory.Context.PlayerSettings.StepHeightRange.Max, Color.blue, 0.1f);

                // Check the surface of the step
                if (Physics.Raycast(lookAheadStart, Vector3.down, out RaycastHit stairSurfaceHit, controller.PlayerSettings.StepHeightRange.Max, controller.PlayerSettings.GroundMask, QueryTriggerInteraction.Ignore))
                {
                    float stepHeight = stairSurfaceHit.point.y - controller.PlayerRigidbody.position.y;

                    // Check if angle is not too step
                    if (Vector3.Angle(Vector3.up, stairSurfaceHit.normal) <= controller.PlayerSettings.SlopeLimit)
                    {
                        controller.IsRunning = false;
                        controller.PlayerRigidbody.position += controller.PlayerSettings.StepInstant * Mathf.Pow(stepHeight, 2) * Vector3.up;

                        controller.IsGrounded = false;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    protected override void ExitState()
    {
        //Debug.Log("Exiting Airborne State...");
    }
}

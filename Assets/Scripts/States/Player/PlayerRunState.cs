using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
    {
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Run State...");
    }

    protected override void UpdateState()
    {
        AudioManager.Instance.SetSoundRefractoryPeriod("Player_Step", parentFactory.Context.PlayerSettings.UnscaledRunSoundInterval / parentFactory.Context.CurrentMaxSpeed);
        AudioManager.Instance.Play("Player_Step");
    }

    protected override void FixedUpdateState()
    {

    }

    protected override void ExitState()
    {
        //Debug.Log("Exiting Run State...");
    }
}

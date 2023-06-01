using UnityEngine;

public class PlayerWalkState: PlayerBaseState
{
    public PlayerWalkState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
    {
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Walk State...");
    }

    protected override void UpdateState()
    {
        AudioManager.Instance.SetSoundRefractoryPeriod("Player_Step", parentFactory.Context.PlayerSettings.UnscaledWalkSoundInterval / parentFactory.Context.CurrentMaxSpeed);
        AudioManager.Instance.Play("Player_Step");
    }

    protected override void FixedUpdateState()
    {

    }

    protected override void ExitState()
    {
        //Debug.Log("Exiting Walk State...");
    }
}

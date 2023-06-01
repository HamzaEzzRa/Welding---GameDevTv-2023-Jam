using UnityEngine;

public class SoundTriggerArea : TriggerArea
{
    [SerializeField] private string soundName;

    protected override void HandleTriggerEnter(Collider other)
    {
        AudioManager.Instance?.Play(soundName);
    }

    protected override void HandleTriggerExit(Collider other)
    {

    }
}

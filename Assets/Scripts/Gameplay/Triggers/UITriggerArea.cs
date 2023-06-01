using UnityEngine;

public class UITriggerArea : TriggerArea
{
    [SerializeField] private UIFader fader;

    protected override void HandleTriggerEnter(Collider other)
    {
        fader.gameObject.SetActive(true);
        fader.FadeIn();
    }

    protected override void HandleTriggerExit(Collider other)
    {

    }
}

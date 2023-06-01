using UnityEngine;
using UnityEngine.Playables;

public class TimelinePlayer : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;

    public void Play()
    {
        UIManager.Instance?.SetCenterSpriteToNone();
        InputManager.Instance.DisableInGameInput();
        playableDirector.Play();
    }
}

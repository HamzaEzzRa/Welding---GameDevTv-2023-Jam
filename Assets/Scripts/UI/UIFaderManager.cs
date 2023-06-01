using UnityEngine;

public class UIFaderManager : MonoBehaviour
{
    [SerializeField] private UIFader[] orderedFaders;

    private int currentFaderIdx;

    public static UIFaderManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (UIFader fader in orderedFaders)
        {
            if (fader != null)
            {
                break;
            }
            currentFaderIdx++;
        }
    }

    public void FadeOutSequence(int index)
    {
        if (index != currentFaderIdx || orderedFaders[currentFaderIdx] == null || !orderedFaders[currentFaderIdx].isActiveAndEnabled)
        {
            return;
        }

        orderedFaders[currentFaderIdx].FadeOut();
        currentFaderIdx++;
    }
}

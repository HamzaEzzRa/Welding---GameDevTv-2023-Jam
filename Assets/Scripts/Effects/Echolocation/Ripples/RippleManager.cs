using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class RippleManager : MonoBehaviour
{
    public const int MAX_RIPPLE_AMOUNT = 50;

    public static RippleManager Instance { get; private set; }

    [SerializeField] private PostProcessVolume volume;

    private PostProcessEcholocation postProcessEcholocation;

    private RippleDataArray rippleDataArray = new RippleDataArray();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (volume.sharedProfile.TryGetSettings(out postProcessEcholocation))
        {
            postProcessEcholocation.rippleDataArray.Override(rippleDataArray);
        }
        else
        {
            Debug.LogWarning("Echolocation component not found...");
        }
    }

    public void AddRippleData(RippleData rippleData)
    {
        rippleDataArray.AddData(rippleData);
        postProcessEcholocation.rippleCount.Override(rippleDataArray.Count);
    }

    public void RemoveRippleData(RippleData rippleData)
    {
        rippleDataArray.RemoveData(rippleData);
        postProcessEcholocation.rippleCount.Override(rippleDataArray.Count);
    }
}

using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool multipleInteractions;
    
    [SerializeField] private UnityEvent OnInteraction;
    [SerializeField] private UnityEvent OnEnable;
    [SerializeField] private UnityEvent OnDisable;

    private bool isEnabled;

    public void Interact()
    {
        isEnabled = !isEnabled;

        OnInteraction?.Invoke();

        if (isEnabled)
        {
            OnEnable?.Invoke();
        }
        else
        {
            OnDisable?.Invoke();
        }

        if (!multipleInteractions)
        {
            Destroy(this);
        }
    }
}

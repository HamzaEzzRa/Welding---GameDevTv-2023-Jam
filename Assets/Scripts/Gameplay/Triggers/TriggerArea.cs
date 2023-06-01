using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class TriggerArea : MonoBehaviour
{
    [SerializeField] protected LayerMask triggerLayers;
    [SerializeField] protected bool destroyOnTriggerExit = true;

    public Collider Collider
    {
        get
        {
            if (cachedCollider == null)
            {
                cachedCollider = GetComponent<Collider>();
            }
            return cachedCollider;
        }
    }

    protected Collider cachedCollider;

    protected abstract void HandleTriggerEnter(Collider other);
    protected abstract void HandleTriggerExit(Collider other);

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & triggerLayers) == 0)
        {
            return;
        }

        HandleTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleTriggerExit(other);

        if (destroyOnTriggerExit)
        {
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        Collider.isTrigger = true; // Just in case I forget to set it in the inspector
    }
}

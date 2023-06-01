using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Image screenCenterImage;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite interactSprite;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetCursorLockState(CursorLockMode lockMode)
    {
        Cursor.lockState = lockMode;
        SetCenterSpriteToDefault();
    }

    public void SetCenterSpriteToNone()
    {
        if (screenCenterImage != null)
        {
            screenCenterImage.sprite = null;
        }
    }

    public void SetCenterSpriteToDefault()
    {
        if (screenCenterImage != null)
        {
            screenCenterImage.sprite = defaultSprite;
            screenCenterImage.SetNativeSize();
        }
    }

    public void SetCenterSpriteToInteract()
    {
        if (screenCenterImage != null)
        {
            screenCenterImage.sprite = interactSprite;
            screenCenterImage.SetNativeSize();
        }
    }
}

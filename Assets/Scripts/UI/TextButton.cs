using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    public bool IsEnabled => isEnabled;
    public bool ResetOnClick => resetOnClick;

    [SerializeField] private TextMeshProUGUI textMesh;

    [SerializeField, ColorUsage(true, true)] private Color
        defaultColor, hoverColor, disabledColor;

    [SerializeField] private bool affectedByMouse = true;
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private bool resetOnClick = true;

    [SerializeField] private UnityEvent buttonEvent;

    public void Enable()
    {
        isEnabled = true;
        textMesh.color = defaultColor;
    }

    public void ButtonEnter()
    {
        if (!isEnabled)
        {
            return;
        }

        textMesh.color = hoverColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (affectedByMouse)
        {
            ButtonEnter();
            GameEvents.TextButtonPointerEnterInvoke(this);
        }
    }

    public void ButtonDown()
    {
        if (!isEnabled)
        {
            return;
        }

        if (buttonEvent != null)
        {
            buttonEvent.Invoke();
        }

        if (resetOnClick)
        {
            textMesh.color = defaultColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (affectedByMouse)
        {
            ButtonDown();
        }
    }

    public void ButtonExit()
    {
        if (!isEnabled)
        {
            return;
        }

        textMesh.color = defaultColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (affectedByMouse)
        {
            ButtonExit();
        }
    }

    private void OnValidate()
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        textMesh.color = isEnabled ? defaultColor : disabledColor;
        textMesh.SetText(gameObject.name);
    }
}

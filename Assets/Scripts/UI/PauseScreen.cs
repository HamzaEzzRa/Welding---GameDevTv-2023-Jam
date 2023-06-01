using UnityEngine;
using UnityEngine.InputSystem;

using System.Collections.Generic;

public class PauseScreen : MonoBehaviour
{
    public bool ActiveControls
    {
        get => activeControls;
        set
        {
            activeControls = value;
            if (value)
            {
                if (pause != null)
                {
                    pause.Enable();
                    pause.performed += TogglePause;
                }

                if (select != null)
                {
                    select.Enable();
                    select.performed += Select;
                }

                if (confirm != null)
                {
                    confirm.Enable();
                    confirm.performed += Confirm;
                }
            }
            else
            {
                if (pause != null)
                {
                    pause.Disable();
                    pause.performed -= TogglePause;
                }

                if (select != null)
                {
                    select.Disable();
                    select.performed -= Select;
                }

                if (confirm != null)
                {
                    confirm.Disable();
                    confirm.performed -= Confirm;
                }
            }
        }
    }

    private InputAction pause, select, confirm;

    private Transform[] childrenTransforms;
    private List<TextButton> activeMenuButtons = new List<TextButton>();
    private int currentButtonIndex = -1;

    private bool isPaused;
    private bool activeControls;

    private void Awake()
    {
        childrenTransforms = GetComponentsInChildren<Transform>(true);

        TextButton[] allTextButtons = GetComponentsInChildren<TextButton>(true);
        foreach (TextButton button in allTextButtons)
        {
            if (button.IsEnabled)
            {
                activeMenuButtons.Add(button);
            }
        }
    }

    private void Start()
    {
        InputManager inputManager = InputManager.Instance;
        if (inputManager == null)
        {
            Debug.LogWarning("InputManager instance not found ...");
        }
        else
        {
            pause = inputManager.GameInputActions.UI.Pause;
            select = inputManager.GameInputActions.UI.Select;
            confirm = inputManager.GameInputActions.UI.Confirm;

            ActiveControls = true;
        }
    }

    public void Pause()
    {
        AudioManager.Instance.MuteAll();
        InputManager.Instance.DisableInGameInput();
        UIManager.Instance?.SetCursorLockState(CursorLockMode.None);

        isPaused = true;

        if (childrenTransforms != null)
        {
            for (int i = 1; i < childrenTransforms.Length; i++)
            {
                childrenTransforms[i].gameObject.SetActive(true);
            }
        }
    }

    public void Unpause()
    {
        AudioManager.Instance.UnmuteAll();
        UIManager.Instance?.SetCursorLockState(CursorLockMode.Locked);
        InputManager.Instance.EnableInGameInput();

        isPaused = false;

        if (childrenTransforms != null)
        {
            for (int i = 1; i < childrenTransforms.Length; i++)
            {
                childrenTransforms[i].gameObject.SetActive(false);
            }
        }
    }

    public void RestartLevel()
    {
        Unpause();
        GameManager.Instance.ReloadLevel();
    }

    public void ToMainMenu()
    {
        Unpause();
        GameManager.Instance.ToMainMenu();
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }
    }

    private void Select(InputAction.CallbackContext context)
    {
        if (!isPaused)
        {
            return;
        }

        Vector2 input = select.ReadValue<Vector2>();
        int yChange = 0;
        if (input.y > 0.3f)
        {
            yChange = 1;
        }
        else if (input.y < -0.3f)
        {
            yChange = -1;
        }

        if (currentButtonIndex >= 0)
        {
            activeMenuButtons[currentButtonIndex].OnPointerExit(null);
        }
        currentButtonIndex = ((currentButtonIndex - yChange) + activeMenuButtons.Count) % activeMenuButtons.Count;
        activeMenuButtons[currentButtonIndex].OnPointerEnter(null);
    }

    private void Confirm(InputAction.CallbackContext context)
    {
        if (!isPaused)
        {
            return;
        }

        if (currentButtonIndex >= 0)
        {
            activeMenuButtons[currentButtonIndex].OnPointerDown(null);
            if (activeMenuButtons[currentButtonIndex].ResetOnClick)
            {
                currentButtonIndex = -1;
            }
        }
    }

    private void OnTextButtonPointerEnter(TextButton textButton)
    {
        if (!activeMenuButtons.Contains(textButton))
        {
            return;
        }

        if (currentButtonIndex >= 0)
        {
            activeMenuButtons[currentButtonIndex].ButtonExit();
        }
        currentButtonIndex = activeMenuButtons.IndexOf(textButton);
        activeMenuButtons[currentButtonIndex].ButtonEnter();
    }

    private void OnEnable()
    {
        GameEvents.TextButtonPointerEnterEvent += OnTextButtonPointerEnter;

        ActiveControls = true;
    }

    private void OnDisable()
    {
        GameEvents.TextButtonPointerEnterEvent -= OnTextButtonPointerEnter;

        ActiveControls = false;
    }
}

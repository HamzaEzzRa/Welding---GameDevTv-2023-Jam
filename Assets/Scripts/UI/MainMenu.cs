using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public bool ActiveControls
    {
        get => activeControls;
        set
        {
            activeControls = value;
            if (value)
            {
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

    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private TextButton continueButton;

    [SerializeField] private GameObject optionsMenuObject;
    [SerializeField] private Slider sensitivitySlider, musicSlider, sfxSlider;

    [SerializeField] private AudioMixer mainMixer;

    private InputAction select, confirm;

    private List<TextButton> menuTextButtons = new List<TextButton>();
    private int currentMenuButton = -1;

    private bool isOptionMenuActive;
    private Slider[] optionsSliders;
    private List<TextButton> optionsTextButtons = new List<TextButton>();
    private int currentOptionsButton = -1;

    private bool activeControls;

    private void Awake()
    {
        if (continueButton != null && SerializationManager.HasSaves())
        {
            continueButton.Enable();
        }

        if (mainMenuObject != null)
        {
            TextButton[] allTextButtons = mainMenuObject.GetComponentsInChildren<TextButton>();
            foreach (TextButton button in allTextButtons)
            {
                if (button.IsEnabled)
                {
                    menuTextButtons.Add(button);
                }
            }
        }

        if (optionsMenuObject != null)
        {
            optionsSliders = optionsMenuObject.GetComponentsInChildren<Slider>();

            TextButton[] allTextButtons = optionsMenuObject.GetComponentsInChildren<TextButton>();
            foreach (TextButton button in allTextButtons)
            {
                if (button.IsEnabled)
                {
                    optionsTextButtons.Add(button);
                }
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
            select = inputManager.GameInputActions.UI.Select;
            confirm = inputManager.GameInputActions.UI.Confirm;

            ActiveControls = true;
        }

        SetSensitivity(sensitivitySlider.value);

        if (musicSlider != null)
        {
            SetMusicVolume(musicSlider.value);
        }

        if (sfxSlider != null)
        {
            SetSFXVolume(sfxSlider.value);
        }
    }

    private void StartGame(UnityEvent callback)
    {
        if (!mainMenuObject.activeInHierarchy)
        {
            return;
        }

        mainMenuObject.SetActive(false);
        AudioManager.Instance.StopAllSounds();

        GameManager.Instance.LoadScene((SceneMap)GameManager.Instance.CurrentLevelOrder, UnityEngine.SceneManagement.LoadSceneMode.Single, callback);
    }

    public void NewGame()
    {
        GameManager.Instance.CurrentLevelOrder = 1;
        StartGame(null);
    }

    public void LoadGame()
    {
        SerializationManager.LoadGame();
        StartGame(null);
    }

    public void ShowSettings()
    {
        if (!mainMenuObject.activeInHierarchy)
        {
            return;
        }

        optionsMenuObject.SetActive(true);
        isOptionMenuActive = true;
    }

    public void HideSettings()
    {
        if (!mainMenuObject.activeInHierarchy)
        {
            return;
        }

        optionsMenuObject.SetActive(false);
        isOptionMenuActive = false;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetSensitivity(float value)
    {
        GameManager.Instance.SensitivityFactor = value;
    }

    public void SetMusicVolume(float value)
    {
        mainMixer.SetFloat("musicVolume", Mathf.Log10(value) * 20f);

        AudioManager.Instance.InitialMusicVolume = Mathf.Log10(value) * 20f;
    }

    public void SetSFXVolume(float value)
    {
        mainMixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Max(0.0001f, value - 0.3f)) * 20f);
        mainMixer.SetFloat("focusVolume", Mathf.Log10(Mathf.Max(0.0001f, value - 0.3f)) * 20f);

        AudioManager.Instance.InitialSFXVolume = Mathf.Log10(Mathf.Max(0.0001f, value - 0.3f)) * 20f;
        AudioManager.Instance.InitialFocusVolume = Mathf.Log10(Mathf.Max(0.0001f, value - 0.3f)) * 20f;
    }

    private void Select(InputAction.CallbackContext context)
    {
        Vector2 input = select.ReadValue<Vector2>();
        int xChange = 0, yChange = 0;
        if (input.y > 0.25f)
        {
            yChange = -1;
        }
        else if (input.y < -0.25f)
        {
            yChange = 1;
        }

        if (input.x > 0.25f)
        {
            xChange = 1;
        }
        else if (input.x < -0.25f)
        {
            xChange = -1;
        }

        if (yChange != 0 && xChange == 0)
        {
            if (!isOptionMenuActive)
            {
                if (currentMenuButton >= 0)
                {
                    menuTextButtons[currentMenuButton].ButtonExit();
                }
                currentMenuButton = ((currentMenuButton + yChange) + menuTextButtons.Count) % menuTextButtons.Count;
                menuTextButtons[currentMenuButton].ButtonEnter();
            }
            else
            {
                if (currentOptionsButton >= 0)
                {
                    optionsTextButtons[currentOptionsButton].ButtonExit();
                }
                currentOptionsButton = ((currentOptionsButton + yChange) + optionsTextButtons.Count) % optionsTextButtons.Count;
                optionsTextButtons[currentOptionsButton].ButtonEnter();
            }
        }

        if (xChange != 0 && yChange == 0)
        {
            if (isOptionMenuActive)
            {
                if (currentOptionsButton < optionsSliders.Length)
                {
                    optionsSliders[currentOptionsButton].value += xChange * 0.1f;
                }
            }
        }
    }

    private void Confirm(InputAction.CallbackContext context)
    {
        if (!isOptionMenuActive)
        {
            if (currentMenuButton >= 0)
            {
                menuTextButtons[currentMenuButton].ButtonDown();
                if (menuTextButtons[currentMenuButton].ResetOnClick)
                {
                    currentMenuButton = -1;
                }
            }
        }
        else
        {
            if (currentOptionsButton >= 0)
            {
                optionsTextButtons[currentOptionsButton].ButtonDown();
                if (optionsTextButtons[currentOptionsButton].ResetOnClick)
                {
                    currentOptionsButton = -1;
                }
            }
        }
    }

    private void OnTextButtonPointerEnter(TextButton textButton)
    {
        if (!isOptionMenuActive)
        {
            if (!menuTextButtons.Contains(textButton))
            {
                return;
            }

            if (currentMenuButton >= 0)
            {
                menuTextButtons[currentMenuButton].ButtonExit();
            }
            currentMenuButton = menuTextButtons.IndexOf(textButton);
            menuTextButtons[currentMenuButton].ButtonEnter();
        }
        else
        {
            if (!optionsTextButtons.Contains(textButton))
            {
                return;
            }

            if (currentOptionsButton >= 0)
            {
                optionsTextButtons[currentOptionsButton].ButtonExit();
            }
            currentOptionsButton = optionsTextButtons.IndexOf(textButton);
            optionsTextButtons[currentOptionsButton].ButtonEnter();
        }
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

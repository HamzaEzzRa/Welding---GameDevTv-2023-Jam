using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public GameInputActions GameInputActions => gameInputActions;

    private GameInputActions gameInputActions;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        gameInputActions = new GameInputActions();
    }

    public void EnableInGameInput()
    {
        gameInputActions.InGame.Enable();
    }

    public void DisableInGameInput()
    {
        gameInputActions.InGame.Disable();
    }

    public void EnableMenuInput()
    {
        gameInputActions.UI.Enable();
    }

    public void DisableMenuInput()
    {
        gameInputActions.UI.Disable();
    }

    public void EnableEndScreenInput()
    {
        gameInputActions.EndScreen.Enable();
    }
    
    private void DisableEndScreenInput()
    {
        gameInputActions.EndScreen.Enable();
    }
}

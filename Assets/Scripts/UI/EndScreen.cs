using UnityEngine;
using UnityEngine.InputSystem;

public class EndScreen : MonoBehaviour
{
    private InputAction exit;

    public void Enable()
    {
        exit = InputManager.Instance.GameInputActions.EndScreen.Exit;
        exit.Enable();
        exit.performed += Exit;

        gameObject.SetActive(true);
        SerializationManager.DestroySave();
    }

    private void OnDisable()
    {
        if (exit != null)
        {
            exit.Disable();
            exit.performed -= Exit;
        }
    }

    private void Exit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            gameObject.SetActive(false);
            GameManager.Instance.ToMainMenu();
        }
    }
}

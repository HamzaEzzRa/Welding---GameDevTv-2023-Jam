using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneChangeTrigger : TriggerArea
{
    [SerializeField] private SceneMap nextSceneIndex;
    [SerializeField] private UnityEvent onNextScene;

    protected override void HandleTriggerEnter(Collider other)
    {
        GameManager.Instance.CurrentLevelOrder++;

        AudioManager.Instance.StopAllSFXs();
        InputManager.Instance.DisableInGameInput();
        onNextScene.AddListener(() =>
        {
            InputManager.Instance.EnableInGameInput();
        });

        GameManager.Instance.LoadScene(nextSceneIndex, LoadSceneMode.Single, onNextScene);
        SerializationManager.SaveGame();
    }

    protected override void HandleTriggerExit(Collider other)
    {

    }
}

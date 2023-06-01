using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

public enum SceneMap
{
    MAIN_MENU = 0,
    LEVEL_1 = 1,
    LEVEL_2 = 2,
    LEVEL_3 = 3,
    LEVEL_4 = 4,
}

public class GameManager : MonoBehaviour, IPersistentObject
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Image sceneFader;
    [SerializeField] private GameObject progressBar;

    private List<AsyncOperation> sceneAsyncOps = new List<AsyncOperation>();

    public float SensitivityFactor { get; set; } = 1f;

    public int CurrentLevelOrder { get; set; } = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        QualitySettings.vSyncCount = 1;
    }

    public void LoadScene(SceneMap sceneMap, LoadSceneMode loadSceneMode, UnityEvent callback = null)
    {
        StopAllCoroutines();

        sceneAsyncOps.Add(SceneManager.LoadSceneAsync((int)sceneMap, loadSceneMode));
        StartCoroutine(SceneProgressCoroutine(callback));
    }

    public void UnloadScene(SceneMap sceneMap)
    {
        sceneAsyncOps.Add(SceneManager.UnloadSceneAsync((int)sceneMap));
    }

    public IEnumerator SceneProgressCoroutine(UnityEvent callback)
    {
        Color tmp = sceneFader.color;
        tmp.a = 1f;
        sceneFader.color = tmp;
        sceneFader.gameObject.SetActive(true);
        progressBar.SetActive(true);

        for (int i = 0; i < sceneAsyncOps.Count; i++)
        {
            while (!sceneAsyncOps[i].isDone)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(3f);

        callback?.Invoke();

        progressBar.SetActive(false);

        float progress = 0f;
        while (progress < 1f)
        {
            progress += 0.5f * Time.deltaTime;

            tmp = sceneFader.color;
            tmp.a = 1 - progress;
            sceneFader.color = tmp;

            yield return null;
        }

        tmp = sceneFader.color;
        tmp.a = 0f;
        sceneFader.color = tmp;
        sceneFader.gameObject.SetActive(false);
    }

    public void ToMainMenu()
    {
        AudioManager.Instance.StopAllSounds();

        sceneAsyncOps.Add(SceneManager.LoadSceneAsync((int)SceneMap.MAIN_MENU, LoadSceneMode.Single));
        StartCoroutine(SceneProgressCoroutine(null));
    }

    public void ReloadLevel(UnityEvent callback = null)
    {
        StopAllCoroutines();

        sceneAsyncOps.Add(SceneManager.LoadSceneAsync(CurrentLevelOrder, LoadSceneMode.Single));
        StartCoroutine(SceneProgressCoroutine(callback));
    }

    public void SaveData(GameData data)
    {
        data.currentLevelOrder = CurrentLevelOrder;
    }

    public void LoadData(GameData data)
    {
        CurrentLevelOrder = data.currentLevelOrder;
    }

    private void OnEnable()
    {
        SerializationManager.AddPersistentObject(this);
    }

    private void OnDisable()
    {
        SerializationManager.RemovePersistentObject(this);
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TMP_Text text;

    private Coroutine currentRoutine;

    private void Awake()
    {
        infoPanel.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    public void Show(string message, float seconds = 5f)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(ShowRoutine(message, seconds));
    }

    private IEnumerator ShowRoutine(string message, float seconds)
    {
        text.text = message ?? "";
        infoPanel.SetActive(true);

        yield return new WaitForSeconds(seconds);

        Hide();
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        Hide();
    }

    private void Hide()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        infoPanel.SetActive(false);
    }
}

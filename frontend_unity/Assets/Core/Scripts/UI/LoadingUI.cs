using UnityEngine;
using TMPro;
using System.Collections;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float showDelay = 0.5f;

    private Coroutine showCoroutine;

    private void Awake()
    {
        Hide();
    }

    public void Show(string message)
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
        }

        showCoroutine = StartCoroutine(ShowDelayed(message));
    }

    private IEnumerator ShowDelayed(string message)
    {
        yield return new WaitForSeconds(showDelay);

        loadingPanel.SetActive(true);

        if (messageText != null)
        {
            messageText.text = string.IsNullOrEmpty(message) ? "" : message;
        }

        showCoroutine = null;
    }

    public void Hide()
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }

        if (messageText != null)
        {
            messageText.text = "";
        }

        loadingPanel.SetActive(false);
    }
}

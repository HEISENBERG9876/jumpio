using UnityEngine;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TMP_Text messageText;

    private void Awake()
    {
        if (loadingPanel == null)
        {
            loadingPanel = gameObject;
        }
        Hide();
    }

    public void Show(string message)
    {
        loadingPanel.SetActive(true);

        if (messageText != null)
        {
            messageText.text = string.IsNullOrEmpty(message) ? "" : message;
        }
    }

    public void Hide()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
        loadingPanel.SetActive(false);
    }
}

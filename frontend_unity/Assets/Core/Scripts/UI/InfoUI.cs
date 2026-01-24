using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        if (infoPanel == null)
        {
            infoPanel = gameObject;
        }

        infoPanel.SetActive(false);
    }

    public void Show(string message, float seconds = 5f)
    {
        ShowAsync(message, seconds).Forget();
    }

    private async UniTask ShowAsync(string message, float seconds)
    {
        text.text = message ?? "";
        infoPanel.SetActive(true);
        await UniTask.Delay(System.TimeSpan.FromSeconds(seconds));
        infoPanel.SetActive(false);
    }
}

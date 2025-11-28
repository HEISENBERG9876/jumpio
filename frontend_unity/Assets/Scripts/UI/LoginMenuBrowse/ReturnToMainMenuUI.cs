using UnityEngine;

public class ReturnToMainMenuUI : MonoBehaviour
{
    public void OnReturnToMenuButtonClicked()
    {
               UIManager.Instance.ShowMainMenuPanel();
    }
}

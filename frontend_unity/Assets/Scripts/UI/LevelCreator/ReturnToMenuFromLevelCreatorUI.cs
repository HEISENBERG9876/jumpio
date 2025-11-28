using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuFromLevelCreatorUI : MonoBehaviour
{
    public void OnReturnToMenuButtonClicked()
    {
        SceneManager.LoadScene("LoginMenuBrowseScene");
    }
}
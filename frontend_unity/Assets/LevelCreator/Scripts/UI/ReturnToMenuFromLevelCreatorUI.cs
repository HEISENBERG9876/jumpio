using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuFromLevelCreatorUI : MonoBehaviour
{
    public RuntimeLevelData runtimeLevelData;
    public void OnReturnToMenuButtonClicked()
    {
        runtimeLevelData.Clear();
        SceneManager.LoadScene("LoginMenuBrowseScene");
    }
}
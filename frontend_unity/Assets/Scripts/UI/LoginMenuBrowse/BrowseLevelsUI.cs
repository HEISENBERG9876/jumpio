using UnityEngine;
using UnityEngine.SceneManagement;

public class BrowseLevelsUI : MonoBehaviour
{
    public void OnBrowseLevelsButtonClicked()
    {
        UIManager.Instance.ShowBrowsePanel();
    }
}

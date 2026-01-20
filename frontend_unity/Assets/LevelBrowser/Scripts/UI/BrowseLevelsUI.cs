using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class BrowseLevelsUI : MonoBehaviour
{
    public LevelBrowser levelBrowser;
    public void OnBrowseLevelsButtonClicked() // better to delete, belongs to LevelBrowser
    {
        UIManager.Instance.OnBrowseLevelsClicked();
    }
}

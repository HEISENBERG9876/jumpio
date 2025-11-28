using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class BrowseLevelsUI : MonoBehaviour
{
    public LevelBrowser levelBrowser;
    public async void OnBrowseLevelsButtonClicked()
    {
        await UIManager.Instance.ShowBrowsePanel();
    }
}

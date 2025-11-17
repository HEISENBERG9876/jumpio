using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateLevelUI : MonoBehaviour
{
     public void OnCreateLevelButtonClicked()
    {
        SceneManager.LoadScene("LevelCreatorScene");
    }
}

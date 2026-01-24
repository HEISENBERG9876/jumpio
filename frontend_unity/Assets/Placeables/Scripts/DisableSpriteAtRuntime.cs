using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableSpriteAtRuntime : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "LevelCreatorScene")
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = false;
            }
        }
    }
}

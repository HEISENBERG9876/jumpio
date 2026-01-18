using UnityEngine;

public class CreatorSession : MonoBehaviour
{
    private static CreatorSession _instance;

    public static CreatorSession Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CreatorSession>();
                if (_instance == null)
                {
                    Debug.LogError("[CreatorSession] Instance not found.");
                }
            }
            return _instance;
        }
    }

    [SerializeField] private GameObject creatorRoot;
    public GameObject CreatorRoot
    {
        get { 
            return creatorRoot; 
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            if (creatorRoot != null)
            {
                _instance.creatorRoot = creatorRoot;
            }
            Destroy(gameObject);
            return;
        }

        if (creatorRoot == null)
        {
            Debug.LogError("[CreatorSession] CreatorRoot not found");
        }
    }

    public void SetCreatorActive(bool shouldBeActive)
    {
        if (creatorRoot == null)
        {
            Debug.LogError("[CreatorSession] CreatorRoot not assigne");
            return;
        }

        creatorRoot.SetActive(shouldBeActive);
    }

    public void EnterTestMode() => SetCreatorActive(false);
    public void ExitTestMode() => SetCreatorActive(true);
}

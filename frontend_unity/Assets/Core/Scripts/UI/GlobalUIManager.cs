using UnityEngine;

public class GlobalUIManager : MonoBehaviour
{
    [SerializeField] private LoadingUI loadingPanel;
    [SerializeField] private InfoUI infoPanel;

    private static GlobalUIManager _instance;
    public static GlobalUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GlobalUIManager>();
                if (_instance == null)
                {
                    Debug.LogError("[GlobalUIManager] Instance not found.");
                }
            }
            return _instance;
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
            Destroy(gameObject);
        }
    }

    public void ShowLoading(string msg = null)
    {
        loadingPanel.Show(msg);
    }
    
    public void HideLoading()
    {
        loadingPanel.Hide();
    }

    public void ShowInfo(string msg)
    {
        infoPanel.Show(msg);
    }
}

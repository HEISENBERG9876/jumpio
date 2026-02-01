using UnityEngine;

public class ClearData : MonoBehaviour
{
    public RuntimeLevelData runtimeLevelData;
    public RuntimeCampaignData runtimeCampaignData;
    public TestLevelData testLevelData;
    void Start()
    {
        runtimeLevelData.Clear();
        runtimeCampaignData.Clear();
        testLevelData.Clear();
    }

    void Update()
    {
        
    }
}

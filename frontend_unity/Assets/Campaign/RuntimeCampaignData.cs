using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/RuntimeCampaignData")]
public class RuntimeCampaignData : ScriptableObject
{
    public bool hasData;
    public string campaignId;
    public int currentIndex;

    public List<string> levelLayoutUrls = new List<string>();
    public List<int> timers = new List<int>();
    public string campaignTitle;

    public void Clear()
    {
        hasData = false;
        campaignId = null;
        currentIndex = 0;
        levelLayoutUrls.Clear();
        timers.Clear();
        campaignTitle = null;
    }
}
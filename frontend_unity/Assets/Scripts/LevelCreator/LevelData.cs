using System.Collections.Generic;

[System.Serializable]
public class PlacedObjectData
{
    public string id;
    public int x;
    public int y;
    public int width = 1;
    public int height = 1;
    public int rotation = 0;
    public string metadataJson;
}


[System.Serializable]
public class LevelPostBody
{
    public string title;
    public string difficulty; //easy/medium/hard
    public int timer = 120;
    public List<PlacedObjectData> layout;
}
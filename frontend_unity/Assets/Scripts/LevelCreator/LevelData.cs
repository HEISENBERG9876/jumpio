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
public class LayoutWrapper
{
    public List<PlacedObjectData> layout;
}
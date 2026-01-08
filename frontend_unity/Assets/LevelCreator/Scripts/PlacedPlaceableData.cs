using System.Collections.Generic;
using UnityEngine.Serialization;

[System.Serializable]
public class PlacedPlaceableData
{
    public string id; //Placeable id
    [FormerlySerializedAs("worldX")]
    public float x; //might be local or world depending on context
    [FormerlySerializedAs("worldY")]
    public float y;
    //public int width = 1;
    //public int height = 1;
    public int rotation = 0;
    //public string metadataJson;

    // TODO metadata, rotation, width and height most likely useless. Might add rotation support.
}
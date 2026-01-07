using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/LevelChunk")]
public class LevelChunk : ScriptableObject
{
    public int width;
    public int height;
    //public Vector2Int entranceCell;
    //public Vector2Int exitCell;
    public List<PlacedPlaceableData> placeables = new();
}
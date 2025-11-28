using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Placeable Database")]
public class PlaceableDatabase : ScriptableObject
{
    public List<Placeable> placeables;
}
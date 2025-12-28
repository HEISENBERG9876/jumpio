using System.Collections.Generic;
using UnityEngine;

//TODO Prefabs are meant to have unique names, but just in case: add duplicate ID detection and correction
[CreateAssetMenu(menuName = "Scriptable Objects/Placeable Database")]
public class PlaceableDatabase : ScriptableObject
{
    public List<Placeable> placeables;
}
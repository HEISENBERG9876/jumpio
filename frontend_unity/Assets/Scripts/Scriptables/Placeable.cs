using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable Objects/Placeable")]
public class Placeable : ScriptableObject
{
    public string id;
    public Sprite icon;
    public GameObject prefab;   
}
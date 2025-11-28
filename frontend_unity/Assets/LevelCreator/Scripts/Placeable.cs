using UnityEngine;

//TODO auto generate ID, take icon from prefab
[CreateAssetMenu(menuName = "Scriptable Objects/Placeable")]
public class Placeable : ScriptableObject
{
    public string id;
    public Sprite icon;
    public GameObject prefab;   
}
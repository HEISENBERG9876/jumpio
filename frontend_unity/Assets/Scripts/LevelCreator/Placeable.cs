using UnityEngine;


[CreateAssetMenu(menuName = "LevelCreator/Placeable")]
public class Placeable : ScriptableObject
{
    public string id;
    public Sprite icon;
    public GameObject prefab;   
}
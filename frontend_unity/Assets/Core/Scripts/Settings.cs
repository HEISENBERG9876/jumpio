using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Scriptable Objects/Settings")]
public class Settings : ScriptableObject
{
    [Header("API")]
    [SerializeField] private string baseUrl = "http://localhost:8000/api/";
    [SerializeField] private string userEndpoint = "users/";
    [SerializeField] private string levelEndpoint = "levels/";
    public string baseUserUrl => baseUrl + userEndpoint;
    public string baseLevelUrl => baseUrl + levelEndpoint;
    [Header("Gameplay")]
    public Vector2 playerSpawnPosition = new Vector2(0, 0);
}

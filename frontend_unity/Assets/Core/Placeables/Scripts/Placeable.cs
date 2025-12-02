using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Scriptable Objects/Placeable")]
public class Placeable : ScriptableObject
{
    public string id;
    public Sprite icon;
    public GameObject prefab;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(string.IsNullOrEmpty(id) && prefab != null)
        {
            id = prefab.name;
            EditorUtility.SetDirty(this);
        }
        if(icon == null && prefab != null)
        {
            var spriteRenderer = prefab.GetComponent<SpriteRenderer>();
            if(spriteRenderer != null)
            {
                icon = spriteRenderer.sprite;
                EditorUtility.SetDirty(this);
            }
        }
    }
#endif
}
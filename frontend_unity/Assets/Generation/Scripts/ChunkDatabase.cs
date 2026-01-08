using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ChunkDatabase", menuName = "Scriptable Objects/ChunkDatabase")]
public class ChunkDatabase : ScriptableObject
{
    public List<LevelChunk> chunks = new();
}

using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public ArenaEpisodeController arenaPrefab;

    public int arenasX = 4;
    public int arenasY = 4;
    public int spacingX = 70;
    public int spacingY = 30;

    public int baseSeed = 1234;

    void Start()
    {
        int idx = 0;
        for (int y = 0; y < arenasY; y++)
        {
            for (int x = 0; x < arenasX; x++)
            {
                var arena = Instantiate(arenaPrefab, transform);
                arena.transform.position = new Vector3(x * spacingX, y * spacingY, 0f);
                arena.baseSeed = baseSeed + idx * 99991;
                idx++;
            }
        }
    }
}

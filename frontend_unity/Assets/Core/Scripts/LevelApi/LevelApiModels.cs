using System.Collections.Generic;


[System.Serializable]
public class LevelDataResponse
{
    public int id;
    public string title;
    public string difficulty;
    public int timer;
    public string layout_file; // URL to the layout file
    public string preview_image_file;
}

[System.Serializable]
public class PaginatedLevelsResponse
{
    public int count;
    public string next;
    public string previous;
    public List<LevelDataResponse> results;
}
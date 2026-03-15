using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public int Width;
    public int Height;
    public int Mines;

    public GameSettings(int w, int h, int m)
    {
        Width = w;
        Height = h;
        Mines = m;
    }

    public static GameSettings Beginner =
        new GameSettings(9,9,10);

    public static GameSettings Intermediate =
        new GameSettings(16,16,40);

    public static GameSettings Expert =
        new GameSettings(30,16,99);
    
    public static GameSettings SelectedMode = Beginner; 
}
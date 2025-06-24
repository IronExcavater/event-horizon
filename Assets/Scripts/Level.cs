using UI;
using Utilities;

public class Level : Singleton<Level>
{
    public float score;
    public float time;

    public static float GetScore => Instance.score;
    public static float GetTime => Instance.time;

    public LevelUI levelUI;

    private void Update()
    {
        levelUI.Score.Value = score;
        levelUI.Timer.Value = time;
    }
}

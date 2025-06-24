using UI;
using UnityEngine;
using Utilities;

public class Level : Singleton<Level>
{
    public float score;
    public float time;

    public static float GetScore
    {
        get => Instance.score;
        set => Instance.score = value;
    }

    public static float GetTime => Instance.time;

    public LevelUI levelUI;

    private void Update()
    {
        time += Time.deltaTime;

        score += Time.deltaTime / 4;

        levelUI.Score.Value = score;
        levelUI.Timer.Value = time;
    }
}

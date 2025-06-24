using UI;
using UnityEngine;
using Utilities;

public class ScoreManager : Singleton<ScoreManager>
{
    public float score = 0;
    public float time = 0;

    float startTime = 0;
    bool isTiming = false;

    public static float GetScore => Instance.score;
    public static float GetTime => Instance.time;

    public LevelUI levelUI;

    public void UpdateScore(int _score)
    {
        score += _score;
    }
    public void ResetScore()
    {
        score = 0;
    }
    private void Update()
    {
        if (isTiming) time = Time.time - startTime;

        //levelUI.Score.Value = score;
        //levelUI.Timer.Value = time;
    }
    public void StartTimer()
    {
        startTime = Time.time;
        isTiming = true;
    }
}

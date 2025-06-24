using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    int totalScore = 0;

    public void UpdateScore(int _score)
    {
        totalScore += _score;
        Debug.Log("Score: " + totalScore);
    }
    public void ResetScore()
    {
        totalScore = 0;
    }
}

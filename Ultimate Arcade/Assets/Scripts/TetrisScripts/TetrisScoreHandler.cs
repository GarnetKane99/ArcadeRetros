using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TetrisScoreHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI HighScoreText;
    [SerializeField] private TextMeshProUGUI LevelText;

    [SerializeField] private int CurrentScore;
    [SerializeField] private int CurrentLevel;
    [SerializeField] private int HighScore;
    [SerializeField] private int LinesPassed;


    // Start is called before the first frame update
    void Start()
    {
        CurrentLevel = 1;
        HighScore = PlayerPrefs.GetInt("TetrisHighScore");
        LevelText.text = CurrentLevel.ToString();
        HighScoreText.text = HighScore.ToString();
    }

    public void SetCurrentScore()
    {
        CurrentScore += 100 * CurrentLevel;

        if(CurrentScore > HighScore)
        {
            HighScore = CurrentScore;
            HighScoreText.text = HighScore.ToString();
            PlayerPrefs.SetInt("TetrisHighScore", HighScore);
        }

        ScoreText.text = CurrentScore.ToString();
    }

    public void SetCurrentLevel()
    {
        LinesPassed++;
        if (LinesPassed % 15 == 0)
        {
            CurrentLevel++;
            LevelText.text = CurrentLevel.ToString();
        }
    }

    public int GetScore()
    {
        return CurrentScore;
    }

    public int GetLevel()
    {
        return CurrentLevel;
    }
}

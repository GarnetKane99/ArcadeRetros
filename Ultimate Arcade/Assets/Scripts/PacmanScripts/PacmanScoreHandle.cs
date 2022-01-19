using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PacmanScoreHandle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI HighScoreText;
    [SerializeField] private TextMeshProUGUI LevelText;

    [SerializeField] private int CurrentScore;
    [SerializeField] private int CurrentLevel;
    [SerializeField] private int HighScore;

    // Start is called before the first frame update
    void Start()
    {
        CurrentLevel = 1;
        HighScore = PlayerPrefs.GetInt("PacmanHighScore");
        LevelText.text = CurrentLevel.ToString();
        HighScoreText.text = HighScore.ToString();
    }
}

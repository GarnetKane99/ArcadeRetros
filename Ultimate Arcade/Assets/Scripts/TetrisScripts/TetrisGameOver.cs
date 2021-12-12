using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TetrisGameOver : MonoBehaviour
{
    [SerializeField] private Button PlayAgain;
    [SerializeField] private Button MainMenu;

    private void Start()
    {
        PlayAgain.onClick.AddListener(ReloadScene);
        MainMenu.onClick.AddListener(MainMenuScene);
    }

    private void ReloadScene()
    {
        Scene CurScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(CurScene.name);
    }

    private void MainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

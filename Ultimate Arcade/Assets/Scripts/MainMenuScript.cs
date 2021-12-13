using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    public Button Tetris, Pacman, Asteroids, SpaceInvaders, Frogger, Pong;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        Tetris.onClick.AddListener(LoadTetris);
        Pacman.onClick.AddListener(LoadPacman);
    }

    void LoadTetris()
    {
        SceneManager.LoadScene("Tetris");
    }

    void LoadPacman()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TetrisPauseHandler : MonoBehaviour
{
    [SerializeField] private Button Continue, Quit;
    [SerializeField] private AudioSource Music;
    [SerializeField] private GameObject MenuItems;
    [SerializeField] private TetrisBoardManager TManager;

    public bool currentlyPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        Continue.onClick.AddListener(ContinueGame);
        Quit.onClick.AddListener(QuitGame);
    }

    void ContinueGame()
    {
        currentlyPaused = false;
        MenuItems.SetActive(false);
        Music.volume *= 2.0f;
        Time.timeScale = 1;
    }

    void QuitGame()
    {

    }

    private void Update()
    {
        CheckInput();
    }

    void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !TManager.GameOverScreen.activeInHierarchy)
        {
            currentlyPaused = !currentlyPaused;
            if (currentlyPaused)
            {
                MenuItems.SetActive(true);
                Music.volume /= 2.0f;
                Time.timeScale = 0;
            }
            else
            {
                ContinueGame();
            }
        }
    }

}

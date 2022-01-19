using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PacmanPauseHandler : MonoBehaviour
{
    [SerializeField] private Button Continue, Quit;
    [SerializeField] private AudioSource Music;
    [SerializeField] private GameObject MenuItems;
    [SerializeField] PacmanController PacController;

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
        //Music.volume *= 2.0f;
        Time.timeScale = 1;
    }

    void QuitGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void Update()
    {
        if (PacController == null)
        {
            PacController = FindObjectOfType<PacmanController>();
        }
        CheckInput();
    }

    void CheckInput()
    {
        if (PacController != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !PacController.GameOverScreen.activeInHierarchy)
            {
                currentlyPaused = !currentlyPaused;
                if (currentlyPaused)
                {
                    MenuItems.SetActive(true);
                    //Music.volume /= 2.0f;
                    Time.timeScale = 0;
                }
                else
                {
                    ContinueGame();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                currentlyPaused = !currentlyPaused;
                if (currentlyPaused)
                {
                    MenuItems.SetActive(true);
                    //Music.volume /= 2.0f;
                    Time.timeScale = 0;
                }
                else
                {
                    ContinueGame();
                }
            }
        }
    }
}

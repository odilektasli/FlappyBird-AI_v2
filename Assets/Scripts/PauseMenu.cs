using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static int timeScaleCounter = 0;
    public GameObject pauseMenuUI;
    public GameObject dontdestroyonload;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameIsPaused)
        {
            Pause();
        } else
        {
            Resume();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadScene(string scene)
    {
        if(scene == "Game")
        {
            SceneManager.LoadScene("Game");
            GameIsPaused = false;
        }
        else if (scene == "MainMenu") {
            SceneManager.LoadScene("MainMenu");
            GameIsPaused = false;
        }
    }

    public void SetSpeed()
    {
        
        if(timeScaleCounter == 0)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().timeScale = 2;
            timeScaleCounter++;
            GameObject.Find("Speed").GetComponentInChildren<Text>().text = "2X";
        } else if(timeScaleCounter == 1)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().timeScale = 3;
            timeScaleCounter++;
            GameObject.Find("Speed").GetComponentInChildren<Text>().text = "3X";
        } else if(timeScaleCounter == 2 )
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().timeScale = 5;
            timeScaleCounter++;
            GameObject.Find("Speed").GetComponentInChildren<Text>().text = "5X";
        } else if (timeScaleCounter == 3)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().timeScale = 1;
            timeScaleCounter = 0;
            GameObject.Find("Speed").GetComponentInChildren<Text>().text = "1X";
        }

        

    }

}

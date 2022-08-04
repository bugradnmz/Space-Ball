using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject mainMenu, controls, userInterface, gameMenu, astronauts;
    TextMeshProUGUI pointsUI; //In game interface
    TextMeshProUGUI pointsMenu; //Game menu which appears when level completed

    [HideInInspector] public float points;
    float maxPoints;

    void Start()
    {
        //DontDestroyOnLoad(this);
        //Debug.Log(SceneManager.GetActiveScene().buildIndex);
        //Debug.Log(SceneManager.sceneCountInBuildSettings - 1);

        if (SceneManager.GetActiveScene().buildIndex != 0) //Otherwise script looks for UI elements in the main menu and we get editor error
        {
            pointsUI = userInterface.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            pointsMenu = gameMenu.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            maxPoints = astronauts.transform.childCount;
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0) //Otherwise script looks for UI elements in the main menu and we get editor error
        {
            pointsUI.text = (points + " / " + maxPoints);
            pointsMenu.text = (points + " / " + maxPoints);
        }
    }

    public void LoadNextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1) //if it is last level, load level 1 again
        { 
            SceneManager.LoadScene(1); 
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }            
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Controls()
    {
        mainMenu.SetActive(false);
        controls.SetActive(true);
    }

    public void Back()
    {       
        controls.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void GameMenu()
    {
        userInterface.SetActive(false);
        gameMenu.SetActive(true);
    }
}

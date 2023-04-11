using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static GameManager instance()
    {
        return _instance;
    }
    public PlayerControllerNeo player;
    public Canvas gameOverCanvas;
    public Canvas victoryCanvas;
    public TMP_Text coinText;
    public TMP_Text dashText;
    public Canvas caveCanvas;
    private void Start()
    {

        //player.Reset();
        //gameOverCanvas.gameObject.SetActive(false);
        victoryCanvas.gameObject.SetActive(false);
        caveCanvas.gameObject.SetActive(false);
    }
    public void onRestartClick()
    {
        //player.Reset();
        coinText.text = "x" + 0;
        gameOverCanvas.gameObject.SetActive(false);
        SceneManager.LoadScene("mainField");
    }
    public void onMenuClick()
    {
        SceneManager.LoadScene("mainMenu");
    }
    public void onQuitClick()
    {
        Application.Quit();
    }
    public void gameOverCanvasSwitch(bool state)
    {
        gameOverCanvas.gameObject.SetActive(state);
    }
    public void victoryCanvasSwitch(bool state)
    {
        victoryCanvas.gameObject.SetActive(state);
    }

    public void caveCanvasSwitch(bool state)
    {
        caveCanvas.gameObject.SetActive(state);
    }    
    public PlayerControllerNeo getPlayer()
    {

        return player;

    }
}

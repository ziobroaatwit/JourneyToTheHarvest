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
    private void Start()
    {

        //player.Reset();
        gameOverCanvas.gameObject.SetActive(false);
        victoryCanvas.gameObject.SetActive(false);
    }
    public void updateCoinText(int value)
    {
        coinText.text = "x" + value;
    }
    public void updateDashText(int value)
    {
        if (value <= 0)
        {
            dashText.text = "Dash In: NOW";
        }
        else
        {
            dashText.text = "Dash In: " + value;
        }
    }
    public void onRestartClick()
    {
        //player.Reset();
        coinText.text = "x" + 0;
        gameOverCanvas.gameObject.SetActive(false);
        SceneManager.LoadScene("MainGame");
    }
    public void onMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void gameOverCanvasSwitch(bool state)
    {
        gameOverCanvas.gameObject.SetActive(state);
    }
    public void victoryCanvasSwitch(bool state)
    {
        victoryCanvas.gameObject.SetActive(state);
    }

    public PlayerControllerNeo getPlayer()
    {

        return player;

    }
}

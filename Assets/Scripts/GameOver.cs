using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text scoreText;
    public Text killsText;

    private void Awake()
    {
        scoreText.text = "Score: " + GameController.instance.score.ToString();
        killsText.text = "Demons killed: " + GameController.instance.noDemonsKilled.ToString();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}

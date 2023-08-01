using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text pointsText;
    public int points;
    public bool ded;
    [SerializeField] private GameObject howToPlayScreen;
    [SerializeField] private GameObject restartUI;
    private float timeRemaining = 121f;
    [SerializeField] private TMP_Text timer;
    // Update is called once per frame
    void Update()
    {
        pointsText.text = "Points: " + points;
        timeRemaining -= Time.deltaTime;
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        timer.text = string.Format("{0:00} : {1:00}",minutes,seconds);
        if(timeRemaining <= 0)
        {
            restartUI.SetActive(true);
            timeRemaining = 0f;
        }
    }

    public void OnRestartClicked()
    {
        SceneManager.LoadScene(0);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    public void OnXClicked()
    {
        howToPlayScreen.SetActive(false);
    }
    public void OnQuestionClicked()
    {
        howToPlayScreen.SetActive(true);
    }

}

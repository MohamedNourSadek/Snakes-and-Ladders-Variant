using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject endGamePanel;

    [Header("References")]
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI rollText;
    [SerializeField] TextMeshProUGUI winnerName;
    [SerializeField] Button rollButton;
    [SerializeField] Button restartGameButton;
    [SerializeField] Button backMenuButton;
    
    public static UIController instance;

    private void Awake()
    {
        instance = this;
        restartGameButton.onClick.AddListener(OnRestartPress);
        backMenuButton.onClick.AddListener(OnBackMenuPress);
    }

    public void EndGameCondition(PlayerInfo winnerName)
    {
        this.winnerName.text = winnerName.playerName + " has won the game";
        this.winnerName.color = winnerName.playerColor;

        endGamePanel.SetActive(true);
        gamePanel.SetActive(false);
    }
    public void AddOnRollEvent(UnityAction action)
    {
        rollButton.onClick.AddListener(action);
    }
    public void UpdatePlayerName(string name, Color playerColor)
    {
        playerName.text = name;    
        playerName.color = playerColor;
    }
    public void UpdateRoll(string rollNumber)
    {
        rollText.text = rollNumber;
    }
    public void CanRoll(bool state)
    {
        rollButton.interactable = state;
    }



    //private events
    void OnRestartPress()
    {
        SceneManager.LoadScene(1);
    }
    void OnBackMenuPress()
    {
        SceneManager.LoadScene(0);
    }
}

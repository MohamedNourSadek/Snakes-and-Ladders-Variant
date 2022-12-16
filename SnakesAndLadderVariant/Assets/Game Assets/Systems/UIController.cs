using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject endGamePanel;
    [SerializeField] GameObject logMessageAsset;

    [Header("References")]
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI rollText;
    [SerializeField] TextMeshProUGUI dieRollText;
    [SerializeField] TextMeshProUGUI winnerName;
    [SerializeField] Button rollButton;
    [SerializeField] Button playButton;
    [SerializeField] Button skipButton;
    [SerializeField] Button restartGameButton;
    [SerializeField] Button restartGameMiniButton;
    [SerializeField] Button backMenuButton;
    [SerializeField] Button backMenuMiniButton;
    [SerializeField] Button greenArrowsButton;

    public static UIController instance;
    public bool showingMessage = false;
    public bool isGreenArrowsOn;
    public Queue<MessageData> gameMessages = new Queue<MessageData>();


    //Initailization
    private void Awake()
    {
        instance = this;

        restartGameButton.onClick.AddListener(OnRestartPress);
        restartGameMiniButton.onClick.AddListener(OnRestartPress);

        backMenuButton.onClick.AddListener(OnBackMenuPress);
        backMenuMiniButton.onClick.AddListener(OnBackMenuPress);

        greenArrowsButton.onClick.AddListener(OnGreenActionPress);
    }
    private void Update()
    {
        if(showingMessage == false && gameMessages.Count > 0)
        {
            showingMessage = true;
            SpawnMessage(gameMessages.Dequeue());
        }
    }

    //UI functionalities
    public void EndGameCondition(PlayerInfo winnerName)
    {
        this.winnerName.text = winnerName.playerName + " has won the game";
        this.winnerName.color = winnerName.playerColor;

        endGamePanel.SetActive(true);
        gamePanel.SetActive(false);
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
    public void UpdateDie(int dieNumber)
    {
        if (dieNumber == 1)
            dieRollText.text = ".";
        else if (dieNumber == 2)
            dieRollText.text = "..";
        else if (dieNumber == 3)
            dieRollText.text = "...";
        else if (dieNumber == 4)
            dieRollText.text = "..\n..";
        else if (dieNumber == 5)
            dieRollText.text = ".....";
        else if (dieNumber == 6)
            dieRollText.text = "......";
    }
    public void SetRollAbility(bool state)
    {
        rollButton.interactable = state;
    }
    public void SetPlayAbility(bool state)
    {
        playButton.interactable = state;
    }
    public void SetSkipAbility(bool state)
    {
        skipButton.interactable = state;
    }
    public void LogGameMessage(string message)
    {
        LogGameMessage(message, Color.grey, 1f);
    }
    public void LogGameMessage(string message, float time)
    {
        LogGameMessage(message, Color.gray, time);
    }
    public void LogGameMessage(string message, Color color, float time)
    {
        gameMessages.Enqueue(new MessageData { text = message, color = color, time = time });
    }

    public void SpawnMessage(MessageData message)
    {
        TextMeshProUGUI gameMessage = Instantiate(logMessageAsset, canvas.transform).GetComponent<TextMeshProUGUI>();
        gameMessage.text = message.text;
        gameMessage.color = message.color;

        gameMessage.GetComponent<GameMessage>().Initialize(message.time);
    }


    //Events Handler events
    void OnGreenActionPress()
    {
        SoundManager.instance.PlayEffect(Effects.ButtonPress);
        isGreenArrowsOn = !isGreenArrowsOn;
        greenArrowsButton.GetComponentInChildren<Image>().color = isGreenArrowsOn ? Color.green : Color.black;
    }
    void OnRestartPress()
    {
        SoundManager.instance.PlayEffect(Effects.ButtonPress);
        SceneManager.LoadScene(1);
    }
    void OnBackMenuPress()
    {
        SoundManager.instance.PlayEffect(Effects.ButtonPress);
        SceneManager.LoadScene(0);
    }



    //Events Subscribtion
    public void AddOnRollEvent(UnityAction action)
    {
        rollButton.onClick.AddListener(action);
    }
    public void AddOnPlayEvent(UnityAction action)
    {
        playButton.onClick.AddListener(action);
    }
    public void AddOnSkipEvent(UnityAction action)
    {
        skipButton.onClick.AddListener(action);
    }
    public void AddOnGreenEvent(UnityAction action)
    {
        greenArrowsButton.onClick.AddListener(action);
    }
}



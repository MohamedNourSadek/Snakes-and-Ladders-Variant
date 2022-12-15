using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

enum ErrorType { error, warning};

public class MenuManager : MonoBehaviour
{
    [SerializeField] TMPro.TMP_InputField playerName;
    [SerializeField] TMPro.TMP_Dropdown playerColor;
    [SerializeField] TextMeshProUGUI playerText;
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] Button add;
    [SerializeField] Button start;

    private void Awake()
    {
        GlobalData.gameData = new GameData();

        start.onClick.AddListener(OnStartPress);
        add.onClick.AddListener(OnAddPress);
    }

    public void OnAddPress()
    {
        if(playerName.text.Length > 4)
        {
            PlayerInfo player = new PlayerInfo(playerName.text, GetColor());
            GlobalData.gameData.PlayersInfo.Add(player);

            //Refresh UI
            playerColor.value = playerColor.value == 3 ? 0 : (playerColor.value + 1);
            playerName.text = "";


            playerText.text = "Player " + (GlobalData.gameData.PlayersInfo.Count + 1).ToString();
            LogMessage("", ErrorType.error);


            if (GlobalData.gameData.PlayersInfo.Count > 4)
            {
                LogMessage("Number of players is very high, you still can play" +
                            ", but it might not be unstable", ErrorType.warning);
            }
        }
        else
        {
            LogMessage("Player Name is too short", ErrorType.error);
        }

    }
    public void OnStartPress()
    {
        if(GlobalData.gameData.PlayersInfo.Count >= 2)
        {
            LogMessage("",ErrorType.warning);
            SceneManager.LoadScene(1);
        }
        else
        {
            LogMessage("Players must be 2 or more to start", ErrorType.error);
        }
    }

    void LogMessage(string message, ErrorType type)
    {
        errorText.text = message;
        errorText.color = (type == ErrorType.error? Color.red : Color.blue);
    }
    Color GetColor()
    {
        Color color;

        if (playerColor.value == 0)
            color = new Color(1, .5f, 0f);
        else if (playerColor.value == 1)
            color = new Color(.5f, 0.6f, .9f);
        else if (playerColor.value == 2)
            color = new Color(.13f, .5f, 0.133f);
        else
            color = Color.black;

        return color;
    }
}

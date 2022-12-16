using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

enum ErrorType { error, warning};

public class MenuManager : MonoBehaviour
{
    [SerializeField] RectTransform animatedBackground;
    [SerializeField] float circleAnimationRadius;
    [SerializeField] float circleAnimationSpeed;
    [SerializeField] TMPro.TMP_InputField playerName;
    [SerializeField] TMPro.TMP_Dropdown playerColor;
    [SerializeField] TextMeshProUGUI playerText;
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] Button add;
    [SerializeField] Button start;

    private void Awake()
    {
        GlobalData.FrameRate = 60;
        Application.targetFrameRate = GlobalData.FrameRate;
        GlobalData.gameData = new GameData();

        start.onClick.AddListener(OnStartPress);
        add.onClick.AddListener(OnAddPress);

        StartCoroutine(animateBackground());
    }

    public void OnAddPress()
    {
        SoundManager.instance.PlayEffect(Effects.ButtonPress);

        if(playerName.text.Length > 4)
        {
            PlayerInfo player = new PlayerInfo(playerName.text, GetColor());
            GlobalData.gameData.PlayersInfo.Add(player);

            //Refresh UI
            playerColor.value = playerColor.value == 3 ? 0 : (playerColor.value + 1);
            playerName.text = "";


            playerText.text = "Player " + (GlobalData.gameData.PlayersInfo.Count + 1).ToString();
            ClearLog();


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
        SoundManager.instance.PlayEffect(Effects.ButtonPress);

        if (GlobalData.gameData.PlayersInfo.Count >= 2)
        {
            ClearLog();
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

        if (type == ErrorType.error)
            SoundManager.instance.PlayEffect(Effects.Error);

    }
    void ClearLog()
    {
        errorText.text = "";
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
    IEnumerator animateBackground()
    {
        Vector3 position = Vector3.zero;

        while(true)
        {
            position.x = (Screen.width/2f) + (circleAnimationRadius * Mathf.Sin(Time.timeSinceLevelLoad * circleAnimationSpeed));
            position.y = (Screen.height/2f) + circleAnimationRadius * Mathf.Cos(Time.timeSinceLevelLoad * circleAnimationSpeed);
            
            animatedBackground.position = position;


            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}

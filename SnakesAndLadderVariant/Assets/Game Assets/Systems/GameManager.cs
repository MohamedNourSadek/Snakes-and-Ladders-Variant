using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [Header("Grid Design Parameters")]
    [SerializeField] Transform grid00;
    [SerializeField] float separation = 2.1f;

    [Header("References")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject gridPrefab;
    [SerializeField] Transform mapParent;

    [Header("Game Elements")]
    [SerializeField] float rollingTime = 1f;

    Vector2 endGamePoint = new(1, 8);
    List<Player> players= new List<Player>();
    GridPoint[,] map = new GridPoint[8, 8];
    int currentPlayer = 0;
    int currentRollNumber;

    //Initialization functions
    void Awake()
    {
        UIController.instance.AddOnRollEvent(OnRollPressed);

        GameData gameData = CreateTestGameData();
        CreateGridMap();
        SpawnPlayers(gameData.PlayersInfo);
        UpdatePlayerUI();

        //start game
        currentPlayer = 0;
        UIController.instance.CanRoll(true);
    }
    GameData CreateTestGameData()
    {
        GameData gameData = new GameData();
        PlayerInfo player1Info = new PlayerInfo("Mohamed", Color.red);
        PlayerInfo player2Info = new PlayerInfo("Omar", Color.blue);
        gameData.PlayersInfo.Add(player1Info);
        gameData.PlayersInfo.Add(player2Info);

        return gameData;
    }
    void CreateGridMap()
    {
        for(int y = 1; y <= 8; y++)
        {
            for(int x = 1; x <= 8; x++)
            {
                Vector2 destination = ComputeDefaultNextDest(new Vector2(x,y));
                GridPoint point =  Instantiate(gridPrefab, grid00.position + (separation * new Vector3(x, 0f, y)), gridPrefab.transform.rotation, mapParent).GetComponent<GridPoint>();


                point.Initialize(new Vector2(x,y), PointType.Default, destination);
                map[x-1,y-1] = point;
            }
        }
    }
    void SpawnPlayers(List<PlayerInfo> playersInfo)
    {
        foreach (PlayerInfo playerInfo in playersInfo)
        {
            Player player = Instantiate(playerPrefab, this.transform.position, playerPrefab.transform.rotation).GetComponent<Player>();
            
            player.SetPlayerInfo(playerInfo);
            player.SetPoint(map[0,0]);
            players.Add(player);
        }
    }

    //Game Loop
    void SetNextPlayer()
    {
        if (currentPlayer >= (players.Count - 1))
            currentPlayer = 0;
        else
            currentPlayer ++;

        UpdatePlayerUI();
    }
    void UpdatePlayerUI()
    {
        PlayerInfo playerInfo = players[currentPlayer].GetPlayerInfo();
        UIController.instance.UpdatePlayerName(playerInfo.playerName, playerInfo.playerColor);
    }
    void OnRollPressed()
    {
        UIController.instance.CanRoll(false);
        StartCoroutine(Rolling());
    }
    void ExecuteRoll()
    {
        UIController.instance.CanRoll(true);

        GridPoint finalPoint = ComputeFinalPoint(players[currentPlayer].GetPoint().point, currentRollNumber);
        players[currentPlayer].SetPoint(finalPoint);
         
        if (finalPoint.point == endGamePoint)
            UIController.instance.EndGameCondition(players[currentPlayer].GetPlayerInfo());
        else
            SetNextPlayer();
    }


    //Algorithms
    GridPoint ComputeFinalPoint(Vector2 current, int dice)
    {
        Vector2 nextMove = current;
        int moves = dice;

        while(moves>0)
        {
            nextMove = ComputeDefaultNextDest(nextMove);

            moves--;
        }

        return map[(int)nextMove.x - 1, (int)nextMove.y - 1];
    }
    Vector2 ComputeDefaultNextDest(Vector2 current)
    {
        Vector2 next = new Vector2();

        if (current == endGamePoint)
            return endGamePoint; //End Game Condition


        //odd y moves right by default, unless it's the last one, then it moves up.
        //but even y moves left by default, ---
        bool even = (current.y % 2 == 0);

        if(even)
        {
            if(current.x != 1)
            {
                next = current + Vector2.left;
            }
            else
            {
                next = current + Vector2.up;
            }
        }
        else
        {
            if (current.x != 8)
            {
                next = current + Vector2.right;
            }
            else
            {
                next = current + Vector2.up;
            }
        }

        return next;
    }
    IEnumerator Rolling()
    {
        float time = rollingTime;

        while(time > 0)
        {
            currentRollNumber = UnityEngine.Random.Range(1, 7);

            UIController.instance.UpdateRoll(currentRollNumber.ToString());

            time -= 0.1f;
            yield return new WaitForSeconds(.1f);
        }

        ExecuteRoll();
    }
}

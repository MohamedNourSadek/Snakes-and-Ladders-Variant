using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [Header("Grid Design Parameters")]
    [SerializeField] float separation = 2.1f;
    [SerializeField] Transform grid00;

    [Header("References")]
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject gridPrefab;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] Transform mapParent;
    [SerializeField] Transform arrowParent;

    [Header("Game Elements")]
    [SerializeField] float rollingTime = 1f;
    [SerializeField] float playerSpeed = 1f;
    [SerializeField] UnityEngine.Color pitfallColor; 
    [SerializeField] UnityEngine.Color shortcutColor; 

    Vector2 endGamePoint = new(1, 8);
    List<Player> players= new List<Player>();
    GridPoint[,] map = new GridPoint[8, 8];
    int currentPlayer = 0;
    int currentRollNumber;


    //Initialization functions
    void Awake()
    {
        Application.targetFrameRate = 60;
        UIController.instance.AddOnRollEvent(OnRollPressed);
        GameData gameData = (GlobalData.gameData.PlayersInfo.Count >= 2) ? GlobalData.gameData : CreateTestGameData();

        CreateGridMap();
        SpawnArrows();
        SpawnPlayers(gameData.PlayersInfo);
        UpdatePlayerUI();

        //start game settings
        currentPlayer = 0;
        UIController.instance.CanRoll(true);
    }
    GameData CreateTestGameData()
    {
        GameData gameData = new GameData();
        PlayerInfo player1Info = new PlayerInfo("Mohamed", UnityEngine.Color.red);
        PlayerInfo player2Info = new PlayerInfo("Omar", UnityEngine.Color.blue);
        gameData.PlayersInfo.Add(player1Info);
        gameData.PlayersInfo.Add(player2Info);

        return gameData;
    }
    void CreateGridMap()
    {
        //Create All default first
        for(int y = 1; y <= 8; y++)
        {
            for(int x = 1; x <= 8; x++)
            {
                Vector2 destination = ComputeDefaultNextStep(new Vector2(x,y));
                GridPoint point =  Instantiate(gridPrefab, grid00.position + (separation * new Vector3(x, 0f, y)), gridPrefab.transform.rotation, mapParent).GetComponent<GridPoint>();
                point.Initialize(new Vector2(x,y), PointType.Default, destination);

                map[x-1,y-1] = point;
            }
        }

        //Shortcuts
        map[5, 0].pointType = PointType.ShortCut;
        map[5, 0].pointDestination = new Vector2(6, 2);
        map[6, 0].pointType = PointType.ShortCut;
        map[6, 0].pointDestination = new Vector2(7, 2);


        //Pitfalls
        map[3, 1].pointType = PointType.Pitfall;
        map[3, 1].pointDestination = new Vector2(4, 1);
        map[4, 1].pointType = PointType.Pitfall;
        map[4, 1].pointDestination = new Vector2(5, 1);
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
    void SpawnArrows()
    {
        for (int y = 1; y <= 8; y++)
        {
            for (int x = 1; x <= 8; x++)
            {
                //create the arrow if it's not the end game point
                if (new Vector2(x,y) != endGamePoint)
                {
                    GridPoint point = map[x - 1, y - 1];
                    Arrow arrow = Instantiate(arrowPrefab, arrowParent).GetComponent<Arrow>();
                    arrow.SetArrowTransform(point, PointToGridPoint(point.pointDestination));

                    UnityEngine.Color color = (point.pointType == PointType.Pitfall) ? pitfallColor : shortcutColor; 

                    if(point.pointType != PointType.Default)
                    {
                        point.GetComponentInChildren<MeshRenderer>().material.color = color;

                        foreach (MeshRenderer mesh in arrow.GetComponentsInChildren<MeshRenderer>())
                        {
                            mesh.material.color = color;
                        }
                    }

                }
            }
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


    //Algorithms
    List<GridPoint> ComputePlayerPath(Vector2 current, int dice)
    {
        List<GridPoint> animationKeys = new List<GridPoint>();
        Vector2 nextMove = current;
        int moves = dice;

        while(moves>0)
        {
            nextMove = ComputeDefaultNextStep(nextMove);

            animationKeys.Add(PointToGridPoint(nextMove));

            moves--;
        }

        if (animationKeys[animationKeys.Count - 1].pointType != PointType.Default)
        {
            Vector2 point = animationKeys[animationKeys.Count - 1].pointDestination;
            animationKeys.Add(PointToGridPoint(point));
        }

        return animationKeys;
    }
    Vector2 ComputeDefaultNextStep(Vector2 current)
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
    GridPoint PointToGridPoint(Vector2 point)
    {
        return map[(int)(point.x - 1), (int)(point.y - 1)];
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

        StartCoroutine(ExcuteMove());
    }
    IEnumerator ExcuteMove()
    {
        var animationKeys = ComputePlayerPath(players[currentPlayer].GetPoint().point, currentRollNumber);

        float animationTimePerKey = 1f / playerSpeed;

        for (int i = 0; i < animationKeys.Count; i++)
        {
            float ramainingPerKey = animationTimePerKey;
            float numberOfIncrements = 25f;

            float deltaTime = animationTimePerKey / numberOfIncrements;
            Vector3 deltaSpace = (animationKeys[i].gameObject.transform.position - players[currentPlayer].transform.position) / numberOfIncrements;

            while (ramainingPerKey > 0)
            {
                players[currentPlayer].transform.position += deltaSpace;
                ramainingPerKey -= deltaTime;
                yield return new WaitForSecondsRealtime(deltaTime);
            }

            players[currentPlayer].transform.position = animationKeys[i].gameObject.transform.position;
        }

        players[currentPlayer].SetPoint(animationKeys[animationKeys.Count - 1]);


        if (animationKeys[animationKeys.Count - 1].point == endGamePoint)
            UIController.instance.EndGameCondition(players[currentPlayer].GetPlayerInfo());
        else
            SetNextPlayer();

        UIController.instance.CanRoll(true);
    }
}

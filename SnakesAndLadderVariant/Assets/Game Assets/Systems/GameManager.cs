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
    bool boostInPath;


    //Initialization functions
    void Awake()
    {
        Application.targetFrameRate = GlobalData.FrameRate;
        UIController.instance.AddOnRollEvent(OnRollPressed);
        UIController.instance.AddOnPlayEvent(OnPlayPress);
        UIController.instance.AddOnSkipEvent(OnSkipPress);
        UIController.instance.AddOnGreenEvent(SetGreenArrowsState);

        GameData gameData = (GlobalData.gameData.PlayersInfo.Count >= 2) ? GlobalData.gameData : CreateTestGameData();

        CreateGridMap();
        SpawnArrows();
        SpawnPlayers(gameData.PlayersInfo);
        UpdatePlayerUI();

        //start game settings
        currentPlayer = 0;
        UIController.instance.SetRollAbility(true);
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

        //Pitfalls
        //I will generate one in the upper part that is big, and one small in the middle half.
        GenerateRandomBoost(new Vector2(5,8), new Vector2(1, 2), PointType.Pitfall);
        GenerateRandomBoost(new Vector2(2,5), new Vector2(3, 4), PointType.Pitfall);

        //Pitfalls
        //I will generate one in the upper part that is big, and one small in the middle half.
        GenerateRandomBoost(new Vector2(4, 7), new Vector2(5, 6), PointType.ShortCut);
        GenerateRandomBoost(new Vector2(1, 4), new Vector2(7, 8), PointType.ShortCut);
    }
    void GenerateRandomBoost(Vector2 Yrange, Vector2 Xrange, PointType pointType)
    {
        int yPoint = (int)(Random.Range(Yrange.x - 1, Yrange.y));
        int xPoint = (int)(Random.Range(Xrange.x - 1, Xrange.y));
        int endPoint = 0;

        if (pointType == PointType.ShortCut)
            endPoint = Random.Range(yPoint + 1, 8);
        else
            endPoint = Random.Range(0, yPoint);

        //reject point if its already a special move point
        bool isStartVaild = PointToGridPoint(new Vector2(xPoint + 1, yPoint +1)).pointType == PointType.Default;
        bool isEndValid = PointToGridPoint(new Vector2(xPoint +1, endPoint + 1)).pointType == PointType.Default;
       
        if (!isStartVaild || !isEndValid)
        {
            GenerateRandomBoost(Yrange, Xrange, pointType);
            return;
        }

        //contine if it's valid
        map[xPoint, yPoint].pointType = pointType;
        map[xPoint, yPoint].pointDestination = new Vector2(xPoint + 1, endPoint + 1);
    }
    void SpawnPlayers(List<PlayerInfo> playersInfo)
    {
        foreach (PlayerInfo playerInfo in playersInfo)
        {
            Player player = Instantiate(playerPrefab, this.transform.position, playerPrefab.transform.rotation).GetComponent<Player>();
            
            player.Initialize(playerInfo, map[0, 0]);
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
                    point.myArrow = arrow;

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

        SetGreenArrowsState();
    }
    void SetGreenArrowsState()
    {
        for (int y = 1; y <= 8; y++)
        {
            for (int x = 1; x <= 8; x++)
            {
                if (new Vector2(x, y) != endGamePoint)
                {
                    GridPoint point = map[x - 1, y - 1];

                    if (point.pointType == PointType.Default)
                    {
                        foreach (MeshRenderer mesh in point.myArrow.GetComponentsInChildren<MeshRenderer>())
                        {
                            mesh.enabled = UIController.instance.isGreenArrowsOn;
                        }
                    }
                }
            }
        }
    }


    //Game Loop
    void SetNextPlayer()
    {
        players[currentPlayer].SetLastRoll(currentRollNumber);

        players[currentPlayer].SetHighlight(false);

        if (currentPlayer >= (players.Count - 1))
            currentPlayer = 0;
        else
            currentPlayer ++;


        UpdatePlayerUI();
    }
    void UpdatePlayerUI()
    {
        PlayerInfo playerInfo = players[currentPlayer].GetPlayerInfo();

        players[currentPlayer].SetHighlight(true);
        UIController.instance.UpdatePlayerName(playerInfo.playerName, playerInfo.playerColor);
        UIController.instance.SetPlayAbility(false);
        UIController.instance.SetSkipAbility(false);
        UIController.instance.SetRollAbility(true);
    }


    //Events
    void OnRollPressed()
    {
        SoundManager.instance.PlayEffect(Effects.Roll);
        UIController.instance.SetRollAbility(false);
        StartCoroutine(Rolling());
    }
    void OnPlayPress()
    {
        SoundManager.instance.PlayEffect(Effects.ButtonPress);
        UIController.instance.SetSkipAbility(false);
        UIController.instance.SetPlayAbility(false);

        StartCoroutine(ExcuteMove());
    }
    void OnSkipPress()
    {
        SoundManager.instance.PlayEffect(Effects.ButtonPress);
        UIController.instance.SetSkipAbility(false);
        UIController.instance.SetPlayAbility(false);

        players[currentPlayer].UseSkip();
        players[currentPlayer].IncreaseReserve(currentRollNumber);

        SetNextPlayer();
    }


    //Algorithms
    List<GridPoint> ComputePlayerPath(Vector2 current, int dice)
    {
        List<GridPoint> animationKeys = new List<GridPoint>();
        Vector2 nextMove = current;
        int moves = dice;

        while (moves>0)
        {
            nextMove = ComputeDefaultNextStep(nextMove);

            animationKeys.Add(PointToGridPoint(nextMove));

            moves--;
        }

        if (animationKeys[animationKeys.Count - 1].pointType != PointType.Default)
        {
            Vector2 point = animationKeys[animationKeys.Count - 1].pointDestination;
            animationKeys.Add(PointToGridPoint(point));
            boostInPath = true;
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


        while (time > 0)
        {
            currentRollNumber = Random.Range(1,7);

            UIController.instance.UpdateRoll(currentRollNumber.ToString());
            UIController.instance.UpdateDie(currentRollNumber);

            time -= 0.1f;
            yield return new WaitForSeconds(.1f);
        }


        //6 6 in a row
        if (players[currentPlayer].GetLastRoll() == 6 && currentRollNumber == 6)
        {
            UIController.instance.SetSkipAbility(false);
            UIController.instance.SetPlayAbility(false);

            SetNextPlayer();
        }
        else
        {
            if (players[currentPlayer].CanSkip())
            {
                UIController.instance.SetSkipAbility(true);
                UIController.instance.SetPlayAbility(true);
            }
            else
            {
                if(players[currentPlayer].GetReserveAmount() > 0)
                {
                    UIController.instance.UpdateRoll(currentRollNumber.ToString() + " + " + players[currentPlayer].GetReserveAmount().ToString());

                    currentRollNumber += players[currentPlayer].GetReserveAmount();
                    players[currentPlayer].SetReserve(0);

                    UIController.instance.SetPlayAbility(true);
                }
                else
                {
                    UIController.instance.SetPlayAbility(true);
                }
            }
        }

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

            if ((i == animationKeys.Count - 2) && boostInPath)
            {
                if (animationKeys[i].pointType == PointType.Pitfall)
                    SoundManager.instance.PlayEffect(Effects.Fail);
                else if (animationKeys[i].pointType == PointType.ShortCut)
                    SoundManager.instance.PlayEffect(Effects.Successs);
                
                boostInPath = false;
            }

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

        UIController.instance.SetRollAbility(true);
    }
}

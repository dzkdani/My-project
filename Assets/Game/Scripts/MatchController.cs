using UnityEngine;
using UnityEngine.UI;

public class MatchController : MonoBehaviour
{
    public static MatchController Instance;
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private const int MAXMATCH = 5;
    private const float TIMEPERMATCH = 140f;
    private int blueWinCount;
    private int redWinCount;

    [Header("Match Status")]
    public bool IsPlaying;
    public int matchCount;

    [Header("Scripts Component")]
    public Button pauseButton;
    public GameObject paused;
    public Timer Timer;
    public SpawnController Spawn;
    public ResultController Result;
    public Energy Energy;
    [Header("GameObjects Component")]
    public GameObject Player1Gate;
    public GameObject Player1Fence;
    public GameObject Player2Gate;
    public GameObject Player2Fence;

    Material player1Material;
    Material player2Material;

    public void Init() {
        blueWinCount = 0;
        redWinCount = 0;

        player1Material = Player1Gate.GetComponent<Renderer>().material;
        player2Material = Player2Gate.GetComponent<Renderer>().material;

        matchCount = 0;
        Spawn = SpawnController.Instance;
        MatchStart();

        pauseButton.onClick.AddListener(delegate {MatchPause();});
    }

    public void MatchStart() {
        if (matchCount % 2 == 0)
        {
            Player1Fence.GetComponent<Renderer>().material = player1Material;
            Player1Gate.GetComponent<Renderer>().material = player1Material;
            Player2Fence.GetComponent<Renderer>().material = player2Material;
            Player2Gate.GetComponent<Renderer>().material = player2Material;
        }
        else
        {
            Player1Fence.GetComponent<Renderer>().material = player2Material;
            Player1Gate.GetComponent<Renderer>().material = player2Material;
            Player2Fence.GetComponent<Renderer>().material = player1Material;
            Player2Gate.GetComponent<Renderer>().material = player1Material;
        }

        Result.resultPanel.SetActive(false);
        Timer.InitTimer();
        Spawn.BallSpawner();
        Spawn.RefreshPool();
        Energy.InitEnergy();
        IsPlaying = true;
        matchCount += 1;
    }

    private void MatchPause() {
        if (IsPlaying)
        {
            IsPlaying = false;
            paused.SetActive(true);
        }
        else
        {
            IsPlaying = true;
            paused.SetActive(false);
        }
    }

    public void MatchEnd(Color _winner) {
        IsPlaying = false;
        
        if (_winner == Color.blue)
        {
            blueWinCount++;
            Result.SetMatchWinner("Blue", redWinCount, blueWinCount);
        }

        if (_winner == Color.red)
        {
            redWinCount++;
            Result.SetMatchWinner("Red", redWinCount, blueWinCount);
        }

        if (matchCount == MAXMATCH)
        {
            if (blueWinCount > redWinCount)
            {
                Result.SetGameWinner("Blue");
            }
            else
            {
                Result.SetGameWinner("Red");
            }
        }
    }
}

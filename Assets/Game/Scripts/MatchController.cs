using UnityEngine;

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
    private int atkWinCount;
    private int defWinCount;

    [Header("Match Status")]
    public bool IsPlaying;
    public int matchCount;

    [Header("Scripts Component")]
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

    private void Start() {
        Init();
    }

    public void Init() {
        atkWinCount = 0;
        defWinCount = 0;

        player1Material = Player1Gate.GetComponent<Renderer>().material;
        player2Material = Player2Gate.GetComponent<Renderer>().material;

        matchCount = 0;
        Spawn = SpawnController.Instance;
        MatchStart();
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

    public void MatchEnd(string _winner) {
        IsPlaying = false;
        Result.SetMatchWinner(_winner);
        
        if (_winner == "Attacker")
        {
            atkWinCount++;
        }

        if (_winner == "Defender")
        {
            defWinCount++;
        }

        if (matchCount == MAXMATCH)
        {
            if (atkWinCount > defWinCount)
            {
                Result.SetGameWinner("Attacker");
            }
            else
            {
                Result.SetGameWinner("Defender");
            }
        }
    }
}

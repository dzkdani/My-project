using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("General Status")]
    private bool CanMove;
    [SerializeField]
    private float SpawnTime = 0.5f;
    [SerializeField]
    private Type unitType;
    
    [Header("Attacker Status")]
    public bool IsHoldingBall;
    public bool GetCaught;

    [Header("Defender Status")]
    [SerializeField]
    private bool FindAttackerWithBall;
    [SerializeField]
    private Unit AttackerLockedOn;
    private Vector3 DefSpawnPos;

    Material material;
    Color player1Color;
    Color player2Color;
    Color inactiveColor;

    GameObject ball;
    GameObject goal;
    GameObject arrow;
    GameObject radar;

    [Header("Attacker Variables")]
    [SerializeField]
    private float atkNormalSpd;
    [SerializeField]
    private float atkCarrySpd;
    [SerializeField]
    private float atkReactiveDur;
    [SerializeField]
    [Header("Defender Variables")]
    private float defNormalSpd;
    [SerializeField]
    private float defReturnSpd;
    [SerializeField]
    private float defReactiveDur;

    #region Init
    private void Awake() {
        material = GetComponent<Renderer>().material;
        player1Color = Color.blue;
        player2Color = Color.red;
        inactiveColor = Color.gray;
    }

    private void OnEnable() {
        StartCoroutine(SpawnTimeDelay());
        ball = GameObject.FindWithTag("Ball");
        goal = GameObject.FindWithTag("Goal");
        arrow = transform.GetChild(1).gameObject;
        radar = transform.GetChild(0).gameObject;
    }
    
    private void OnDisable() {
        gameObject.name = "Unit";
        CanMove = false;
        IsHoldingBall = false;
        GetCaught = false;
    }

    private IEnumerator SpawnTimeDelay() {
        material.color = inactiveColor;
        CanMove = false;

        yield return new WaitForSecondsRealtime(SpawnTime);
        
        CanMove = true;
        
        if (gameObject.name.Contains("Defender"))
        {
            material.color = CheckDefenderColor();
            radar.SetActive(true);
            arrow.SetActive(false);
            unitType = Type.Defender;
            gameObject.tag = "Defender";
            DefSpawnPos = transform.position;
        }
        if (gameObject.name.Contains("Attacker"))
        {
            material.color = CheckAttackerColor();
            radar.SetActive(false);
            arrow.SetActive(false);
            unitType = Type.Attacker;
            gameObject.tag = "Attacker";
        }
    }

    private Color CheckDefenderColor() 
    {
        if (MatchController.Instance.matchCount % 2 == 0)
            {
                return player1Color;
            }
            else 
            {
                return player2Color;
            }
    }

    private Color CheckAttackerColor()
    {
        if (MatchController.Instance.matchCount % 2 == 0)
            {
                return player2Color;
            }
            else 
            {
                return player1Color;
            }
    }
    #endregion

    private void Update() {
        if (MatchController.Instance.IsPlaying)
        {
            if (CanMove)
            {
                switch (unitType)
                {
                    case Type.Attacker : AttackerBehaviour(); 
                    break;
                    case Type.Defender : DefenderBehaviour();
                    break;
                }
            }
            else
            {
                if (unitType == Type.Attacker && GetCaught)
                {
                    PassTheBall();
                }
            }
        }
    }

    #region Behaviour
    #region Defender
    private void DefenderBehaviour() {
        if (FindAttackerWithBall)
        {
            CatchAttacker(AttackerLockedOn);
        }
        else
        {
            ReturnToSpwnPos();
        }
    }

    private void CatchAttacker(Unit _attacker) {
        transform.position = Vector3.MoveTowards(transform.position, _attacker.gameObject.transform.position, defNormalSpd * Time.deltaTime);
    }

    private void ReturnToSpwnPos() {
        transform.position = Vector3.MoveTowards(transform.position, DefSpawnPos, defReturnSpd * Time.deltaTime);
    }
    private IEnumerator DefenderInactive(float _dur)
    {
        yield return new WaitForSecondsRealtime(_dur);
    
        CanMove = true;
        material.color = CheckDefenderColor();
        radar.SetActive(true);
        gameObject.tag = "Defender";
    }
    #endregion

    #region Attacker
    private void AttackerBehaviour() {
        if (!ball.GetComponent<Ball>().IsPickedUp)
        {
            FindTheBall();
        }
        else 
        {
            if (IsHoldingBall)
            {
                MoveToGate();
            }
            else 
            {
                MoveForward();
            }
        }
    }

    private void FindTheBall() {
        Vector3 ballPos = new Vector3(ball.transform.position.x, ball.transform.position.y + 0.5f,ball.transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, ballPos, atkNormalSpd * Time.deltaTime);
    }

    private void MoveToGate() {
        transform.position = Vector3.MoveTowards(transform.position, goal.transform.position, atkCarrySpd * Time.deltaTime);
        ball.transform.position = transform.position + Vector3.up;

        arrow.gameObject.SetActive(true);     
    }

    private void MoveForward() {
        transform.position += Vector3.forward * atkCarrySpd * Time.deltaTime;

        arrow.SetActive(true);
    }

    private void PassTheBall() {
        ball.GetComponent<Ball>().CanPass(FindNearestAttacker());
    }

    private Vector3 FindNearestAttacker() {
        GameObject[] nearestAttackers;
        nearestAttackers = GameObject.FindGameObjectsWithTag("Attacker");
        List<GameObject> nearestActiveAttackers = new List<GameObject>();
        
        foreach (GameObject attacker in nearestAttackers)
        {
            if (attacker.GetComponent<Unit>().CanMove)
            {
                nearestActiveAttackers.Add(attacker);
            }
        }

        if (nearestActiveAttackers.Count > 0)
        {
            float[] distances = new float[nearestActiveAttackers.Count];
            int nearestIndex = 0;
            float nearestDistance = distances[nearestIndex];
            for (int i = 0; i < nearestActiveAttackers.Count; i++)
            {
                distances[i] = Vector3.Distance(nearestAttackers[i].transform.position, transform.position);
            }

            for (int i = 0; i < distances.Length; i++)
            {
                if (nearestDistance > distances[i])
                {
                    nearestDistance = distances[i];
                    nearestIndex = i;        
                }
            }
            return nearestActiveAttackers[nearestIndex].transform.position;
        }
        else
        {
            Destroy(ball);
            MatchController.Instance.MatchEnd(CheckDefenderColor());
            return Vector3.zero;
        }
    }

    private IEnumerator AttackerInactive(float _dur)
    {
        yield return new WaitForSecondsRealtime(_dur);
    
        CanMove = true;
        GetCaught = false;
        material.color = CheckAttackerColor();
        gameObject.tag = "Attacker";
    }
    #endregion
    #endregion

    #region Collider

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Field"))
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        }

        if (other.CompareTag("Fence"))
        {
            SpawnController.Instance.ReturnToPool(gameObject);
        }

        if (other.CompareTag("Goal"))
        {
            if (IsHoldingBall)
            {
                Destroy(ball);
                MatchController.Instance.MatchEnd(CheckAttackerColor());
            }
        }

        if (other.CompareTag("Ball"))
        {
            if (!IsHoldingBall && unitType == Type.Attacker && CanMove)
            {
                IsHoldingBall = true;
                ball.transform.position = transform.position + Vector3.up * 2;
                ball.GetComponent<Ball>().IsPickedUp = true;
            }
        }

        if (other.CompareTag("Defender"))
        {
            if (unitType == Type.Attacker)
            {
                if (IsHoldingBall)
                {
                    gameObject.tag = "Unit";
                    IsHoldingBall = false;
                    CanMove = false;
                    GetCaught = true;
                    transform.position = transform.position;
                    material.color = inactiveColor;
                    arrow.SetActive(false);
                    StartCoroutine(AttackerInactive(atkReactiveDur));
                }
            }
        }

        if (other.CompareTag("Attacker"))
        {
            if (unitType == Type.Defender && other.GetComponent<Unit>().IsHoldingBall)
            {
                //first collision with radar
                FindAttackerWithBall = true;
                radar.SetActive(false);
                AttackerLockedOn = other.GetComponent<Unit>();
                
                //second collision with defender
                if (FindAttackerWithBall && Vector3.Distance(transform.position, other.GetComponent<Transform>().position) < 1)
                {
                    FindAttackerWithBall = false;
                    transform.position = transform.position;
                    material.color = inactiveColor;
                    StartCoroutine(DefenderInactive(defReactiveDur));
                }
            }
        }
    }
    #endregion
}

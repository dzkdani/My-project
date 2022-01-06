using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Attacker,
    Defender
}

public class SpawnController : MonoBehaviour
{
    public static SpawnController Instance;
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
    private const string DEFENDER = "Defender";
    private const string ATTACKER = "Attacker";
    public GameObject BallPrefab;
    public GameObject UnitPrefab;
    public Transform AttackerField;
    public Transform DefenderField;
    
    [Header("Object Pool System")]
    [SerializeField]
    private int MaxUnitPool;
    private List<GameObject> UnitPool = new List<GameObject>();
    public Transform UnitPoolContainer;

    Plane plane;
    Vector3 normal;

    private void Start() {
        InitUnitPool();
    }

    #region Control
    private void Update() {
#if UNITY_ANDROID
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var input = Input.GetTouch(0).position;
            Vector3 spawnPos = Camera.main.ScreenToWorldPoint(input);
            spawnPos += Vector3.down * 24;
            Ray ray = Camera.main.ScreenPointToRay(input);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name.Contains(DEFENDER))
                {
                    UnitSpawner(DEFENDER, spawnPos);
                }
                if (hit.transform.name.Contains(ATTACKER))
                {
                    UnitSpawner(ATTACKER, spawnPos);
                }
            }
        }
#endif

#if UNITY_EDITOR_WIN
        if (Input.GetMouseButtonDown(0))
        {
            var input = Input.mousePosition;
            Vector3 spawnPos = Camera.main.ScreenToWorldPoint(input);
            spawnPos += Vector3.down * 24;
            Ray ray = Camera.main.ScreenPointToRay(input);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name.Contains(DEFENDER))
                {
                    UnitSpawner(DEFENDER, spawnPos);
                }
                if (hit.transform.name.Contains(ATTACKER))
                {
                    UnitSpawner(ATTACKER, spawnPos);
                }
            }
        }
#endif
    }
    #endregion

    #region Ball
    private Plane PlaneProjection() {
        var filter = AttackerField.GetComponent<MeshFilter>();

        if (filter && filter.mesh.normals.Length > 0)
        {
            normal = filter.transform.TransformDirection(filter.mesh.normals[0]);
        }

        return new Plane(normal, AttackerField.transform.position);
    }

    public void BallSpawner() {
        float spawnRange = 5f;
        Vector3 spawnCenter = AttackerField.transform.position;
        Vector3 randomPointOnPlane = PlaneProjection().ClosestPointOnPlane(spawnCenter + spawnRange * Random.insideUnitSphere);
        randomPointOnPlane = Vector3.up * 2f + randomPointOnPlane;

        Instantiate(BallPrefab, randomPointOnPlane, Quaternion.identity);
    }
    #endregion

    #region Unit
    private void InitUnitPool() {
        for (int i = 0; i < MaxUnitPool; i++)
        {
            GameObject obj = Instantiate(UnitPrefab, UnitPoolContainer);
            obj.SetActive(false);
            UnitPool.Add(obj);
        }
    }

    private GameObject UnitSpawner(string _type, Vector3 _pos) {
        foreach (GameObject Unit in UnitPool)
        {
            if (!Unit.activeInHierarchy)
            {
                if (_type == "Attacker" && Energy.Instance.IsAtkEnergyAvailable())
                {
                    Unit.transform.name = _type+"Unit";
                    Unit.transform.position = _pos;
                    Unit.transform.rotation = Quaternion.identity;
                    Unit.SetActive(true);
                    Energy.Instance.UseEnergyPoints(Type.Attacker);
                    return Unit;
                }
                if (_type == "Defender" && Energy.Instance.IsDefEnergyAvailable())
                {
                    Unit.transform.name = _type+"Unit";
                    Unit.transform.position = _pos;
                    Unit.transform.rotation = Quaternion.identity;
                    Unit.SetActive(true);
                    Energy.Instance.UseEnergyPoints(Type.Defender);
                    return Unit;
                }
            }
        }
        return null;
    }

    public void ReturnToPool(GameObject obj) {
        obj.transform.position = UnitPoolContainer.transform.position;
        obj.transform.rotation = Quaternion.identity;
        obj.SetActive(false);
    }

    public void RefreshPool() {
        foreach (GameObject unit in UnitPool)
        {
            ReturnToPool(unit);
        }
    }
    #endregion

}

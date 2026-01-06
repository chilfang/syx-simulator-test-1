using Unity.Mathematics;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    GameObject pathfindingGoalpost;
    public Vector2Int goalPosition = Vector2Int.zero;

    public Vector3Int currentCell = Vector3Int.zero;

    TilesManager tilesManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goalPosition = WorldPositionToGridPosition(transform.position);

        pathfindingGoalpost = GameObject.Find("Pathfinding Goalpost");

        tilesManager = GameObject.Find("TilesManager").GetComponentInChildren<TilesManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate () {
        
    }

    public void SetPositionGoal(Vector2Int goalPosition) {
        this.goalPosition = goalPosition;
    }

    private Vector2Int WorldPositionToGridPosition(Vector3 vector) {
        return new Vector2Int((int) vector.x, (int) vector.y);
    }
}

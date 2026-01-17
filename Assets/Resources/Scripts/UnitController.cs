using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    //accesibles
    private GameObject pathfindingGoalpost;
    private TilesManager tilesManager;

    //system
    public Vector3Int goalTilePosition = Vector3Int.zero;

    private Vector3 _currentPosition;
    public Vector3 currentPosition {
        get { return _currentPosition; }
        set {
            _currentPosition = value;
            transform.position = _currentPosition + new Vector3(0.5f, 0.5f);
        }
    }

    public Stack<PathfindingNode> intermediateSteps = new();

    //technical stats
    public TileInfo parentTile;
    public Vector3 direction = Vector3.zero;
    public TileEnum[] inaccessibleTiles = {
        TileEnum.Red
    };

    //rpg stats
    public float speedModifier = 5f;
    public string characterName = "Bob";
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPosition = WorldPositionToGridPosition(transform.position);

        pathfindingGoalpost = GameObject.Find("Pathfinding Goalpost");

        tilesManager = GameObject.Find("TilesManager").GetComponentInChildren<TilesManager>();

        setParentTile(WorldPositionToGridPosition(transform.position));

        Debug.Log(
            new PathfindingNode(Vector3Int.one, Vector3Int.zero).Equals(
                new PathfindingNode(Vector3Int.one, Vector3Int.zero, new PathfindingNode(Vector3Int.zero, Vector3Int.zero))
                ));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate () {
        if (intermediateSteps.Count > 0) {
            PathfindingNode currentNode = intermediateSteps.Peek();
            var distanceFromTarget = (currentNode.Position - currentPosition);
            direction = distanceFromTarget.normalized;
            var positionOffset = direction * Time.deltaTime * speedModifier;

            if (positionOffset.magnitude > distanceFromTarget.magnitude) {
                currentPosition = currentNode.Position;
            }
            else {
                currentPosition += positionOffset;
            }

            if (currentPosition == currentNode.Position) {
                //Debug.Log($"{characterName} ({name}) arrived at {currentNode.Position}");
                setParentTile(currentNode.Position);

                intermediateSteps.Pop();
            }
        }
    }

    public void setParentTile(Vector3Int parentTilePosition) {
        TileInfo tileInfo = tilesManager.GetTileInfo(parentTilePosition);

        tileInfo.addUnitToTile(this);
        parentTile?.removeUnitFromTile(this);

        parentTile = tileInfo;
    }

    public void setGoalPosition(Vector3Int goalPosition) {
        if (goalTilePosition == goalPosition) { return; }

        //Debug.Log(!isTileValid(goalTilePosition));
        if (!isTileValid(goalPosition)) {
            Debug.Log($"Tile {goalPosition} not valid");
            return; 
        }

        this.goalTilePosition = goalPosition;
        GenerateIntermediateSteps();
    }

    private Vector3Int WorldPositionToGridPosition(Vector3 vector) {
        return Vector3Int.FloorToInt(vector);
    }

    private void GenerateIntermediateSteps() {
        Debug.Log("Generating Steps");

        List<PathfindingNode> openList = new() { new PathfindingNode(goalTilePosition, parentTile.position) };
        HashSet<Vector3Int> closedList = new();

        Debug.Log(openList[0]);

        PathfindingNode finalNode = null;

        TilePlacer tilePlacer = GameObject.Find("Tilemap").GetComponent<TilePlacer>();
        tilePlacer.SetBrushToGreen(true);

        int loopCount = 0;
        while (openList.Count > 0) {
            loopCount++;
            if (loopCount > 500) { 
                Debug.Log("Too many loops");

                break;
            }

            PathfindingNode currentNode = openList.OrderBy(node => node.DistanceValue).First();

            //Debug.Log($"{loopCount}, {openList.Count}: {currentNode}");

            if (currentNode.Position == goalTilePosition) {
                finalNode = currentNode;
                break;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.Position);

            foreach (var direction in PathfindingNode.ValidDirections) {
                Vector3Int newPosition = currentNode.Position + direction;
                //PathfindingNode newNode = new PathfindingNode(goalTilePosition, newPosition, currentNode);

                if (closedList.Contains(newPosition)) {
                    continue;
                }

                if (!isTileValid(newPosition)) {
                    continue;
                }

                if (!openList.Contains(currentNode)) {
                    //Debug.Log($"{!openList.Contains(currentNode)}, {currentNode.Position}");

                    //foreach (PathfindingNode node in openList) {
                    //    if (node.Position == currentNode.Position) {
                    //        Debug.Log($"{node.Position}, {currentNode.Position}, {node.GetHashCode()}, {currentNode.GetHashCode()}, {node == currentNode}, {node.Equals(currentNode)}");
                    //    }
                    //}
                    openList.Add(new PathfindingNode(goalTilePosition, newPosition, currentNode));
                }

                tilePlacer.PaintTileAt(currentNode.Position);
            }
        }

        if (finalNode != null) {
            intermediateSteps = finalNode.CollapseNodes();
        }



    }

    public bool isTileValid(Vector3Int vector) {
        TileEnum tileType = tilesManager.GetTileInfo(vector).tileType;
        
        foreach (var type in inaccessibleTiles) {
            if (tileType == type) {
                return false;
            }
        }

        return true;
    }
}

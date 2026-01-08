using System;
using System.Collections;
using System.Collections.Generic;
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
                Debug.Log($"{characterName} ({name}) arrived at {currentNode.Position}");
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
        //intermediateSteps.Enqueue(new PathfindingNode(new Vector3(13, -1), goalTilePosition));
        Debug.Log("Generating Steps");

        Dictionary<Vector3Int, PathfindingNode> allNodes = new() {
            { parentTile.position, new PathfindingNode(goalTilePosition, parentTile.position) }
        };


        LinkedList<PathfindingNode> bestNewNodes = new();
        bestNewNodes.AddFirst(allNodes[parentTile.position]);

        LinkedListNode<PathfindingNode> currentNodeInList = bestNewNodes.First;

        PathfindingNode previousNode = currentNodeInList.Value;
        PathfindingNode finalNode = null;

        int currentBestDistanceValue = previousNode.GetDistanceValue();

        int loopcount = 0;
        while (finalNode == null) {
            loopcount++;
            if (loopcount == 50) {
                Debug.Log("Too many loops");
                break;
            }

            bool newBestFound = false;
            Debug.Log("new loop");
            foreach (Vector3Int searchDirection in PathfindingNode.ValidDirections) {
                Vector3Int newPosition = previousNode.Position + searchDirection;

                if (!isTileValid(newPosition)) { continue; }

                PathfindingNode newNode = new PathfindingNode(goalTilePosition, newPosition, previousNode);


                if (newNode.GetDistanceValue() < currentBestDistanceValue) {
                    currentBestDistanceValue = newNode.GetDistanceValue();
                    bestNewNodes.Clear();
                    bestNewNodes.AddFirst(newNode);
                    currentNodeInList = bestNewNodes.First;
                    newBestFound = true;
                }
                else if (newNode.GetDistanceValue() == currentBestDistanceValue) {
                    bestNewNodes.AddLast(newNode);
                }

                if (allNodes.ContainsKey(newPosition)) {
                    if (newNode.GetDistanceValue() < allNodes[newPosition].GetDistanceValue()) {
                        allNodes[newPosition] = newNode;
                    }
                }
                else {
                    allNodes.Add(newPosition, newNode);
                }

                if (newPosition == goalTilePosition) {
                    Debug.Log(bestNewNodes.Count);

                    bestNewNodes.Remove(newNode);
                    finalNode = newNode;

                    Debug.Log($"Final node found {newPosition}");
                }
            }

            if (allNodes.Count > 100) {
                Debug.Log("Too many pathfinding nodes");
                bestNewNodes.Clear();
                break;
            }

            if (!newBestFound) {
                if (currentNodeInList.Next == null) {
                    previousNode = previousNode.PreviousNode;

                    currentBestDistanceValue = previousNode.GetDistanceValue();
                    bestNewNodes.Clear();
                    bestNewNodes.AddFirst(previousNode);
                    currentNodeInList = bestNewNodes.First;
                } else {
                    currentNodeInList = currentNodeInList.Next; 
                }
            }

            if (finalNode == null) {
                previousNode = currentNodeInList.Value;
            }
        }

        intermediateSteps = finalNode.CollapseNodes();
        
    }

    public bool isTileValid(Vector3Int vector) {
        return !(tilesManager.GetTileInfo(vector).tileType == TileEnum.Red);
    }
}

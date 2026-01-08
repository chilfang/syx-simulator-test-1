using System;
using Unity.VectorGraphics;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class GUIController : MonoBehaviour
{
    public enum BrushType {
        None,
        Tiles,
        PathfindingFlag,
        Units,
        PathfindingInfo
    }

    public bool holdingClick = false;
    public BrushType CurrentBrush = BrushType.Tiles;

    GameObject gridVisualizer;
    GameObject pathfindingGoalPost;
    GameObject pathfindingInfoPost;

    GameObject grid;
    Tilemap tilemap;

    TilePlacer tilePlacer;
    TilesManager tilesManager;

    UnitController targetUnit;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridVisualizer = GameObject.Find("GridVisualizer");
        gridVisualizer.GetComponent<SpriteRenderer>().enabled = true;
        gridVisualizer.SetActive(false);

        pathfindingGoalPost = GameObject.Find("Pathfinding Goalpost");
        pathfindingGoalPost.SetActive(false);
        pathfindingInfoPost = GameObject.Find("Pathfinding Info Post");
        pathfindingInfoPost.SetActive(false);

        grid = GameObject.Find("Grid");
        tilemap = grid.GetComponentInChildren<Tilemap>();
        tilePlacer = tilemap.GetComponent<TilePlacer>();

        tilesManager = GameObject.Find("TilesManager").GetComponent<TilesManager>();

        targetUnit = GameObject.Find("BlackCircle32_0").GetComponent<UnitController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (holdingClick) {
            if (!EventSystem.current.IsPointerOverGameObject()) { 
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector3Int tilePosition = tilemap.WorldToCell(mousePosition);

                switch (CurrentBrush) {
                    case BrushType.None:
                        break;

                    case BrushType.Tiles:
                        tilePlacer.PaintTileAt(tilePosition);
                        break;

                    case BrushType.PathfindingFlag:
                        targetUnit.setGoalPosition(tilePosition);
                        MovePathfindingGoalPostToPosition(tilePosition);
                        break;

                    case BrushType.Units:
                        break;

                    case BrushType.PathfindingInfo:
                        DisplayPathfindingInfoForTile(tilePosition);
                        break;

                    default:
                        throw new Exception($"Unknown brush type: {CurrentBrush}");
                }
            }
        }
    }

    //--------------------------------[[ Functions ]]--------------------------------
    public void MovePathfindingGoalPostToPosition (Vector3Int vector) {
        if (pathfindingGoalPost.transform.position == vector) { return; }
        pathfindingGoalPost.transform.position = vector;

        Debug.Log(tilesManager.GetTileInfo(vector));
    }

    public void DisplayPathfindingInfoForTile(Vector3Int vector) {
        if (pathfindingInfoPost.transform.position == vector) { return; }

        pathfindingInfoPost.transform.position = vector;

        Debug.Log(tilesManager.GetTileInfo(vector));

        foreach (PathfindingNode node in targetUnit.intermediateSteps) {
            if (node.Position == vector) {
                Debug.Log(node);
                break;
            }
        }

    }

    //--------------------------------[[ Input ]]--------------------------------
    public void OnClick (InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            holdingClick = true;
        }
        else if (context.phase == InputActionPhase.Canceled) {
            holdingClick = false;
        }
    }

    //--------------------------------[[ Options ]]--------------------------------

    public void SetGridVisualizerActive(bool active) {
        gridVisualizer.SetActive(active);
    }

    //--------------------------------[[ Brushes ]]--------------------------------
    private void SetBrushTo (BrushType brushType, bool active) {
        if (active) {
            CurrentBrush = brushType;
        } else {
            CurrentBrush = BrushType.None;
        }
    }
    public void SetTileBrushActive(bool active) {
        SetBrushTo(BrushType.Tiles, active);
    }

    public void SetPathfindingGoalpostBrushActive (bool active) {
        SetBrushTo(BrushType.PathfindingFlag, active);

        pathfindingGoalPost.SetActive(active);
    }

    public void SetUnitsBrushActive (bool active) {
        SetBrushTo(BrushType.Units, active);
    }

    public void SetPathfindingInfoBrushActive (bool active) {
        SetBrushTo(BrushType.PathfindingInfo, active);

        pathfindingGoalPost.SetActive(active);
        pathfindingInfoPost.SetActive(active);
    }


}

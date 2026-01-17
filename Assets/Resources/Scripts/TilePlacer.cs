using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

public class TilePlacer : MonoBehaviour
{
    //--------------------------------[[ Variables ]]--------------------------------


    GameObject grid;
    Tilemap tilemap;
    GameObject mainGUI;

    GUIController guiController;
    TilesManager tilesManager;

    public TileEnum TileBrush = TileEnum.Red;

    //--------------------------------[[ Game Engine ]]--------------------------------
    void Start ()
    {
        grid = GameObject.Find("Grid");
        tilemap = grid.GetComponentInChildren<Tilemap>();
        mainGUI = GameObject.Find("MainGUI");

        guiController = mainGUI.GetComponent<GUIController>();
        tilesManager = GameObject.Find("TilesManager").GetComponent<TilesManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //--------------------------------[[ Functions ]]--------------------------------

    public void PaintTileAt(Vector3Int position) {
        TileInfo tileInfo = tilesManager.GetTileInfo(position);
        TileEnum newTileType = TileBrush;
        Tile newTile = tilesManager.ConnvertTileTypeToTile[newTileType];

        tilemap.SetTile(position, newTile);

        tileInfo.tileType = newTileType;
        tileInfo.tile = newTile;
    }

    //--------------------[[ Brush switching ]]--------------------
    public void SetBrushToRed(bool active) {
        if (active) {
            TileBrush = TileEnum.Red;
        }
    }
    public void SetBrushToGreen (bool active) {
        if (active) {
            TileBrush = TileEnum.Green;
        }
    }
    public void SetBrushToWhite (bool active) {
        if (active) {
            TileBrush = TileEnum.White;
        }
    }
}

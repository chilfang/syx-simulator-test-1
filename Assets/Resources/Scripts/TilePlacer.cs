using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Tilemaps;

public class TilePlacer : MonoBehaviour
{
    //--------------------------------[[ Variables ]]--------------------------------

    public Dictionary<TileEnum, Tile> Tiles = new Dictionary<TileEnum, Tile>();

    GameObject grid;
    Tilemap tilemap;
    GameObject mainGUI;

    GUIController guiController;

    public TileEnum TileBrush = TileEnum.Red;

    //--------------------------------[[ Game Engine ]]--------------------------------
    void Start ()
    {
        grid = GameObject.Find("Grid");
        tilemap = grid.GetComponentInChildren<Tilemap>();
        mainGUI = GameObject.Find("MainGUI");

        guiController = mainGUI.GetComponent<GUIController>();

        Tiles[TileEnum.Red] = Resources.Load<Tile>("Tilemap/Palletes/Pallete1/RedSquare32_0");
        Tiles[TileEnum.Green] = Resources.Load<Tile>("Tilemap/Palletes/Pallete1/GreenSquare32_0");
        Tiles[TileEnum.White] = Resources.Load<Tile>("Tilemap/Palletes/Pallete1/WhiteSquare32_0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //--------------------------------[[ Functions ]]--------------------------------

    public void PaintTileAt(Vector3Int position) {
        tilemap.SetTile(position, Tiles[TileBrush]);
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

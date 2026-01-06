using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public enum TileEnum {
    None = -1,
    Red = 0,
    Green = 1,
    White = 2
}

public class TilesManager : MonoBehaviour
{
    private Dictionary<Vector3Int, TileInfo> TileInfoDictionary = new Dictionary<Vector3Int, TileInfo>();

    public TileInfo GetTileInfo(Vector3Int vector) {
        if (TileInfoDictionary.ContainsKey(vector)) {
            return TileInfoDictionary[vector];
        }
        else {
            TileInfo newTileInfo = new(vector);
            TileInfoDictionary.Add(vector, newTileInfo);

            return newTileInfo;
        }


    }

    private void Start () {

    }
}

public class TileInfo {
    private static readonly Dictionary<string, TileEnum> tileNameToEnumConversion = new() {
        { "WhiteSquare32_0", TileEnum.White },
        { "RedSquare32_0", TileEnum.Red },
        { "GreenSquare32_0", TileEnum.Green },
    };

    public TileEnum tileType = TileEnum.None;
    public Tile tile;
    public HashSet<UnitController> unitsOnTile = new HashSet<UnitController>();
    public Vector3Int position;

    public TileInfo(Vector3Int vector) {
        Debug.Log($"Creating TileInfo for {vector}");

        Tilemap tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();
        position = vector;
        
        tile = tilemap.GetTile<Tile>(vector);

        if (tile == null) { return; } 

        tileType = tileNameToEnumConversion[tile.sprite.name];
        
    }

    public override string ToString () {
        return $"Tileinfo: (Position: {position}, TileType: {tileType}, Units on Tile: {unitsOnTile.Count})";
    }
}

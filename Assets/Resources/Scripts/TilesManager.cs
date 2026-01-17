using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEditor.U2D;
using UnityEditorInternal;
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
    public Dictionary<TileEnum, Tile> ConnvertTileTypeToTile = new Dictionary<TileEnum, Tile>();
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
        ConnvertTileTypeToTile[TileEnum.Red] = Resources.Load<Tile>("Tilemap/Palletes/Pallete1/RedSquare32_0");
        ConnvertTileTypeToTile[TileEnum.Green] = Resources.Load<Tile>("Tilemap/Palletes/Pallete1/GreenSquare32_0");
        ConnvertTileTypeToTile[TileEnum.White] = Resources.Load<Tile>("Tilemap/Palletes/Pallete1/WhiteSquare32_0");
    }
}

public class TileInfo {
    private static readonly Dictionary<string, TileEnum> tileNameToEnumConversion = new() {
        { "WhiteSquare32_0", TileEnum.White },
        { "RedSquare32_0", TileEnum.Red },
        { "GreenSquare32_0", TileEnum.Green },
    };

    private static readonly Tilemap tilemap = GameObject.Find("Grid").GetComponentInChildren<Tilemap>();

    public TileEnum tileType = TileEnum.None;
    public Tile tile;
    public HashSet<UnitController> unitsOnTile = new();
    public Vector3Int position;

    public TileInfo(Vector3Int vector) {
        Debug.Log($"Creating TileInfo for {vector}");

        position = vector;
        
        tile = tilemap.GetTile<Tile>(vector);

        if (tile == null) { return; } 

        tileType = tileNameToEnumConversion[tile.sprite.name];
    }

    public override string ToString () {
        return $"Tileinfo: (Position: {position}, TileType: {tileType}, Units on Tile: {unitsOnTile.Count})";
    }

    public void addUnitToTile (UnitController unit) {
        unitsOnTile.Add(unit);
    }

    public void removeUnitFromTile (UnitController unit) {
        unitsOnTile.Remove(unit);
    }
}

using UnityEngine;
using UnityEngine.Tilemaps;

public class Storage : MonoBehaviour
{
    public Tilemap TilemapWall;
    public Tilemap TilemapColor;
    public WallGrid Grid;
    public Movable Player;

    public static Storage instance;

    private void Awake() => Storage.instance = (Storage.instance == null) ? this : Storage.instance;
}   
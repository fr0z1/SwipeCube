using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class WallGrid : MonoBehaviour
{
    //settings
    private readonly int _skipLinesCount = 13;
    //settings

    [Space(2f)][Header("Tilemap")]
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tile _tile;
    [SerializeField] private Vector2Int _offset;
    [SerializeField] private bool _offsetInFile;

    [Space(2f)][Header("Map")]
    [SerializeField] private string _fileName;
    [SerializeField] private List<string> _mapFile;
    [SerializeField] private Vector2Int _mapSize;
    [SerializeField] private char[,] _map;

    public int _canRecoloring;

    private Vector3 _playerPosition;
    private Movable _player;

    private void Start()
    {
        Storage.instance.Grid = this;
        _player = Storage.instance.Player;
        GetMap();
        GenerateTilemap();
    }
    private void GetMap()
    {
        string[] _file = File.ReadAllLines(Application.streamingAssetsPath + "/Maps/" + _fileName + ".map");

        for (int i = _skipLinesCount; i < _file.Length; i++)
            { _mapFile.Add(_file[i]); }

        var mapX = Convert.ToInt32(_file[0].Remove(0, 7));
        var mapY = Convert.ToInt32(_file[1].Remove(0, 7));

        if (_offsetInFile)
        {
            var offsetX = Convert.ToInt32(_file[3].Remove(0, 9));
            var offsetY = Convert.ToInt32(_file[4].Remove(0, 9));

            _offset = new Vector2Int(offsetX, offsetY);
        }

        var playerX = Convert.ToDouble(_file[6].Remove(0, 18));
        var playerY = Convert.ToDouble(_file[7].Remove(0, 18));
        _playerPosition = new Vector3((float)playerX, (float)playerY, 0);

        var cameraSize = Convert.ToInt32(_file[9].Remove(0, 12));
        Camera.main.orthographicSize = cameraSize;

        var IsSymmetricalMap = Convert.ToBoolean(_file[11].Remove(0, 14));
        if (IsSymmetricalMap) { Camera.main.transform.position = new Vector3(0, 0, -10); }
        else { Camera.main.transform.position = new Vector3(-0.5f, -0.5f, -10); }

        _mapSize = new Vector2Int(mapX, mapY);
        _map = new char[_mapSize.x, _mapSize.y];

        for(int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0; y < _mapSize.y; y++)
            {
                _map[x, y] = _mapFile[y][x];
            }
        }
    }

    private void GenerateTilemap()
    {
        _offset.x += -(_mapSize.x/2);

        for (int x = 0; x < _mapSize.x; x++)
        {
            for (int y = 0 ; y < _mapSize.y; y++)
            {
                if (_map[x, y] == '#')
                    { _tilemap.SetTile(new Vector3Int(x + _offset.x, -y + _offset.y, 0), _tile); }
                else
                    { if (_canRecoloring == 0) { SetPlayerPosition(); } _canRecoloring++; }
            }
        }
    }

    private void SetPlayerPosition()
    {
        _player.transform.position = _playerPosition;
    }
}
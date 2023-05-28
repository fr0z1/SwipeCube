using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movable : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Tilemap _tilemapWall;
    [SerializeField] private Tilemap _tilemapColor;
    [SerializeField] private Tile _tile;

    public Vector2Int InGridPosition;
    public Vector2 _input;

    private WallGrid _grid;
    private int _recolors;
    private Vector3 targetPosition;
    private bool isMove = false;

    private void Start()
    {
        targetPosition = transform.position;
        Storage.instance.Player = this;
        _grid = Storage.instance.Grid;
        _tilemapWall = Storage.instance.TilemapWall;
        _tilemapColor = Storage.instance.TilemapColor;
    }

    private void FixedUpdate()
    {
        if (!isMove)
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");

            if (_input != Vector2.zero)
            {
                targetPosition = FindCell(Vector2Int.RoundToInt(_input));
                targetPosition.z = transform.position.z;
            }
        }

        if (transform.position != targetPosition)
        {
            isMove = true;
            StartCoroutine(Coloring(transform.position));
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed);
        } else { 
            isMove = false; 
        }
    }

    private Vector2 FindCell(Vector2Int direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position,direction, float.PositiveInfinity);
        if (hit.collider)
        {
            if(hit.distance < 1f) { return targetPosition; }
            var cellPosition = _tilemapWall.WorldToCell(hit.point + (new Vector2(direction.x, direction.y) / 2f));
            return (_tilemapWall.CellToWorld(new Vector3Int(cellPosition.x - direction.x, cellPosition.y - direction.y, 0)) + _tilemapWall.tileAnchor);
        }
        return Vector2.zero;
    }

    private IEnumerator Coloring(Vector3 position)
    {
        yield return new WaitForSeconds(0.01f/_speed);
        if (_tilemapColor.GetTile(_tilemapColor.WorldToCell(position)) != _tile)
        {
            _tilemapColor.SetTile(_tilemapColor.WorldToCell(position), _tile);
            _recolors++;
        if (_grid._canRecoloring == _recolors) { Debug.Log("Win!"); }
        }
    }
}
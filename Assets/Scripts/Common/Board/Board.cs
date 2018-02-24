using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour {

    public enum LockType { None, ContentsPositionIs0}
    public delegate void OnTileBeginClick(Board board, Tile tile);
    public delegate void OnTileClicked(Board board, Tile tile);
    public delegate void OnTileClickBlocked(Board board, Tile tile);
    public delegate void OnTileBeginDrag(Board board, Tile tile, PointerEventData eventData);
    public delegate void OnTileDragged(Board board, Tile tile, PointerEventData eventData);
    public delegate void OnTileEndDrag(Board board, Tile tile, PointerEventData eventData);

    [SerializeField]
    private LockType lockType = LockType.None;
    public int boardWidth;
    public int boardHeight;
    public Vector2 tileScale;
    public Vector2 margin;
    public Tile[] tilePrefabs;
    private Tile[,] tiles;
    public OnTileBeginClick onTileBeginClick;
    public OnTileClicked onTileClicked;
    public OnTileClickBlocked onTileClickBlocked;
    public OnTileBeginDrag onTileBeginDrag;
    public OnTileDragged onTileDragged;
    public OnTileEndDrag onTileEndDrag;

    private bool isLocked = false;
    private bool outsideLocked = false;

    public Tile[,] Tiles
    {
        get {
            return tiles;
        }
    }

    public bool IsLocked
    {
        get {
            if (isLocked || outsideLocked)
            {
                return true;
            }
            
            switch (lockType)
            {
                case (LockType.ContentsPositionIs0):
                    { 
                        foreach (Tile tile in tiles)
                        {
                            if (!tile.IsContentCentered())
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }

    public bool IsEmpty
    {
        get
        {
            if (tiles == null)
            {
                return true;
            }

            return !tiles.Cast<Tile>().Any(tile => tile.HasContents);
        }
    }

    public void SetSize(int width, int height)
    {
        boardWidth = width;
        boardHeight = height;
    }

	public void GenerateBoard () {
        tiles = new Tile[boardWidth, boardHeight];
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Tile tile = Instantiate(tilePrefabs[((x + y) % tilePrefabs.Length)], transform);
                tile.transform.localPosition = CalculateTilePosition(x, boardHeight - 1 - y, boardWidth, boardHeight, tileScale, margin);
                tiles[x,y] = tile;
                tile.SetPosition(x, y);

                tile.onTileBeginClick += OnAnyTileBeginClick;
                tile.onTileClicked += OnAnyTileClicked;
                tile.onTileBeginDrag += OnAnyTileBeginDrag;
                tile.onTileDragged += OnAnyTileDragged;
                tile.onTileEndDrag += OnAnyTileEndDrag;

                LeanTween.alpha(tile.gameObject, 1, 0.3f).setFrom(0f).setEase(LeanTweenType.easeInQuad);
            }
        }
	}

    public void Clear()
    {
        if (tiles == null)
        {
            return;
        }

        foreach (Tile tile in tiles)
        {
            if (tile)
            {
                tile.Clear();
            }
        }
    }

    private Vector2 CalculateTilePosition(int xPosition, int yPosition, int boardWidth, int boardHeight, Vector2 size, Vector2 margin)
    {
        return new Vector2(
                CalculateTilePositionForAxis(xPosition, size.x, boardWidth,  margin.x),
                CalculateTilePositionForAxis(yPosition, size.y, boardHeight, margin.y)
            );
    }

    private float CalculateTilePositionForAxis(int cellPosition, float cellSize, int boardSize, float margin)
    {
        return (cellPosition * cellSize) + ((cellPosition) * margin - (((boardSize - 1) * cellSize) + ((boardSize - 1) * margin)) / 2);
    }

    public Tile GetTile(int x, int y)
    {
        return tiles[x, y];
    }

    public Boolean IsWithinBounds(int x, int y)
    {
        return (x < boardWidth && x >= 0 && y < boardHeight && y >= 0);
        
    }

    public Tile GetTileOrNull(int x, int y)
    {
        if (!IsWithinBounds(x, y))
        {
            return null;
        }

        return GetTile(x, y);
    }

    public List<Tile> GetAdjacentTiles(Tile tile, bool includeDiagonals)
    {
        List<Tile> result = new List<Tile>();
        // iterate over all diagonals
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x + y != 0 && (includeDiagonals || Math.Abs(x + y) == 1))
                {
                    Tile adjacentTile = GetTileOrNull(tile.x + x, tile.y + y);
                    if (adjacentTile != null)
                    {
                        result.Add(adjacentTile);
                    }
                }
            }
        }
        return result;
    }

    public List<Tile> GetAdjacentTiles(List<Tile> tiles, bool includeDiagonals)
    {
        return (from tile in tiles
                select GetAdjacentTiles(tile, false))
                .Aggregate((acc, list) => acc.Union(list).ToList())
                .Distinct()
                .Except(tiles)
                .ToList();
    }

    public void OnAnyTileBeginClick(Tile tile)
    {
        if (IsLocked)
        {
            return;
        }

        isLocked = true;
        if (onTileBeginClick != null)
        {
            onTileBeginClick(this, tile);
        }
        isLocked = false;
    }

    public void OnAnyTileClicked(Tile tile)
    {
        if (IsLocked)
        {
            if (onTileClickBlocked != null)
            {
                onTileClickBlocked(this, tile);
            }
            tile.Block();
            return;
        }

        isLocked = true;
        if (onTileClicked != null)
        {
            onTileClicked(this, tile);
        }
        isLocked = false;
    }

    public void OnAnyTileDragged(Tile tile, PointerEventData eventData)
    {
        if (IsLocked)
        {
            return;
        }

        isLocked = true;
        if (onTileDragged != null)
        {
            onTileDragged(this, tile, eventData);
        }
        isLocked = false;
    }

    public void OnAnyTileBeginDrag(Tile tile, PointerEventData eventData)
    {
        if (IsLocked)
        {
            return;
        }

        isLocked = true;
        if (onTileBeginDrag != null)
        {
            onTileBeginDrag(this, tile, eventData);
        }
        isLocked = false;
    }

    public void OnAnyTileEndDrag(Tile tile, PointerEventData eventData)
    {
        if (IsLocked)
        {
            return;
        }

        isLocked = true;
        if (onTileEndDrag != null)
        {
            onTileEndDrag(this, tile, eventData);
        }
        isLocked = false;
    }

    public void Lock()
    {
        outsideLocked = true;
    }

    public void Unlock()
    {
        outsideLocked = false;
    }
}

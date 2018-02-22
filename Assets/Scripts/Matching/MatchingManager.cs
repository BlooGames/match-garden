using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchingManager : MonoBehaviour
{
    [SerializeField]
    private Board board;
    [SerializeField]
    private PieceProvider pieceProvider;
    [SerializeField]
    private int minimumMatches;

    void Start()
    {
        board.onTileClicked += OnTileClicked;
        Play();
    }

    private void Play()
    {
        board.Clear();
        board.GenerateBoard();
        pieceProvider.FillEmptySpaces(board);
    }

    void OnTileClicked(Board board, Tile tile)
    {
        board.Lock();
        StartCoroutine(OnTileClickedCoroutine(board, tile));
    }

    IEnumerator OnTileClickedCoroutine(Board board, Tile tile)
    {
        try
        {
            if (tile.HasContents)
            {
                List<Tile> matchTiles = GetAdjacentMatches(board, tile);
                List<MatchPiece> matches = (from match in matchTiles
                select match.Contents.GetComponent<MatchPiece>())
                .ToList();

                if (matches.Count < minimumMatches)
                {
                    matches.ForEach(match => AnimatorUtils.Trigger(match.gameObject, "NoMatch"));
                    yield break;
                }

                matches.ForEach(match =>
                {
                    AnimatorUtils.Trigger(match.gameObject, "IsMatching");
                });

                yield return matches[0].ProcessMatch(matches, matchTiles, board);

                matchTiles.ForEach(matchedTile =>
                {
                    GameObject piece = matchedTile.PushContents(null);

                    if (!AnimatorUtils.Trigger(piece, "Matched"))
                    {
                        Destroy(piece);
                    }
                });

                yield return new WaitForEndOfFrame();
                MoveTilesDown(board);
                pieceProvider.FillEmptySpaces(board);
            }
        }
        finally
        {
            board.Unlock();
        }
    }

    List<Tile> GetAdjacentMatches(Board board, Tile tile)
    {
        List<Tile> result = new List<Tile>();
        result.Add(tile);
        return GetAdjacentMatchesRecursive(result, board, tile);
    }

    private List<Tile> GetAdjacentMatchesRecursive(List<Tile> result, Board board, Tile tile)
    {
        MatchPiece piece = tile.Contents.GetComponent<MatchPiece>();

        if (piece == null)
        {
            return result;
        }

        List<Tile> adjacentTiles = board.GetAdjacentTiles(tile, false);

        foreach (Tile adjacentTile in adjacentTiles)
        {
            MatchPiece adjacentPiece = adjacentTile.HasContents ? adjacentTile.Contents.GetComponent<MatchPiece>() : null;
            if (adjacentPiece && piece.CanMatch && adjacentPiece.CanMatch &&
                piece.Matches(adjacentPiece) && !result.Contains(adjacentTile))
            {
                result.Add(adjacentTile);
                GetAdjacentMatchesRecursive(result, board, adjacentTile);
            }
        }

        return result;
    }

    void MoveTilesDown(Board board)
    {
        for (int x = 0; x < board.boardWidth; x++)
        {
            for (int y = board.boardHeight - 1; y >= 0; y--)
            {

                Tile floorTile = board.GetTile(x, y);
                if (!floorTile.HasContents)
                {
                    for (int upY = y - 1; upY >= 0; upY--)
                    {
                        Tile fallTile = board.GetTile(x, upY);

                        if (fallTile.HasContents)
                        {
                            floorTile.PushContentsAndMoveToCenter(fallTile.PushContents(null), 0.4f);
                            break;
                        }
                    }
                }
            }
        }
    }
}
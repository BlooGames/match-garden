using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceProvider : MonoBehaviour {

    [System.Serializable]
    public class PieceData {
        [SerializeField]
        private MatchPiece piecePrefab;
        [SerializeField]
        private int chances = 1;
        
        public GameObject PiecePrefab
        {
            get
            {
                return piecePrefab.gameObject;
            }
        }

        public int Chances
        {
            get
            {
                return chances;
            }
        }

        public bool Matches(MatchPiece otherPiece)
        {
            return piecePrefab.Matches(otherPiece);
        }
    }

	[SerializeField]
	private List<PieceData> pieces;
    [SerializeField]
    private float repeatPieceChance;
    [SerializeField]
    private int maximumClump = 4;
    private List<PieceData> weightedPieces;

    void Awake() 
    {
        weightedPieces = GenerateWeightedPieceData(pieces);    
    }

    public void FillEmptySpaces(Board board)
	{
		for (int x = 0; x < board.boardWidth; x++)
        {
            for (int y = 0; y < board.boardHeight; y++)
            {
                Tile tile = board.GetTileOrNull(x, y);
                if (!tile || tile.HasContents)
                {
                    continue;
                }
                tile.PushContentsFromPrefab(ChooseNextPiece(tile, board, pieces, weightedPieces).PiecePrefab);
                LeanTween.moveLocalY(tile.Contents, 0f, 0.5f).setFrom(5f).setEase(LeanTweenType.easeInOutQuad);
                LeanTween.alpha(tile.Contents, 1f, 0.3f).setFrom(0f).setEase(LeanTweenType.easeInOutQuad);
            }
        }
	}

    private PieceData ChooseNextPiece(Tile tile, Board board, List<PieceData> unweightedPieceOptions, List<PieceData> weightedPieceOptions)
    {
        int attempts = 0;
        PieceData result;
        do
        {
            result = ChoosePossibleNextPiece(tile, board, unweightedPieceOptions, weightedPieceOptions, attempts);
            attempts++;
            if (attempts >= 10)
            {
                Debug.Log("Failed to find non-clumping tiles.");
                break;
            }
        } while (IsClumped(tile, board, result));
        return result;
    }

    private PieceData ChoosePossibleNextPiece(Tile tile, Board board, List<PieceData> unweightedPieceOptions, List<PieceData> weightedPieceOptions, int attempts = 0)
    {
        PieceData result = null;

        if (Random.value < repeatPieceChance)
        {
            result = ChooseAdjacentPiece(tile, board, unweightedPieceOptions);
            if (result != null)
            {
                return result;
            }
        }
        int random = Random.Range(0, weightedPieceOptions.Count);
        return weightedPieceOptions[random];
    }

    public bool IsClumped(Tile tile, Board board, PieceData pieceData)
    {
        List<Tile> adjacentTiles = board.GetAdjacentTiles(tile, false);
        int clumpSize = adjacentTiles
            .Where(adjacentTile => {
                return adjacentTile.HasContents && pieceData.Matches(adjacentTile.Contents.GetComponent<MatchPiece>()); 
            })
            .Select(adjacentTile => MatchingManager.Instance.GetAdjacentMatches(board, adjacentTile, true))
            .Aggregate(new List<Tile>(), (acc, list) => acc.Union(list).ToList())
            .Distinct()
            .Count();
        return clumpSize >= maximumClump;
    }

    public PieceData ChooseAdjacentPiece(Tile tile, Board board, List<PieceData> pieceOptions)
    {
        List<Tile> shuffledTiles = board.GetAdjacentTiles(tile, false)
            .OrderBy(a => Random.value)
            .ToList();

        foreach (Tile randomTile in shuffledTiles)
        {
            if (!randomTile.HasContents) continue;

            MatchPiece matchPiece = randomTile.Contents.GetComponent<MatchPiece>();
            foreach (PieceData pieceOption in pieceOptions)
            {
                if (pieceOption.Matches(matchPiece))
                {
                    return pieceOption;
                }
            }
        }

        return null;
    }

    public List<PieceData> GenerateWeightedPieceData(List<PieceData> pieces)
    {
        List<PieceData> result = new List<PieceData>();
        foreach (PieceData piece in pieces)
        {
            result.AddRange(Enumerable.Repeat(piece, piece.Chances));
        }
        return result;
    }
}

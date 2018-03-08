using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchingManager : MonoBehaviour
{
    public delegate void OnScoreChanged(int score, int byAmount);
    public delegate void OnTurnsChanged(int turns, int byAmount, bool specialEvent);
    public delegate void OnEndGame(int score, int level, int highScore, int highLevel, bool beatHighScore);
    [SerializeField]
    private Board board;
    [SerializeField]
    private LevelProvider levelProvider;
    [SerializeField]
    private int minimumMatches;
    [SerializeField]
    private int startingTurns;
    [SerializeField]
    private float waitBeforeProcessingMatch = 0.5f;

    public int Score { get; private set; }

    public int RemainingTurns { get; private set; }
    public OnScoreChanged onScoreChanged;
    public OnTurnsChanged onTurnsChanged;
    public OnEndGame onEndGame;
    public LevelProvider.OnLevelProgressChanged onLevelProgressChanged;

    public static MatchingManager Instance { get; private set; }

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        RemainingTurns = startingTurns;
        Score = 0;
    }

    void Start()
    {
        board.onTileClicked += OnTileClicked;
        levelProvider.onLevelProgressChanged += OnLevelProgressChanged;
        Play();
    }

    private void Play()
    {
        board.Clear();
        board.GenerateBoard();
        levelProvider.Init();
        levelProvider.CurrentLevel.PieceProvider.FillEmptySpaces(board);
        RemainingTurns = startingTurns;
        StartCoroutine(ScrambleBoardIfNecessary(board));
    }

    void OnLevelProgressChanged(MatchingLevel level, int currentProgress, int scoreToNextLevel)
    {
        if (onLevelProgressChanged != null) onLevelProgressChanged(level, currentProgress, scoreToNextLevel);
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
            if (tile.HasContents && RemainingTurns > 0)
            {
                List<Tile> matchTiles = GetAdjacentMatches(board, tile, true);
                List<MatchPiece> matches = (from match in matchTiles
                select match.Contents.GetComponent<MatchPiece>())
                .ToList();

                if (matches.Count < minimumMatches || !matches[0].CanMatch)
                {
                    matches.ForEach(match => AnimatorUtils.Trigger(match.gameObject, "NoMatch"));
                    AudioManager.Instance.PlaySound("NoMatch");
                    yield break;
                }

                AudioManager.Instance.PlaySound("Match");

                ChangeRemainingTurns(-1, false);
                
                matches.ForEach(match =>
                {
                    AnimatorUtils.Trigger(match.gameObject, "IsMatching");
                });

                yield return new WaitForSeconds(waitBeforeProcessingMatch);

                yield return matches[0].ProcessMatch(matches, matchTiles, board);
                ChangeScore(matches[0].ScoreMatch(matches));

                matchTiles.ForEach(matchedTile =>
                {
                    GameObject piece = matchedTile.PushContents(null);

                    if (!AnimatorUtils.Trigger(piece, "Matched"))
                    {
                        Destroy(piece);
                    }
                });

                yield return FillBoard(board);

                if (RemainingTurns <= 0)
                {
                    EndGame();
                }
            }
        }
        finally
        {
            board.Unlock();
        }
    }

    private IEnumerator FillBoard(Board board)
    {
        yield return new WaitForEndOfFrame();
        MoveTilesDown(board);
        levelProvider.CurrentLevel.PieceProvider.FillEmptySpaces(board);

        yield return ScrambleBoardIfNecessary(board);
    }

    public void ChangeRemainingTurns(int byAmount, bool isSpecialEvent)
    {
        RemainingTurns += byAmount;
        if (onTurnsChanged != null) onTurnsChanged(RemainingTurns, byAmount, isSpecialEvent);
    }

    public void ChangeScore(int byAmount)
    {
        Score += byAmount;
        if (onScoreChanged != null) onScoreChanged(Score, byAmount);

    }

    public List<Tile> GetAdjacentMatches(Board board, Tile tile, bool ignoreCanMatch = false)
    {
        List<Tile> result = new List<Tile>();
        result.Add(tile);
        return GetAdjacentMatchesRecursive(result, board, tile, ignoreCanMatch);
    }

    private List<Tile> GetAdjacentMatchesRecursive(List<Tile> result, Board board, Tile tile, bool ignoreCanMatch)
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
            if (adjacentPiece && (ignoreCanMatch || (piece.CanMatch && adjacentPiece.CanMatch)) &&
                piece.Matches(adjacentPiece) && !result.Contains(adjacentTile))
            {
                result.Add(adjacentTile);
                GetAdjacentMatchesRecursive(result, board, adjacentTile, ignoreCanMatch);
            }
        }

        return result;
    }

    private IEnumerator ScrambleBoardIfNecessary(Board board, int attempts = 0)
    {
        if (attempts >= 5)
        {
            foreach (Tile tile in board.Tiles)
            {
                if (tile.HasContents)
                {
                    GameObject contents = tile.PushContents(null);
                    AnimatorUtils.Trigger(contents, "Matched");
                }
            }
            yield return new WaitForSeconds(1.5f);
            yield return FillBoard(board);
        }

        if (BoardHasNoMatches(board))
        {
            yield return new WaitForSeconds(1.5f);
            yield return ScrambleBoard(board);
            yield return ScrambleBoardIfNecessary(board, attempts + 1);
        }
    }

    private bool BoardHasNoMatches(Board board)
    {
        foreach (Tile tile in board.Tiles)
        {
            if (tile.HasContents 
                && tile.Contents.GetComponent<MatchPiece>().CanMatch 
                && GetAdjacentMatches(board, tile).Count >= minimumMatches)
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator ScrambleBoard(Board board)
    {
        List<GameObject> contentsList = new List<GameObject>();
        foreach (Tile tile in board.Tiles)
        {
            contentsList.Add(tile.PushContents(null));
        }

        contentsList = contentsList.OrderBy(contents => Random.value).ToList();

        int i = 0;
        foreach (Tile tile in board.Tiles)
        {
            tile.PushContentsAndMoveToCenter(contentsList[i], 0.7f);
            i++;
        }

        yield return new WaitForSeconds(0.7f);
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

    private void EndGame()
    {
        bool beatHighScore = false;

        if (PlayerPrefs.GetInt("HighScore") < Score)
        {
            PlayerPrefs.SetInt("HighScore", Score);
            PlayerPrefs.SetInt("HighScoreLevel", levelProvider.CurrentLevel.LevelId);
            beatHighScore = true;
        }

        if (onEndGame != null)
        {
            onEndGame(Score, levelProvider.CurrentLevel.LevelId, PlayerPrefs.GetInt("HighScore"), PlayerPrefs.GetInt("HighScoreLevel"), beatHighScore);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelProvider : MonoBehaviour {

    public delegate void OnLevelProgressChanged(MatchingLevel level, int currentProgress, int scoreToNextLevel);

    [SerializeField]
    private List<int> scoresToNextLevel;
    [SerializeField]
    private int startingLevel = 0;

    private int scoreToNextLevel;
    private int currentProgress = 0;

    private Dictionary<int, List<MatchingLevel>> levelMap = null;

    private int currentLevelId;
    public MatchingLevel CurrentLevel { get; private set; }
    public OnLevelProgressChanged onLevelProgressChanged;

    public void Init()
    {
        MatchingManager.Instance.onScoreChanged += OnScoreChanged;
        GetNextLevel(startingLevel);
        StartCoroutine(FinishInit());
    }

    public IEnumerator FinishInit()
    {
        yield return new WaitForEndOfFrame();
        OnScoreChanged(0, 0);
    }

    private void OnScoreChanged(int newScore, int byAmount)
    {
        currentProgress += byAmount;
        if (currentProgress > scoreToNextLevel)
        {
            currentProgress -= scoreToNextLevel;
            GetNextLevel(currentLevelId + 1);
        }
        if (onLevelProgressChanged != null) onLevelProgressChanged(CurrentLevel, currentProgress, scoreToNextLevel);
    }

    private void GenerateLevelMap()
    {
        levelMap = GetComponentsInChildren<MatchingLevel>()
            .GroupBy(level => level.LevelId)
            .ToDictionary(group => group.Key, group => group.ToList());
    }

    private MatchingLevel GetNextLevel(int levelId)
    {
        Debug.Log("LEVEL " + levelId);
        scoreToNextLevel = scoresToNextLevel[levelId];
        CurrentLevel = GetLevel(levelId);
        currentLevelId = levelId;
        return CurrentLevel;
    }

    private MatchingLevel GetLevel(int levelId)
    {
        if (levelMap == null)
        {
            GenerateLevelMap();
        }
        var levels = levelMap[levelId];
        return levels[Random.Range(0, levels.Count)];
    }
}

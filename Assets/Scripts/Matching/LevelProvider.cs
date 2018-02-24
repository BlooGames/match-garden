using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelProvider : MonoBehaviour {

    Dictionary<int, List<MatchingLevel>> levelMap;
    public void GenerateLevelMap()
    {
        levelMap = GetComponentsInChildren<MatchingLevel>()
            .GroupBy(level => level.LevelId)
            .ToDictionary(group => group.Key, group => group.ToList());
    }


    private MatchingLevel GetLevel(int levelId)
    {
        var levels = levelMap[levelId];
        return levels[Random.Range(0, levels.Count)];
    }
}

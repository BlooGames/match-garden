using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

    private static GameState _instance;

    public static GameState instance
    {
        get
        {
            if (!_instance)
            {
                var gameObject = new GameObject("GameState");
                return gameObject.AddComponent<GameState>();
            }
            return _instance;
        }
    }

   private int currentLevelIndex = 1;
   public string currentLevel
    {
        get
        {
            return "level" + currentLevelIndex;
        }
    }

    void Start()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        _instance = this;
    }

    public void FinishLevel()
    {
        currentLevelIndex += 1;
    }
}

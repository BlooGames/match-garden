using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class Level {

    private int _levelWidth = 0;
    private int _levelHeight = 0;

    public int levelWidth {
        get {
            return _levelWidth;
        }
    }
    
    public int levelHeight {
        get {
            return _levelHeight;
        }
    }

    public Level(string levelId)
    {
        string levelData = Resources.Load<TextAsset>(levelId).text;
        string[] rowsTemp = levelData.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        List<string> rows = new List<string>(rowsTemp);
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i][0] == '#')
            {
                Debug.Log("Level " + levelId + " comment: " + rows[i]);
                rows.RemoveAt(i);
                i--;
            }
        }
        _levelHeight = CalculateWidth(rows);
        _levelWidth = CalculateHeight(rows);

        InitialiseArray();
        
        for (int i = 0; i < rows.Count; i++)
        {
            for (int j = 0; j < rows[i].Length; j++)
            {
                StoreValue(j, i, rows[i][j]);
            }
        }
        
    }

    public virtual int CalculateWidth(List<string> rows)
    {
        return rows.Count;
    }

    public virtual int CalculateHeight(List<string> rows)
    {
        return rows.Count > 0 ? rows[0].Length : 0;
    }

    public abstract void InitialiseArray();

    public abstract void StoreValue(int x, int y, char value);

}

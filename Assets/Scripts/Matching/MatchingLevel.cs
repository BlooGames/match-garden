using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingLevel : MonoBehaviour {

    [SerializeField]
    private int levelId;
    public int LevelId
    {
        get
        {
            return levelId;
        }
    }

    [SerializeField]
    private string levelName;
    public string LevelName
    {
        get
        {
            return levelName;
        }
    }

    private PieceProvider pieceProvider;
    public PieceProvider PieceProvider
    {
        get
        {
            if (!pieceProvider)
            {
                pieceProvider = GetComponent<PieceProvider>();
            }
            return pieceProvider;
        }
    }
}

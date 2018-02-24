using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumpPiece : MatchPiece
{
    public override System.Type TileType
    {
        get
        {
            return typeof(StumpPiece);
        }
    }

    public override bool CanMatch
    {
        get
        {
            return false;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePiece : MatchPiece
{
    public override Type TileType
    {
        get
        {
            return typeof(FirePiece);
        }
    }

    public override bool CanMatch
    {
        get
        {
            return true;
        }
    }
}
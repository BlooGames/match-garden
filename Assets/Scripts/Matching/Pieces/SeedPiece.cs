using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPiece : MatchPiece 
{
    public override Type TileType
    {
        get
        {
            return typeof(SeedPiece);
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

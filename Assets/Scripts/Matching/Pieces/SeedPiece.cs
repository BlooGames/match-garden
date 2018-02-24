using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedPiece : MatchPiece 
{
    [SerializeField]
    private MatchPiece flowerPrefab;
    
    public MatchPiece FlowerPrefab
    {
        get
        {
            return flowerPrefab;
        }
    }

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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterPiece : MatchPiece
{

    [SerializeField]
    private float destroyFireWaitTime = 0.7f;
    public override Type TileType
    {
        get
        {
            return typeof(WaterPiece);
        }
    }

    public override bool CanMatch
    {
        get
        {
            return true;
        }
    }

    public override IEnumerator ProcessMatch(List<MatchPiece> matches, List<Tile> matchTiles, Board board)
    {
        bool destroyedFire = false;

        foreach (Tile tile in board.GetAdjacentTiles(matchTiles, false))
        {
            if (!tile.HasContents)
            {
                continue;
            }

            MatchPiece piece = tile.Contents.GetComponent<MatchPiece>();
            if (piece.Matches(typeof(FirePiece)))
            {
                tile.PushContents(null);
                if (!AnimatorUtils.Trigger(piece.gameObject, "DestroyWithWater"))
                {
                    Destroy(piece.gameObject);
                }
                destroyedFire = true;
            }
        }

        if (destroyedFire)
        {
            yield return new WaitForSeconds(destroyFireWaitTime);
        }
    }
}
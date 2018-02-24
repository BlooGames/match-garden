using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterPiece : MatchPiece
{

    [SerializeField]
    private float destroyFireWaitTime = 0.7f;
    [SerializeField]
    private float bloomWaitTime = 0.7f;

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

        var adjacentTiles = board.GetAdjacentTiles(matchTiles, false);
        foreach (Tile tile in adjacentTiles)
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

        bool bloomed = false;

        foreach (Tile tile in adjacentTiles)
        {
            if (!tile.HasContents)
            {
                continue;
            }

            MatchPiece piece = tile.Contents.GetComponent<MatchPiece>();
            if (piece.Matches(typeof(SeedPiece)))
            {
                var flowerPrefab = ((SeedPiece)piece).FlowerPrefab.gameObject;
                tile.PushContentsFromPrefab(flowerPrefab);
                if (!AnimatorUtils.Trigger(piece.gameObject, "Bloom"))
                {
                    Destroy(piece.gameObject);
                }
                bloomed = true;
            }
        }

        if (bloomed)
        {
            yield return new WaitForSeconds(bloomWaitTime);
        }
    }
}
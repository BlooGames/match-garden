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

    [SerializeField]
    private float destroyPieceWaitTime = 1f;

    public override IEnumerator ProcessMatch(List<MatchPiece> matches, List<Tile> matchTiles, Board board)
    {
        bool destroyedPiece = false;
        int stumpsDestroyed = 0;

        foreach (Tile tile in board.GetAdjacentTiles(matchTiles, false))
        {
            if (!tile.HasContents)
            {
                continue;
            }

            MatchPiece piece = tile.Contents.GetComponent<MatchPiece>();
            if (!piece.Matches(typeof(WaterPiece)))
            {
                if (piece.Matches(typeof(StumpPiece)))
                {
                    stumpsDestroyed++;
                }
                tile.PushContents(null);
                if (!AnimatorUtils.Trigger(piece.gameObject, "DestroyWithFire"))
                {
                    Destroy(piece.gameObject);
                }
                destroyedPiece = true;
            }
        }

        if (destroyedPiece)
        {
            if (stumpsDestroyed > 0)
            {
                MatchingManager.Instance.ChangeScore(stumpsDestroyed * 5);
            }

            yield return new WaitForSeconds(destroyPieceWaitTime);
        }
    }
}
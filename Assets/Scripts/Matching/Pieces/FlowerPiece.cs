using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPiece : MatchPiece {
    public override System.Type TileType
    {
        get
        {
            return typeof(FlowerPiece);
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
        MatchingManager.Instance.ChangeRemainingTurns(Mathf.Min(matches.Count - 1, 9), true);
        yield return null;
    }
}

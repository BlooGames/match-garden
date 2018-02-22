using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MatchPiece : MonoBehaviour 
{

    public abstract System.Type TileType {
        get;
    }

    public abstract bool CanMatch {
        get;
    }

    public virtual IEnumerator ProcessMatch(List<MatchPiece> matches, List<Tile> matchTiles, Board board)
    {
        yield break;
    }

    public bool Matches(MatchPiece otherPiece)
    {
        return TileType.Equals(otherPiece.TileType);
    }

    public bool Matches(System.Type otherType)
    {
        return TileType.Equals(otherType);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}

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

    public virtual int ScoreMatch(List<MatchPiece> matches)
    {
        //// The first piece in a match is worth zero, and each one after that is worth one more than the last.
        //int score = 0;
        //for (int i = 0; i < matches.Count; i++)
        //{
        //    score += i;
        //}
        int score = 0;
        int multiplier = 0;
        for (int i = 0; i < matches.Count; i++)
        {
            if (i % 2 == 0)
            {
                multiplier++;
            }
            score += multiplier;
        }
        return score;
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

    public void BringToFront()
    {
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.sortingOrder = 99;
        }
    }
}

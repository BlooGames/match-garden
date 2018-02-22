using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleManager : MonoBehaviour {

    public delegate void OnWon();
    public delegate void OnFinished();
    public OnWon onWon;
    public OnFinished onFinished;
    protected bool hasWon = false;

    private string _levelId;

    protected abstract string puzzlePrefix {
        get;
    }

    public string levelId {
        get {
            return puzzlePrefix + _levelId;
        }
    }


	public virtual bool canRestart {
        get
        {
            return false;
        }
    }

    public void Play(string levelId)
    {
        _levelId = levelId;
        Play();
    }

    protected abstract void Play();

    public virtual void Restart()
    {
        if (hasWon)
        {
            return;
        }
        Play();
    }

    public void Win(float afterSeconds = 0f)
    {
        hasWon = true;
        StartCoroutine(WinAfterSeconds(afterSeconds));
    }

    public IEnumerator WinAfterSeconds(float afterSeconds)
    {
        yield return new WaitForSeconds(afterSeconds);

        if (onWon != null)
        {
            onWon();
        }
    }
}

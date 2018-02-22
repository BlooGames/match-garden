using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour {

    public enum PuzzleType { None, Matching, Jigsaw, Fullhouse, Boulder } 
    [System.Serializable]
    public class PuzzleId
    {
        public PuzzleType puzzleType;
        public string id;
    }

    [SerializeField]
    private PuzzleId[] puzzleIds;
    [SerializeField]
    private RestartButton restartButton;
    private static readonly string puzzlePath = "PuzzlePrefabs/"; 
    private static readonly Dictionary<PuzzleType, string> puzzleObjectMap = new Dictionary<PuzzleType, string>() {
        { PuzzleType.Matching, "Matching" },
        { PuzzleType.Jigsaw, "Jigsaw" },
        { PuzzleType.Fullhouse, "Fullhouse" },
        { PuzzleType.Boulder, "Boulder" },
    };

    private int index = 0;
    private PuzzleManager currentPuzzle;

	// Use this for initialization
	void Start () {
        StartCoroutine(PlayPuzzle());
    }

    IEnumerator PlayPuzzle()
    {
        GameObject prefab = Resources.Load<GameObject>(puzzlePath + puzzleObjectMap[puzzleIds[index].puzzleType]);
        currentPuzzle = Instantiate(prefab).GetComponent<PuzzleManager>();
        currentPuzzle.onWon += OnWon;
        yield return new WaitForEndOfFrame();
        currentPuzzle.Play(puzzleIds[index].id);
        restartButton.SetVisible(currentPuzzle.canRestart);
    }

    public void RestartPuzzle()
    {
        currentPuzzle.Restart();
    }

    void OnWon()
    {
        index++;
        StartCoroutine(DestroyAfterWait(currentPuzzle.gameObject, 2f));
        if (index < puzzleIds.Length)
        {
            StartCoroutine(PlayPuzzle());
        }
    }

    IEnumerator DestroyAfterWait(GameObject puzzle, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (puzzle)
        {
            Destroy(puzzle);
        }
    }
}

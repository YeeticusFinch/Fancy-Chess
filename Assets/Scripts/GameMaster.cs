using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public Camera cam;
    public Board board;

    public static float spaceBetweenX = 1.4f;
    public static float spaceBetweenY = 1.4f;
    public static float spaceBetweenZ = 2.6f;

    public static bool debug = false;
    public static bool selectTileForNewPiece = false;

    public static ChessPiece selectedPiece = null;

    public static bool canabalism = false;
    public static bool bowling = true;

    public static GameMaster instance;

    private bool allowingMove = false;

    public static bool gameRunning = false;
    public static bool gameStarted = false;

    public static int[] displayDims = { 0, 1, 2, 3, 4, 5 };
    public static List<int> displayLoc = new List<int>();

    public static int usedTurns = 0;

    public GameObject[] disableOnPlay;
    public GameObject[] enableOnPlay;

    public static bool rachel = false;

    public static string winner = null;

    public static bool whiteAI = false;
    public static bool blackAI = false;

    public static int availableTurns()
    {
        return instance.board.whiteTurn ? instance.board.whiteTurnsPerRound : instance.board.blackTurnsPerRound;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        foreach (GameObject o in enableOnPlay)
            o.SetActive(false);
    }

    public static void Rachel(TextMesh buttonText)
    {
        rachel = !rachel;
        buttonText.color = !rachel ? new Color32(0, 255, 23, 255) : new Color32(255, 10, 10, 255);
    }

    public static void Rachel()
    {
        Rachel(GameObject.FindGameObjectWithTag("RachelButton").GetComponent<TextMesh>());
    }

    public static void NoRachel()
    {
        GameObject.FindGameObjectWithTag("RachelButton").GetComponent<TextMesh>().color = new Color32(0, 255, 23, 255);
        rachel = false;
    }

    public static bool pieceSpin = true;
    
    void FixedUpdate()
    {
        if (!gameRunning) return;
        else if (!gameStarted)
        {
            if (whiteAI && blackAI)
            {
                cam.GetComponent<MainCamera>().camSwap = false;
                pieceSpin = false;
            }
            for (int i = 0; i < board.size.Count; i++)
                displayLoc.Add(0);
            foreach (GameObject o in disableOnPlay)
                o.SetActive(false);
            foreach (GameObject o in enableOnPlay)
                o.SetActive(true);
            gameStarted = true;
            board.Init();
            cam.gameObject.GetComponent<MainCamera>().Init();
            GameObject[] yeet = GameObject.FindGameObjectsWithTag("Piece");
            foreach (GameObject o in yeet)
            {
                o.GetComponent<ChessPiece>().Init();
            }
        }
        if (!board.canMove && !allowingMove) StartCoroutine(allowMove());
    }

    IEnumerator allowMove()
    {
        allowingMove = true;
        yield return new WaitForSecondsRealtime(0.5f);
        board.canMove = true;
        allowingMove = false;
    }
}

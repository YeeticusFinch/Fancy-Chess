using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public Camera cam;
    public Board board;

    public static ChessPiece selectedPiece = null;

    public static bool canabalism = false;
    //public static bool bowling = false;

    public static GameMaster instance;

    private bool allowingMove = false;

    public static bool gameRunning = false;
    public static bool gameStarted = false;

    public static int[] displayDims = { 0, 1, 2 };

    public GameObject[] disableOnPlay;
    public GameObject[] enableOnPlay;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        foreach (GameObject o in enableOnPlay)
            o.SetActive(false);
    }
    
    void FixedUpdate()
    {
        if (!gameRunning) return;
        else if (!gameStarted)
        {
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

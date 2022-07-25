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

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    
    void FixedUpdate()
    {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public string type;
    public List<int> location;
    //public List<int> blackStartLocation;
    public bool white;    // True if White, False if Black
    public Board board;

    public bool didDoubleMove = false;  // for pawns, to see if they can be hit by en-passent
    public bool hasMoved = false;
    public bool vip = false;

    public static Move[] KING = { // check and checkmate needs to be hardcoded
        new Move(1),   // left right up down
        new Move(0, 1), // diagonals
    };

    public static Move[] QUEEN = {
        new Move(-1),
        new Move(0, -1),
    };

    public static Move[] BISHOP = {
        new Move(0, -1) // only diagonals
    };

    public static Move[] KNIGHT = { // only catch is the diagonal needs to have a component vector in the same direction as the lateral (prob needs to be hardcoded)
        new Move(1, 1, false, true, true) // move one space laterally, and one space diagonally, must move 1 forward, can jump over pieces
    };

    public static Move[] ROOK = {
        new Move(-1) // move laterally
    };

    public static Move[] PAWN = { // hardest one, a lot probably needs to be hardcoded
        new Move(1, 0, true, false, false, false), // moves forward, can't consume
        new Move(0, 1, true, false, false, true, true), // moves diagonally to consume, can only be used to consume
        new Move(2, 0, true, true, false, false, false, true) // moves forward twice as a first move
    };

    public static Move[] UNICORN = {
        new Move(2, 0, false, true, true),
        new Move(0, 1, false, true, true)
    };

    public static Move[] ELEPHANT = {
        new Move(1, 0, false, false, true),
        new Move(0, 2, false, false, true)
    };

    public static Move[] BUFFALO = { // same hard coding as the knight, basically a super knight
        new Move(1, 1, false, true, true),
        new Move(1, 2, false, true, true),
        new Move(2, 1, false, true, true),
    };

    public static Move[] PRINCESS = { // combines my two fav pieces from traditional chess (knight and bishop)
         new Move(1, 1, false, true, true), // knight moves (with jumping)
         new Move(0, -1) // bishop moves (no jumping)
    };

    public static Move[] EMPRESS = {
        new Move(1, 1, false, true, true), // knight moves (with jumping)
        new Move(-1) // rook moves
    };

    public static Move[] DABBABA = {
        new Move(2, 0, false, true, true) // jumps to spaces laterally
    };

    public static Move[] FERZ = {
        new Move(0, 1) // can only move one space diagonally
    };

    public static Move[] ALPIL = {
        new Move(0, 2, false, true, true) // jumps two spaces diagonally
    };

    public static Move[] CAMEL = {
        new Move(2, 1, false, true, true) // long-jumping knight
    };

    public static Move[] ZEBRA = {
        new Move(1, 2, false, true, true) // another long-jumping knight
    };
    
    [System.Serializable]
    public struct Move
    {
        public Move(int lateral, int diagonal = 0, bool onlyForward = false, bool force = false, bool jump = false, bool canConsume = true, bool onlyForConsume = false, bool firstMove = false)
        {
            this.force = force;
            this.jump = jump;
            this.canConsume = canConsume;
            this.onlyForward = onlyForward;
            this.onlyForConsume = onlyForConsume;
            this.lateral = lateral;
            this.diagonal = diagonal;
            this.firstMove = firstMove;
        }
        // put 0 for nothing, -1 for as much as you want, if a specific is inputted and force is false, you can move up to that number
        public bool canConsume; // can this move result in the consumption of another piece
        public bool force; // forcing a certain amount (ie knights)
        public bool jump; // can jump over other (ie knights)
        public bool onlyForConsume; // this move can only be used to consume another piece (ie pawns)
        public bool onlyForward; // forward movement (ie pawns) for n-dimensional chess, movement in any component of the direction vector towards the start position of the other colour
        public int lateral; // movement paralel to any coordinate axis (ie rooks, queens, kings)
        public int diagonal; // movement across any 45 degree diagonal (ie bishops, queens)
        public bool firstMove; // can only use this move if it's the first move (pawns)
    }

    public List<Move> moves = new List<Move>();
    
    void Start()
    {
        while (location.Count < board.size.Count)
            location.Add(0);
        while (location.Count > board.size.Count)
            location.Remove(location.Count - 1);
        if (!white)
        {
            //if (blackStartLocation != null && blackStartLocation.Count > 0) location = blackStartLocation; // not needed for normal chess game
            for (int i = 0; i < board.size.Count; i++)
            {
                //if (i >= location.Count) location.Add(0);
                if (board.dimensionsToMirror.Contains(i))
                    location[i] = board.size[i] - 1 - location[i];
            }
        }
        StartCoroutine(Place());
        if (name.ToLower().Equals("king") || name.ToLower().Equals("dabbaba"))
            vip = true;
    }

    public List<List<int>> GetMovableLocations()
    {
        List<List<int>> result = new List<List<int>>();
        if (moves == null || moves.Count < 1) return null;
        foreach (Move move in moves)
        {
            
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Place()
    {
        yield return new WaitForSeconds(0.2f);
        transform.position = board.squares[CarlMath.ListAsString(location)].gameObject.transform.position;
    }
}
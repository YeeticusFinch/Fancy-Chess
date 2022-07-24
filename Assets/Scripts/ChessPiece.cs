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
    public List<int> mirror = new List<int>();
    public int timesMirrored = 0;
    public bool arrayToMirrored = false;

    public static Dictionary<string, Move[]> moveSets = new Dictionary<string, Move[]> {
        ["king"] = new Move[] { // check and checkmate needs to be hardcoded
            new Move(1),   // left right up down
            new Move(0, 1), // diagonals
        },

        ["queen"] = new Move[] {
            new Move(-1),
            new Move(0, -1),
        },

        ["bishop"] = new Move[] {
            new Move(0, -1) // only diagonals
        },

        ["knight"] = new Move[] { // only catch is the diagonal needs to have a component vector in the same direction as the lateral (prob needs to be hardcoded)
            new Move(1, 1, false, true, true) // move one space laterally, and one space diagonally, must move 1 forward, can jump over pieces
        },

        ["rook"] = new Move[] {
            new Move(-1) // move laterally
        },

        ["pawn"] = new Move[] { // hardest one, a lot probably needs to be hardcoded
            new Move(1, 0, true, false, false, false), // moves forward, can't consume
            new Move(0, 1, true, false, false, true, true), // moves diagonally to consume, can only be used to consume
            new Move(2, 0, true, true, false, false, false, true) // moves forward twice as a first move
        },

        ["unicorn"] = new Move[] {
            new Move(2, 0, false, true, true),
            new Move(0, 1, false, true, true)
        },

        ["elephant"] = new Move[] {
            new Move(1, 0, false, false, true),
            new Move(0, 2, false, false, true)
        },

        ["buffalo"] = new Move[] { // same hard coding as the knight, basically a super knight
            new Move(1, 1, false, true, true),
            new Move(1, 2, false, true, true),
            new Move(2, 1, false, true, true),
        },

        ["princess"] = new Move[] { // combines my two fav pieces from traditional chess (knight and bishop)
             new Move(1, 1, false, true, true), // knight moves (with jumping)
             new Move(0, -1) // bishop moves (no jumping)
        },

        ["empress"] = new Move[] {
            new Move(1, 1, false, true, true), // knight moves (with jumping)
            new Move(-1) // rook moves
        },

        ["dabbaba"] = new Move[] {
            new Move(2, 0, false, true, true) // jumps to spaces laterally
        },

        ["ferz"] = new Move[] {
            new Move(0, 1) // can only move one space diagonally
        },

        ["alpil"] = new Move[] {
            new Move(0, 2, false, true, true) // jumps two spaces diagonally
        },

        ["camel"] = new Move[] {
            new Move(2, 1, false, true, true) // long-jumping knight
        },

        ["zebra"] = new Move[] {
            new Move(1, 2, false, true, true) // another long-jumping knight
        },
    };

    public List<string> useMoveSets = new List<string>();
    
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
        if (timesMirrored > 0)
        {
            if (!arrayToMirrored)
            location[mirror[timesMirrored-1]] = board.size[mirror[timesMirrored-1]] - 1 - location[mirror[timesMirrored-1]];
        }
        if (mirror.Count > timesMirrored)
        {
            timesMirrored++;
            if (arrayToMirrored) {
                for (int i = 1; i < board.size[mirror[timesMirrored - 1]]; i++) {
                    GameObject newPiece = Instantiate(gameObject);
                    newPiece.GetComponent<ChessPiece>().location[mirror[timesMirrored - 1]]+=i;
                }
            }
            else
                Instantiate(gameObject);
        }
        if (useMoveSets.Count > 0)
            foreach (string str in useMoveSets)
                if (moveSets.ContainsKey(str))
                    foreach (Move m in moveSets[str])
                        moves.Add(m);

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
            if (board.whiteTurn && Mathf.Abs(Mathf.Abs(transform.localEulerAngles.y) - 180) > 2)
                transform.localEulerAngles += Vector3.forward * Mathf.Min(180 - transform.localEulerAngles.z, 2);
            else if (!board.whiteTurn && Mathf.Abs(transform.localEulerAngles.y) > 2)
                transform.localEulerAngles -= Vector3.forward * Mathf.Min(transform.localEulerAngles.z, 2);
    }

    IEnumerator Place()
    {
        yield return new WaitForSeconds(0.2f);
        transform.position = board.squares[CarlMath.ListAsString(location)].gameObject.transform.position;
    }
}
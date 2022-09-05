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
            new Move(2, 0, false, true, true) // jumps two spaces laterally
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

    public Vector3 movingTo = Vector3.zero;

    public List<Collider> ignoredCollisions = new List<Collider>();

    [System.Serializable]
    public struct Move
    {
        public Move(int lateral, int diagonal = 0, bool onlyForward = false, bool force = false, bool jump = false, bool canConsume = true, bool onlyForConsume = false, bool firstMove = false, int step = 1)
        {
            this.force = force;
            this.jump = jump;
            this.canConsume = canConsume;
            this.onlyForward = onlyForward;
            this.onlyForConsume = onlyForConsume;
            this.lateral = lateral;
            this.diagonal = diagonal;
            this.firstMove = firstMove;
            this.step = step;
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
        public int step; // 1 means nothing gets skipped, 2 means every other step, 3 means every third step...
    }

    public List<Move> moves = new List<Move>();
    
    void Start()
    {
        if (!GameMaster.gameRunning)
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
        else if (timesMirrored > 0) Init();
    }

    public void Init()
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        if (timesMirrored > 0)
        {
            if (!arrayToMirrored)
                location[mirror[timesMirrored - 1]] = board.size[mirror[timesMirrored - 1]] - 1 - location[mirror[timesMirrored - 1]];
        } else
        {
            if (board.size.Count > 2)
            {
                if (location.Count < 3) location.Add(0);
                location[2] += board.groundLevel;
            }
        }
        if (mirror.Count > timesMirrored)
        {
            timesMirrored++;
            if (arrayToMirrored)
            {
                for (int i = 1; i < board.size[mirror[timesMirrored - 1]]; i++)
                {
                    GameObject newPiece = Instantiate(gameObject);
                    newPiece.GetComponent<ChessPiece>().location[mirror[timesMirrored - 1]] += i;
                    Debug.Log("Spawning Clone " + newPiece.name);
                    //newPiece.GetComponent<BoxCollider>().isTrigger = false;
                    //newPiece.GetComponent<MeshRenderer>().enabled = true;
                    //newPiece.GetComponent<Collider>().enabled = true;
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

    // int lateral, int diagonal = 0, bool onlyForward = false, bool force = false, bool jump = false, bool canConsume = true, bool onlyForConsume = false, bool firstMove = false
    public List<List<int>> GetMovableLocations()
    {
        List<List<int>> result = new List<List<int>>();
        if (moves == null || moves.Count < 1) return null;
        Debug.Log("Calculating moves for " + type);
        foreach (Move m in moves)
        {
            if (m.firstMove && hasMoved) continue;
            if (m.lateral != 0)
            {
                for (int d = 0; d < board.size.Count; d++) // iterate over dimensions
                {
                    Debug.Log("Checking dim " + d);
                    if (m.onlyForward)
                    {
                        if (board.dimensionsToMirror.Contains(d))
                            LatSlideCheck(result, m, location, d, white ? m.step : -m.step);
                    }
                    else
                    {
                        LatSlideCheck(result, m, location, d, m.step);
                        LatSlideCheck(result, m, location, d, -m.step);
                    }
                }
            } else if (m.diagonal != 0)
            {
                for (int d = 0; d < board.size.Count; d++) // iterate over dimensions
                {
                    for (int dd = 0; dd < board.size.Count; dd++) //iterate over perpendicular dimensions
                    {
                        if (dd == d) continue;
                        if (m.onlyForward)
                        {
                            if (board.dimensionsToMirror.Contains(d))
                            {
                                DiaSlideCheck(result, m, location, d, dd, white ? m.step : -m.step, m.step);
                                DiaSlideCheck(result, m, location, d, dd, white ? m.step : -m.step, -m.step);
                            }
                        }
                        else
                        {
                            DiaSlideCheck(result, m, location, d, dd, m.step, m.step);
                            DiaSlideCheck(result, m, location, d, dd, -m.step, m.step);
                            DiaSlideCheck(result, m, location, d, dd, m.step, -m.step);
                            DiaSlideCheck(result, m, location, d, dd, -m.step, -m.step);
                        }
                    }
                }
            }
        }
        return null;
    }

    // addTo = list to add moves to    m = current move,   startLoc = location to check from,   d = dimension,    step = iteration step (positive or negative)
    private List<List<int>> LatSlideCheck(List<List<int>> addTo, Move m, List<int> startLoc, int d, int step)
    {
        List<int> newLatLoc = new List<int>(startLoc);
        Debug.Log("Lat Slide Check " + Random.Range(0, 100) + " : " + newLatLoc[0] + ", " + newLatLoc[1]);
        while (newLatLoc[d]+step >= 0 && newLatLoc[d]+step < board.size[d] && (m.lateral == -1 || Mathf.Abs(newLatLoc[d] - location[d]) < m.lateral))
        {
            newLatLoc[d]+=step;
            Debug.Log("LatLoop " + Random.Range(0, 100) + " : " + newLatLoc[0] + ", " + newLatLoc[1]);
            if (m.force && Mathf.Abs(newLatLoc[d] - location[d]) != m.lateral) continue;
            if (m.diagonal != 0)
            {
                for (int dd = 0; dd < board.size.Count; dd++) // iterate over dimensions for diagonal movement
                {
                    if (dd == d) continue;
                    DiaSlideCheck(addTo, m, newLatLoc, d, dd, step, step);
                    DiaSlideCheck(addTo, m, newLatLoc, d, dd, step, -step);
                }
            }
            else
            {
                int r = AttemptAddMove(addTo, m, newLatLoc);
                if (r == 1 || r == 3) break;
            }
        }
        return addTo;
    }

    // addTo = list to add moves to   m = current move,   startLoc = location to check form,   d = fixed component dimension,   dd = perpendicular dimension,   step = fixed component iteration step,   step2 = perpendicular iteration step
    private List<List<int>> DiaSlideCheck(List<List<int>> addTo, Move m, List<int> startLoc, int d, int dd, int step, int step2)
    {
        Debug.Log("Dia Slide Check");
        List<int> newDiaLoc = new List<int>(startLoc);
        while (newDiaLoc[d]+step >= 0 && newDiaLoc[dd]+step2 >= 0 && newDiaLoc[d]+step < board.size[d] && newDiaLoc[dd]+step2 < board.size[dd] && (m.diagonal == -1 || Mathf.Abs(newDiaLoc[dd] - location[dd]) < m.diagonal))
        {
            newDiaLoc[d] += step;
            newDiaLoc[dd] += step2;
            if (m.force && Mathf.Abs(newDiaLoc[dd] - location[dd]) != m.diagonal) continue;
            int r = AttemptAddMove(addTo, m, newDiaLoc);
            if (r == 1 || r == 3) break;
            //addTo.Add(new List<int>(newDiaLoc));
        }
        return addTo;
    }

    // 0 = move allowed, no piece on square, CAN move further
    // 1 = move allowed, consumed piece on square, CANNOT move further
    // 2 = move allowed, consumed piece on square, or CAN move further
    // 3 = move not allowed, piece on square, CANNOT move further
    // 4 = move not allowed, piece on square, CAN move further
    private int AttemptAddMove(List<List<int>> addTo, Move m, List<int> loc)
    {
        Debug.Log("Adding move");
        string str = CarlMath.ListAsString(loc);
        board.squares[str].TempColor(Color.blue);
        ChessPiece piece = board.PieceAt(loc);
        if (piece == null)
        {
            if (m.onlyForConsume) return 4;
            board.squares[str].TempColor(Color.green);
            addTo.Add(new List<int>(loc));
            return 0;
        }
        if ((piece.white != white || GameMaster.canabalism) && (m.canConsume || m.onlyForConsume))
        {
            board.squares[str].TempColor(Color.green);
            addTo.Add(new List<int>(loc));
            if (m.jump) return 2;
            return 1;
        }
        //board.squares[str].TempColor(Color.black);
        if (m.jump) return 4;
        return 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameMaster.gameRunning) return;
        if (board.canMove && (transform.forward - Vector3.up).magnitude < 0.1f)
        {
            if (board.whiteTurn && Mathf.Abs(Mathf.Abs(transform.localEulerAngles.y) - 180) > 2)
                transform.localEulerAngles += Vector3.forward * Mathf.Min(180 - transform.localEulerAngles.y, 2);
            else if (!board.whiteTurn && Mathf.Abs(transform.localEulerAngles.y) > 2)
                transform.localEulerAngles -= Vector3.forward * Mathf.Min(transform.localEulerAngles.y, 2);
        }
        if (movingTo.Equals(Vector3.zero) && ignoredCollisions.Count > 0)
        {
            foreach (Collider col in ignoredCollisions)
            {
                Physics.IgnoreCollision(col, GetComponent<BoxCollider>(), false);
            }
            ignoredCollisions = new List<Collider>();
        }
        if (GetComponent<Rigidbody>().velocity.magnitude > 0.01f)
        {
            // if piece is below board, delete it
            if (transform.position.y < board.GetMinY()-5)
            {
                if (GameMaster.instance.board.squares[CarlMath.ListAsString(location)].piece == this)
                {
                    GameMaster.instance.board.squares[CarlMath.ListAsString(location)].piece = null;
                }
                GameObject.Destroy(this.gameObject);
            }

        }
    }

    IEnumerator Place()
    {
        yield return new WaitForSeconds(0.2f);
        string str = CarlMath.ListAsString(location);
        transform.position = board.squares[str].gameObject.transform.position+Vector3.up*0.05f;
        board.squares[str].piece = this;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<BoxCollider>().isTrigger = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Board")
        {
            if (!movingTo.Equals(Vector3.zero))
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), true);
                ignoredCollisions.Add(collision.gameObject.GetComponent<BoxCollider>());
            }
            //else
            //   Physics.IgnoreCollision(collision.gameObject.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), false);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Board")
        {
            if (!movingTo.Equals(Vector3.zero))
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), true);
                ignoredCollisions.Add(collision.gameObject.GetComponent<BoxCollider>());
            }
            //else
            //    Physics.IgnoreCollision(collision.gameObject.GetComponent<BoxCollider>(), GetComponent<BoxCollider>(), false);
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;
using System.IO;

public class MainCamera : MonoBehaviour
{
    bool camSwap = true;

    public GameObject[] pieceSets;

    string[] gameModeFiles;

    public TextMesh winText;

    public GameObject masterPieceLibrary;
    private GameObject libraryInstance;
    public GameObject newPieceSelectorText;

    public GameObject debugButtons;

    public GameObject orbitPosWhite;
    public GameObject orbitPosBlack;

    public TextMesh gameModeTitle;
    public TextMesh gameModeDescirption;

    public GameObject axis;
    public Color backgroundColor = new Color();
    public float speed = 0.1f;
    public float mouseSensitivity = 3f;

    public TextMesh newPieceText;

    float rotX = 0;
    float rotY = 0;
    //Quaternion originalRotation;

    public GameObject displayLoc;
    //GameObject displayPiece;
    GameObject[] pieces;

    public TextMesh cameraModeText;

    [SerializeField]
    public GameObject[] selectors = new GameObject[6];

    bool[] selecting = { false, false, false, false, false, false };

    List<List<GameObject>> selectables = new List<List<GameObject>>();

    public List<GameObject> positionSliders = new List<GameObject>(); // Tags DownArrow and UpArrow

    //public int fancyMask = 2;

    // Start is called before the first frame update
    void Start()
    {
        winText.gameObject.GetComponent<MeshRenderer>().enabled = false;
        //originalRotation = transform.localRotation;
        StartCoroutine(CamSetup());
        pieces = GameObject.FindGameObjectsWithTag("Piece");
        GameObject piece = pieces[Random.Range(0, pieces.Length)];
        displayLoc.GetComponent<MeshFilter>().mesh = piece.GetComponent<MeshFilter>().mesh;
        displayLoc.transform.localScale = piece.transform.localScale*2;
        displayLoc.GetComponent<MeshRenderer>().materials = piece.GetComponent<MeshRenderer>().materials;

        //DirectoryInfo gameModeDir = new DirectoryInfo("Assets/GameModes");
        gameModeFiles = Directory.GetFiles("Assets" + Path.DirectorySeparatorChar + "GameModes");

        debugButtons.GetComponent<RectTransform>().localScale = Vector3.zero;
        newPieceSelectorText.GetComponent<RectTransform>().localScale = Vector3.zero;

        //displayLoc.transform.localEulerAngles += Vector3.right*20;
        //displayPiece.GetComponent<Rigidbody>().useGravity = false;
    }

    public void Init()
    {
        GetComponent<VideoPlayer>().enabled = false;
        List<List<GameObject>> selectables = new List<List<GameObject>>();
        StartCoroutine(StartSelectables());
        GetComponent<Camera>().backgroundColor = backgroundColor;
    }

    public int currentGameMode = 0;

    void NextGameMode()
    {
        do currentGameMode = (currentGameMode + 1) % gameModeFiles.Length;
        while (!SelectGameMode(currentGameMode));
    }

    bool SelectGameMode(int i)
    {
        Debug.Log("Loading " + gameModeFiles[i]);
        if (gameModeFiles[i].IndexOf(".meta") != -1) return false;
        GameMaster.instance.board.loadFromFile(gameModeFiles[i]);
        gameModeTitle.text = GameMaster.instance.board.gameModeName;
        gameModeDescirption.text = GameMaster.instance.board.description;
        return true;
    }

    IEnumerator StartSelectables()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < selectors.Length; i++)
        {
            if (i < GameMaster.instance.board.size.Count)
                selectors[i].GetComponent<TextMesh>().text = "∇ [" + (GameMaster.displayDims[i] + 1) + "]"; // used to not have `+ 1`
            else
            {
                selectors[i].GetComponent<TextMesh>().text = "∇ [nil]";
                GameMaster.displayDims[i] = -1;
            }
        }
        for (int i = 0; i < GameMaster.instance.board.size.Count; i++)
        {
            if (i >= GameMaster.displayLoc.Count) GameMaster.displayLoc.Add(0);
            if (i >= positionSliders.Count)
            {
                GameObject newSlider = Instantiate(positionSliders[0], positionSliders[0].transform.position + positionSliders[0].transform.up * -0.4f * (i), positionSliders[0].transform.rotation);
                newSlider.transform.SetParent(transform);
                newSlider.GetComponent<TextMesh>().text = i + 1 + " [" + GameMaster.displayLoc[i] + "]"; // used to not have `+ 1`
                positionSliders.Add(newSlider);
            }
        }
    }

    bool whiteLock = true;

    GameObject currentAxis()
    {
        return camSwap ? (GameMaster.instance.board.whiteTurn ? orbitPosWhite : orbitPosBlack) : (whiteLock ? orbitPosWhite : orbitPosBlack);
    }

    public void startNewPieceSelector(Tile tile)
    {
        if (tile.piece != null)
            tile.piece.Die();
        GameMaster.selectTileForNewPiece = false;
        GameObject.FindGameObjectWithTag("SpawnPiece").GetComponent<TextMesh>().color = Color.green;
        GameMaster.selectTileForNewPiece = false;
        newPieceSelectorText.GetComponent<RectTransform>().localScale = Vector3.one;
        libraryInstance = GameObject.Instantiate(masterPieceLibrary);
        libraryInstance.GetComponent<PieceLibrary>().square = tile.transform.gameObject.GetComponent<Tile>();
        libraryInstance.transform.position = tile.transform.position;
        newPieceText.text = libraryInstance.GetComponent<PieceLibrary>().getPiece().getName();
    }

    bool orbiting = true;
    bool spinSnapping = false;
    public LayerMask mask;
    // Update is called once per frame
    void Update()
    {
        if (GameMaster.winner != null)
        {
            winText.gameObject.GetComponent<MeshRenderer>().enabled = true;
            winText.text = GameMaster.winner.ToUpper() + " WINS!!!";
            winText.GetComponent<RectTransform>().localScale = new Vector3(Random.Range(0.95f, 1.05f), Random.Range(1.7f, 1.8f), 0);
            if (Random.Range(0, 2) == 1)
                winText.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
            else
                winText.color = GameMaster.winner.Equals("WHITE") ? Color.white : Color.black;
        }
        if ((axis.transform.position - currentAxis().transform.position).magnitude > 0.01f && GameMaster.instance.board.canMove)
            axis.transform.position += (currentAxis().transform.position - axis.transform.position).normalized * Mathf.Min(Vector3.Distance(currentAxis().transform.position, axis.transform.position), 5);
        //axis.transform.eulerAngles += (currentAxis().transform.eulerAngles - axis.transform.eulerAngles).normalized * CarlMath.AbsMin(Vector3.Distance(currentAxis().transform.eulerAngles, axis.transform.eulerAngles), 1);
        if ((axis.transform.eulerAngles - currentAxis().transform.eulerAngles).magnitude > 0.01f && GameMaster.instance.board.canMove)
        {
            axis.transform.eulerAngles += Vector3.right * Mathf.Sign(CarlMath.angleDiff(currentAxis().transform.eulerAngles.x, axis.transform.eulerAngles.x)) * Mathf.Min(Mathf.Abs(CarlMath.angleDiff(currentAxis().transform.eulerAngles.x, axis.transform.eulerAngles.x)), 4);
            axis.transform.eulerAngles += Vector3.up * Mathf.Sign(CarlMath.angleDiff(currentAxis().transform.eulerAngles.y, axis.transform.eulerAngles.y)) * Mathf.Min(Mathf.Abs(CarlMath.angleDiff(currentAxis().transform.eulerAngles.y, axis.transform.eulerAngles.y)), 4);
            axis.transform.eulerAngles += Vector3.forward * Mathf.Sign(CarlMath.angleDiff(currentAxis().transform.eulerAngles.z, axis.transform.eulerAngles.z)) * Mathf.Min(Mathf.Abs(CarlMath.angleDiff(currentAxis().transform.eulerAngles.z, axis.transform.eulerAngles.z)), 4);
            rotX = currentAxis().transform.eulerAngles.x;
            rotY = currentAxis().transform.eulerAngles.y;
        }
        if (!GameMaster.gameRunning)
        {
            displayLoc.transform.localEulerAngles += Vector3.forward;
            if (!spinSnapping && Random.Range(0, 200) == 1)
            {
                StartCoroutine(SpinSwap());
            }
            if (Input.GetButtonDown("Fire1"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, mask))
                {
                    //Debug.Log(hit.transform.gameObject.name);
                    if (hit.transform.gameObject.name.Equals("Start Button"))
                    {
                        foreach (GameObject set in pieceSets) set.SetActive(false);
                        pieceSets[GameMaster.instance.board.pieceSet].SetActive(true);
                        GameMaster.gameRunning = true;
                        foreach (GameObject o in GameObject.FindGameObjectsWithTag("GameModeSelector")) GameObject.Destroy(o);
                    }
                    else if (hit.transform.gameObject.tag.Equals("GameModeSelector")) {
                        Debug.Log("Next game mode");
                        NextGameMode();
                    }
                }
            }
            return;
        }
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Rotate Up") != 0 || Input.GetAxisRaw("Jump") != 0)
        {
            //transform.position += (transform.right * Input.GetAxisRaw("Horizontal") + GetForwardUnitVector() * Input.GetAxisRaw("Vertical") + Vector3.up * Input.GetAxisRaw("Rotate Up")) * speed;
            if (orbiting)
            {
                transform.position += transform.forward * Input.GetAxisRaw("Vertical");
                rotY += Input.GetAxisRaw("Horizontal");
                rotX += Input.GetAxis("Rotate Up");
                currentAxis().transform.eulerAngles = new Vector3(rotX, rotY, 0);
                axis.transform.eulerAngles = new Vector3(rotX, rotY, 0);
                currentAxis().transform.position += 0.1f * Vector3.up * Input.GetAxisRaw("Jump");
                axis.transform.position += 0.1f * Vector3.up * Input.GetAxisRaw("Jump");
            }
            else
            {
                transform.position += 0.2f * transform.forward * Input.GetAxisRaw("Vertical");
                transform.position += 0.2f * transform.right * Input.GetAxisRaw("Horizontal");
                transform.position += 0.2f * Vector3.up * Input.GetAxisRaw("Jump");
            }
        }
        if (Input.GetButtonDown("Camera Mode"))
        {
            GetComponent<Camera>().orthographic = !GetComponent<Camera>().orthographic;
            cameraModeText.text = GetComponent<Camera>().orthographic ? "orthographic" : "perspective";
        }
        if (Input.GetButton("Fire2")) {
            if (orbiting)
            {
                rotX -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
                rotY += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
                //Quaternion xQuaternion = Quaternion.AngleAxis(rotX, Vector3.up);
                //Quaternion yQuaternion = Quaternion.AngleAxis(rotY, -Vector3.right);
                //transform.localRotation = originalRotation * xQuaternion * yQuaternion;
                //transform.eulerAngles = new Vector3(rotX, rotY, 0);
                currentAxis().transform.eulerAngles = new Vector3(rotX, rotY, 0);
                axis.transform.eulerAngles = new Vector3(rotX, rotY, 0);
                //GameMaster.instance.board.canMove = false;
            } else
            {
                rotX -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity * 0.6f;
                rotY += Input.GetAxisRaw("Mouse X") * mouseSensitivity * 0.6f;
                transform.eulerAngles = new Vector3(rotX, rotY, 0);
            }
        }
        if (Input.GetButtonDown("Fire1") && GameMaster.instance.board.canMove)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.GetComponent<ChessPiece>() != null)
                {
                    if (GameMaster.selectTileForNewPiece)
                        startNewPieceSelector(GameMaster.instance.board.squares[CarlMath.ListAsString(hit.transform.gameObject.GetComponent<ChessPiece>().location)]);
                    else
                        GameMaster.instance.board.squares[CarlMath.ListAsString(hit.transform.gameObject.GetComponent<ChessPiece>().location)].Click(hit.transform.gameObject.GetComponent<ChessPiece>());
                }
                else if (hit.transform.gameObject.GetComponent<Tile>() != null)
                {
                    if (GameMaster.selectTileForNewPiece)
                        startNewPieceSelector(hit.transform.gameObject.GetComponent<Tile>());
                    else
                        hit.transform.gameObject.GetComponent<Tile>().Click();
                }
            }
        }
        if (Input.GetButtonDown("Fire3"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.GetComponent<ChessPiece>() != null || hit.transform.gameObject.GetComponent<Tile>() != null)
                    currentAxis().transform.position = hit.transform.position;
            }
        }
        if (Input.GetButtonDown("Reset Camera"))
        {
            ResetCam();
        }
        for (int i = 0; i < positionSliders.Count; i++)
        {
            if (Input.GetKeyDown(""+(i+1)))
            {
                GameMaster.displayLoc[i]++;
                while (GameMaster.displayLoc[i] < 0) GameMaster.displayLoc[i] += GameMaster.instance.board.size[i];
                GameMaster.displayLoc[i] %= GameMaster.instance.board.size[i];
                positionSliders[i].GetComponent<TextMesh>().text = i + 1 + " [" + GameMaster.displayLoc[i] + "]"; // didn't used to have a `+ 1`
                GameMaster.instance.board.UpdateDimensions();
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                if (hit.transform.gameObject.tag.Equals("SelectorWheel"))
                {
                    Debug.Log("Selector Click");
                    if (hit.transform.position.Equals(selectors[0].transform.position))
                        BeginSelect(0);
                    else if (hit.transform.position.Equals(selectors[1].transform.position))
                        BeginSelect(1);
                    else if (hit.transform.position.Equals(selectors[2].transform.position))
                        BeginSelect(2);
                    else if (hit.transform.position.Equals(selectors[3].transform.position))
                        BeginSelect(3);
                    else if (hit.transform.position.Equals(selectors[4].transform.position))
                        BeginSelect(4);
                    else if (hit.transform.position.Equals(selectors[5].transform.position))
                        BeginSelect(5);
                } else if (hit.transform.gameObject.tag.Equals("Selectable"))
                {
                    for (int i = 0; i < selecting.Length; i++)
                    {
                        if (selecting[i])
                        {
                            for (int j = 0; j < selectables[i].Count; j++)
                            {
                                if (hit.transform.position.Equals(selectables[i][j].transform.position))
                                {
                                    KillSelector(i);
                                    if (j == 0 || !GameMaster.displayDims.Contains(j - 1))
                                    {
                                        GameMaster.displayDims[i] = j - 1;
                                        selectors[i].GetComponent<TextMesh>().text = "∇ [" + (j == 0 ? "nil" : "" + (j)) + "]"; // used to be (j-1)
                                        GameMaster.instance.board.UpdateDimensions();
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (hit.transform.gameObject.tag.Equals("CameraMode"))
                {
                    GetComponent<Camera>().orthographic = !GetComponent<Camera>().orthographic;
                    cameraModeText.text = GetComponent<Camera>().orthographic ? "orthographic" : "perspective";
                }
                else if (hit.transform.gameObject.tag.Equals("DebugMode"))
                {
                    GameMaster.debug = !GameMaster.debug;
                    hit.transform.gameObject.GetComponent<TextMesh>().text = "debug = " + (GameMaster.debug ? "true" : "false");
                    debugButtons.GetComponent<RectTransform>().localScale = GameMaster.debug ? Vector3.one : Vector3.zero;
                }
                else if (hit.transform.gameObject.tag.Equals("ForceMove"))
                {
                    foreach (Tile t in GameMaster.instance.board.squares.Values)
                    {
                        t.TempColor(Color.green);
                    }
                }
                else if (hit.transform.gameObject.tag.Equals("RachelButton"))
                {
                    GameMaster.Rachel(hit.transform.gameObject.GetComponent<TextMesh>());
                }
                else if (hit.transform.gameObject.tag.Equals("EndTurn"))
                {
                    GameMaster.instance.board.canMove = false;
                    GameMaster.instance.board.whiteTurn = !GameMaster.instance.board.whiteTurn;
                }
                else if (hit.transform.gameObject.tag.Equals("SpawnPiece"))
                {
                    GameMaster.selectTileForNewPiece = !GameMaster.selectTileForNewPiece;
                    if (GameMaster.selectTileForNewPiece)
                    {
                        hit.transform.gameObject.GetComponent<TextMesh>().color = Color.red;
                        //libraryInstance = GameObject.Instantiate(masterPieceLibrary);
                    } else
                    {
                        hit.transform.gameObject.GetComponent<TextMesh>().color = Color.green;
                        newPieceSelectorText.GetComponent<RectTransform>().localScale = Vector3.zero;
                        if (libraryInstance != null)
                            GameObject.Destroy(libraryInstance);
                    }
                }
                else if (hit.transform.gameObject.tag.Equals("PieceDownArrow") && libraryInstance != null)
                {
                    libraryInstance.GetComponent<PieceLibrary>().next(-1);
                    newPieceText.text = libraryInstance.GetComponent<PieceLibrary>().getPiece().getName();
                }
                else if (hit.transform.gameObject.tag.Equals("PieceUpArrow") && libraryInstance != null)
                {
                    libraryInstance.GetComponent<PieceLibrary>().next(1);
                    newPieceText.text = libraryInstance.GetComponent<PieceLibrary>().getPiece().getName();
                }
                else if (hit.transform.gameObject.tag.Equals("PlacePiece") && libraryInstance != null)
                {
                    ChessPiece newPiece = libraryInstance.GetComponent<PieceLibrary>().getPiece();
                    newPiece.location = libraryInstance.GetComponent<PieceLibrary>().square.GetLocation();
                    if (libraryInstance.GetComponent<PieceLibrary>().square.piece != null)
                        libraryInstance.GetComponent<PieceLibrary>().square.piece.Die();
                    libraryInstance.GetComponent<PieceLibrary>().square.piece = newPiece;
                    StartCoroutine(newPiece.Place());
                    newPiece.transform.SetParent(null);
                    GameObject.Destroy(libraryInstance);
                    newPieceSelectorText.GetComponent<RectTransform>().localScale = Vector3.zero;
                }
                else if (hit.transform.gameObject.tag.Equals("MoveMode"))
                {
                    orbiting = !orbiting;
                    hit.transform.gameObject.GetComponent<TextMesh>().text = orbiting ? "orbiting" : "flying";
                    if (orbiting)
                    {
                        OrbitMode();
                    } else
                    {
                        rotX = transform.eulerAngles.x;
                        rotY = transform.eulerAngles.y;
                    }
                }
                else if (hit.transform.gameObject.tag.Equals("ResetCamera"))
                {
                    ResetCam();
                }
                else if (hit.transform.gameObject.tag.Equals("CamSwap"))
                {
                    camSwap = !camSwap;
                    whiteLock = GameMaster.instance.board.whiteTurn;
                    hit.transform.gameObject.GetComponent<TextMesh>().text = "cam swap = " + (camSwap ? "true" : "false");
                }
                else if (hit.transform.gameObject.tag.Equals("UpArrow"))
                {
                    for (int i = 0; i < positionSliders.Count; i++)
                    {
                        if (hit.transform.IsChildOf(positionSliders[i].transform))
                        {
                            GameMaster.displayLoc[i]++;
                            while (GameMaster.displayLoc[i] < 0) GameMaster.displayLoc[i] += GameMaster.instance.board.size[i];
                            GameMaster.displayLoc[i] %= GameMaster.instance.board.size[i];
                            positionSliders[i].GetComponent<TextMesh>().text = i + 1 + " [" + GameMaster.displayLoc[i] + "]"; // didn't used to have a `+ 1`
                            GameMaster.instance.board.UpdateDimensions();
                        }
                    }
                }
                else if (hit.transform.gameObject.tag.Equals("DownArrow"))
                {
                    for (int i = 0; i < positionSliders.Count; i++)
                    {
                        if (hit.transform.IsChildOf(positionSliders[i].transform))
                        {
                            GameMaster.displayLoc[i]--;
                            while (GameMaster.displayLoc[i] < 0) GameMaster.displayLoc[i] += GameMaster.instance.board.size[i];
                            GameMaster.displayLoc[i] %= GameMaster.instance.board.size[i];
                            positionSliders[i].GetComponent<TextMesh>().text = i + 1 + " [" + GameMaster.displayLoc[i] + "]"; // didn't used to have a `+ 1`
                            GameMaster.instance.board.UpdateDimensions();
                        }
                    }
                } else
                {
                    Tile.deselect = true;
                }
            }
        }
    }
    
    void BeginSelect(int sel)
    {
        Debug.Log("BeginSelect " + sel);
        if (selecting[sel])
            KillSelector(sel);
        else
        {
            Debug.Log("Selector Lives");
            selecting[sel] = true;
            while (selectables.Count <= sel) selectables.Add(new List<GameObject>());
            for (int i = 0; i < GameMaster.instance.board.size.Count + 1; i++)
            {
                Debug.Log("Spawning Selector " + sel + " : " + i);
                if (selectables[sel].Count <= i) {
                    selectables[sel].Add(GameObject.Instantiate(selectors[sel], selectors[sel].transform.position + selectors[sel].transform.up * -0.3f * (selectables[sel].Count + 1), selectors[sel].transform.rotation));
                    selectables[sel][selectables[sel].Count - 1].tag = "Selectable";
                    selectables[sel][selectables[sel].Count - 1].transform.SetParent(transform);
                };
                selectables[sel][i].SetActive(true);
                if (i == 0)
                    selectables[sel][i].GetComponent<TextMesh>().text = "--> [nil]";
                else
                    selectables[sel][i].GetComponent<TextMesh>().text = "--> [" + (i) + "]"; // used to be (i-1)
            }
        }
    }

    void KillSelector(int sel)
    {
        selecting[sel] = false;
        foreach (GameObject o in selectables[sel])
        {
            o.SetActive(false);
        }
    }

    void ResetCam()
    {
        OrbitMode();
        currentAxis().transform.position = Vector3.zero;
        rotX = 0;
        rotY = GameMaster.instance.board.whiteTurn ? 0 : 180;
        currentAxis().transform.eulerAngles = new Vector3(rotX, rotY, 0);
        currentAxis().transform.localEulerAngles = new Vector3(rotX, rotY, 0);
    }

    void OrbitMode()
    {
        orbiting = true;
        GameObject.FindGameObjectWithTag("MoveMode").GetComponent<TextMesh>().text = orbiting ? "orbiting" : "flying";
        transform.position = currentAxis().transform.position + currentAxis().transform.up * 23;
        transform.LookAt(currentAxis().transform.position);
        rotX = currentAxis().transform.eulerAngles.x;
        rotY = currentAxis().transform.eulerAngles.y;
    }

    IEnumerator CamSetup()
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = Vector3.zero + Vector3.up * GameMaster.instance.board.size[0] * Mathf.Sqrt(3) / 2;
        SelectGameMode(0);
    }

    IEnumerator SpinSwap()
    {
        spinSnapping = true;
        for (int i = 0; i < 10; i++)
        {
            displayLoc.transform.eulerAngles -= Vector3.right * 9f;
            yield return new WaitForFixedUpdate();
        }
        displayLoc.transform.eulerAngles += Vector3.right * 180;
        GameObject piece = pieces[Random.Range(0, pieces.Length)];
        displayLoc.GetComponent<MeshFilter>().mesh = piece.GetComponent<MeshFilter>().mesh;
        displayLoc.transform.localScale = piece.transform.localScale*2;
        displayLoc.GetComponent<MeshRenderer>().materials = piece.GetComponent<MeshRenderer>().materials;
        for (int i = 0; i < 12; i++)
        {
            displayLoc.transform.eulerAngles -= Vector3.right * 9f;
            yield return new WaitForFixedUpdate();
        }
        spinSnapping = false;
    }

    Vector3 GetForwardUnitVector()
    {
        if (Mathf.Abs(transform.forward.y) < Mathf.Abs(transform.up.y))
        {
            return new Vector3(transform.forward.x, 0, transform.forward.z);
        }
        else
            return new Vector3(transform.up.x, 0, transform.up.z);
    }
}

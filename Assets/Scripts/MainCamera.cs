using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainCamera : MonoBehaviour
{
    public float speed = 0.1f;
    public float mouseSensitivity = 3f;

    float rotX = 90;
    float rotY = 0;
    //Quaternion originalRotation;

    public GameObject displayLoc;
    //GameObject displayPiece;
    GameObject[] pieces;

    [SerializeField]
    GameObject[] selectors = new GameObject[3];

    bool[] selecting = { false, false, false };

    List<List<GameObject>> selectables = new List<List<GameObject>>();

    // Start is called before the first frame update
    void Start()
    {
        //originalRotation = transform.localRotation;
        StartCoroutine(CamSetup());
        pieces = GameObject.FindGameObjectsWithTag("Piece");
        GameObject piece = pieces[Random.Range(0, pieces.Length)];
        displayLoc.GetComponent<MeshFilter>().mesh = piece.GetComponent<MeshFilter>().mesh;
        displayLoc.transform.localScale = piece.transform.localScale*2;
        displayLoc.GetComponent<MeshRenderer>().materials = piece.GetComponent<MeshRenderer>().materials;
        //displayLoc.transform.localEulerAngles += Vector3.right*20;
        //displayPiece.GetComponent<Rigidbody>().useGravity = false;
    }

    public void Init()
    {
        GetComponent<VideoPlayer>().enabled = false;
        List<List<GameObject>> selectables = new List<List<GameObject>>();
    }

    bool spinSnapping = false;
    // Update is called once per frame
    void Update()
    {
        if (!GameMaster.gameRunning)
        {
            displayLoc.transform.localEulerAngles += Vector3.forward;
            if (!spinSnapping && Random.Range(0, 200) == 1)
            {
                StartCoroutine(SpinSwap());
            }
            if (Input.GetButton("Fire1"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    //Debug.Log(hit.transform.gameObject.name);
                    if (hit.transform.gameObject.name.Equals("Start Button")) {
                        GameMaster.gameRunning = true;
                    }
                }
            }
            return;
        }
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Jump") != 0)
            transform.position += (transform.right * Input.GetAxisRaw("Horizontal") + GetForwardUnitVector() * Input.GetAxisRaw("Vertical") + Vector3.up * Input.GetAxisRaw("Jump")) * speed;
        if (Input.GetButton("Fire2")) {
            rotX -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            rotY += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            //Quaternion xQuaternion = Quaternion.AngleAxis(rotX, Vector3.up);
            //Quaternion yQuaternion = Quaternion.AngleAxis(rotY, -Vector3.right);
            //transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            transform.eulerAngles = new Vector3(rotX, rotY, 0);
            GameMaster.instance.board.canMove = false;
        }
        if (Input.GetButton("Fire1") && GameMaster.instance.board.canMove)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.GetComponent<ChessPiece>() != null)
                    GameMaster.instance.board.squares[CarlMath.ListAsString(hit.transform.gameObject.GetComponent<ChessPiece>().location)].Click();
                else if (hit.transform.gameObject.GetComponent<Tile>() != null)
                    hit.transform.gameObject.GetComponent<Tile>().Click();  
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.gameObject.tag.Equals("SelectorWheel"))
                {
                    Debug.Log("Selector Click");
                    if (hit.transform.gameObject.Equals(selectors[0]))
                        BeginSelect(0);
                    else if (hit.transform.gameObject.Equals(selectors[1]))
                        BeginSelect(1);
                    else if (hit.transform.gameObject.Equals(selectors[2]))
                        BeginSelect(2);
                } else if (hit.transform.gameObject.tag.Equals("Selectable"))
                {
                    for (int i = 0; i < selecting.Length; i++)
                    {
                        if (selecting[i])
                        {
                            for (int j = 0; j < selectables[i].Count; j++)
                            {
                                if (hit.transform.gameObject.Equals(selectables[i][j]))
                                {
                                    KillSelector(i);
                                    GameMaster.displayDims[i] = j - 1;
                                    selectors[i].GetComponent<TextMesh>().text = "∇ [" + (j == 0 ? "nil" : ""+(j - 1)) + "]";
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    void BeginSelect(int sel)
    {
        if (selecting[sel])
            KillSelector(sel);
        else
        {
            selecting[sel] = true;
            while (selectables.Count >= sel) selectables.Add(new List<GameObject>());
            for (int i = 0; i < GameMaster.instance.board.size.Count + 1; i++)
            {
                if (selectables[sel].Count >= i) { selectables[sel].Add(GameObject.Instantiate(selectors[sel], selectors[sel].transform.position + Vector3.up * -0.3f * (selectables[sel].Count + 1), selectors[sel].transform.rotation)); selectables[sel][selectables[sel].Count - 1].tag = "Selectable"; };
                selectables[sel][i].SetActive(true);
                if (i == 0)
                    selectables[sel][i].GetComponent<TextMesh>().text = "--> [nil]";
                else
                    selectables[sel][i].GetComponent<TextMesh>().text = "--> [" + (i - 1) + "]";
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

    IEnumerator CamSetup()
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = Vector3.zero + Vector3.up * GameMaster.instance.board.size[0] * Mathf.Sqrt(3) / 2;
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

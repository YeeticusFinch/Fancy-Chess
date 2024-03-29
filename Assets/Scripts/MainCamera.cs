﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Linq;

public class MainCamera : MonoBehaviour
{
    public GameObject axis;
    public Color backgroundColor = new Color();
    public float speed = 0.1f;
    public float mouseSensitivity = 3f;

    float rotX = 0;
    float rotY = 0;
    //Quaternion originalRotation;

    public GameObject displayLoc;
    //GameObject displayPiece;
    GameObject[] pieces;

    public TextMesh cameraModeText;

    [SerializeField]
    GameObject[] selectors = new GameObject[3];

    bool[] selecting = { false, false, false };

    List<List<GameObject>> selectables = new List<List<GameObject>>();

    public List<GameObject> positionSliders = new List<GameObject>(); // Tags DownArrow and UpArrow

    //public int fancyMask = 2;

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
        StartCoroutine(StartSelectables());
        GetComponent<Camera>().backgroundColor = backgroundColor;
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

    bool spinSnapping = false;
    public LayerMask mask;
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
                if (Physics.Raycast(ray, out hit, 100, mask))
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
        {
            //transform.position += (transform.right * Input.GetAxisRaw("Horizontal") + GetForwardUnitVector() * Input.GetAxisRaw("Vertical") + Vector3.up * Input.GetAxisRaw("Jump")) * speed;
            transform.position += transform.forward * Input.GetAxisRaw("Vertical");
            rotY += Input.GetAxisRaw("Horizontal");
            rotX += Input.GetAxis("Jump");
            axis.transform.eulerAngles = new Vector3(rotX, rotY, 0);
        }
        if (Input.GetButtonDown("Camera Mode"))
        {
            GetComponent<Camera>().orthographic = !GetComponent<Camera>().orthographic;
            cameraModeText.text = GetComponent<Camera>().orthographic ? "orthographic" : "perspective";
        }
        if (Input.GetButton("Fire2")) {
            rotX -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            rotY += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            //Quaternion xQuaternion = Quaternion.AngleAxis(rotX, Vector3.up);
            //Quaternion yQuaternion = Quaternion.AngleAxis(rotY, -Vector3.right);
            //transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            //transform.eulerAngles = new Vector3(rotX, rotY, 0);
            axis.transform.eulerAngles = new Vector3(rotX, rotY, 0);
            //GameMaster.instance.board.canMove = false;
        }
        if (Input.GetButtonDown("Fire1") && GameMaster.instance.board.canMove)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                //Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.GetComponent<ChessPiece>() != null)
                    GameMaster.instance.board.squares[CarlMath.ListAsString(hit.transform.gameObject.GetComponent<ChessPiece>().location)].Click(hit.transform.gameObject.GetComponent<ChessPiece>());
                else if (hit.transform.gameObject.GetComponent<Tile>() != null)
                    hit.transform.gameObject.GetComponent<Tile>().Click();  
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
                    axis.transform.position = hit.transform.position;
            }
        }
        if (Input.GetButtonDown("Reset Camera"))
        {
            axis.transform.position = Vector3.zero;
            rotX = 0;
            rotY = 0;
            axis.transform.eulerAngles = new Vector3(rotX, rotY, 0);
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
                } else if (hit.transform.gameObject.tag.Equals("CameraMode"))
                {
                    GetComponent<Camera>().orthographic = !GetComponent<Camera>().orthographic;
                    cameraModeText.text = GetComponent<Camera>().orthographic ? "orthographic" : "perspective";
                } else if (hit.transform.gameObject.tag.Equals("UpArrow"))
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

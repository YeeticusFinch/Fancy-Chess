using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public float speed = 0.1f;
    public float mouseSensitivity = 3f;

    float rotX = 90;
    float rotY = 0;
    //Quaternion originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CamSetup());
        //originalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
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
                else
                    hit.transform.gameObject.GetComponent<Tile>().Click();  
            }
        }
    }

    IEnumerator CamSetup()
    {
        yield return new WaitForSeconds(0.1f);
        transform.position = Vector3.zero + Vector3.up * GameMaster.instance.board.size[0] * Mathf.Sqrt(3) / 2;
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

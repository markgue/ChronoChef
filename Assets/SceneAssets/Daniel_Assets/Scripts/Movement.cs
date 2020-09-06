﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    #region Private Variables
    private Tilemap tilemap;
    private int faceDirection = 0;
    public int FaceDirection { get; private set; }

    // Movement
    private bool readyToMove;
    private bool isMoving;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float timer;
    private float moveTime = 0.2f;

    // Carrying Items
    private bool carrying;
    private GameObject payload;
    #endregion

    #region Start/Update
    // Start is called before the first frame update
    void Start()
    {
        // Assign Variables
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        readyToMove = true;
        isMoving = false;
        carrying = false;

        // Snap player to grid
        transform.position = tilemap.GetCellCenterWorld(tilemap.WorldToCell(transform.position));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position == endPosition && timer != 0)
        {
            readyToMove = true;
            //startPosition = transform.position;
            //endPosition = transform.position;
            timer = 0;
            isMoving = false;
        }

        int direction = 5;
        if (readyToMove)
        {
            if (Input.GetKeyDown("space"))
                if (!carrying)
                {
                    if (Pickup())
                        return;
                }
                else
                {
                    if (PutDown())
                        return;
                }

            if (Input.GetKey("w"))
                direction = 0;
            else if (Input.GetKey("d"))
                direction = 1;
            else if (Input.GetKey("s"))
                direction = 2;
            else if (Input.GetKey("a"))
                direction = 3;

            if (direction != 5)
            {
                faceDirection = direction;
                FaceForward();
                if (CheckInFront())
                    MovePlayer();
            }
        }
        else if (isMoving)
        {
            timer += Time.deltaTime / moveTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, timer);

            if (carrying)
                payload.transform.position = Vector3.Lerp(startPosition, endPosition, timer);
        }




        //float horizontalAxis = Input.GetAxisRaw("Horizontal");
        //float verticalAxis = Input.GetAxisRaw("Vertical");
        //Vector3 moveDirection = new Vector3(horizontalAxis, verticalAxis, 0);
        //if (Mathf.Abs(verticalAxis) > Mathf.Abs(horizontalAxis))
        //{
        //    if (verticalAxis > 0)
        //        faceDirection = 0;
        //    else
        //        faceDirection = 2;
        //}
        //else if (Mathf.Abs(verticalAxis) < Mathf.Abs(horizontalAxis))
        //{
        //    if (horizontalAxis > 0)
        //        faceDirection = 1;
        //    else
        //        faceDirection = 3;
        //}
        //FaceForward();
        //this.transform.position += moveDirection * speed * Time.deltaTime / 5;

    }
    #endregion

    #region Private Functions
    //private Vector3Int ConvertPositionToGrid(Vector3 position)
    //{
    //    return new Vector3Int((int)Mathf.Floor(position.x * 2), (int)Mathf.Floor(position.y * 2), (int)Mathf.Floor(position.z));
    //}

    //private Vector3Int ConvertGridToPosition(Vector3 grid)
    //{
    //    return new Vector3Int((int)Mathf.Floor(grid.x / 2), (int)Mathf.Floor(grid.y / 2), (int)Mathf.Floor(grid.z));
    //}

    private Vector3 ConvertDirectionToVector(int direction)
    {
        switch (direction)
        {
            case 0: return Vector3Int.up;
            case 1: return Vector3Int.right;
            case 2: return Vector3Int.down;
            case 3: return Vector3Int.left;
            default:
                Debug.Log("Passed in illegal direction to ConvertDirectionToVector");
                throw new System.Exception();
        }
    }

    private bool CheckInFront()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, ConvertDirectionToVector(faceDirection), 1f, LayerMask.GetMask("Obstacle", "items"));
        if (hit.collider != null)
            return false;
        return true;
    }

    private void FaceForward()
    {
        //switch (faceDirection)
        //{
        //    case 0: this.transform.Rotate(Vector3.zero - transform.eulerAngles, Space.Self); break;
        //    case 1: this.transform.Rotate(Vector3.back * 90 - transform.eulerAngles, Space.Self); break;
        //    case 2: this.transform.Rotate(Vector3.forward * 180 - transform.eulerAngles, Space.Self); break;
        //    case 3: this.transform.Rotate(Vector3.forward * 90 - transform.eulerAngles, Space.Self); break;
        //    default:
        //        Debug.Log("Passed illegal direction to faceDirection");
        //        throw new System.Exception();
        //}

        Vector3 rotationAmount = new Vector3(0, 0, -90 * faceDirection) - transform.eulerAngles;
        this.transform.Rotate(rotationAmount, Space.Self);
        if (carrying)
            payload.transform.Rotate(rotationAmount, Space.Self);
    }

    private void MovePlayer()
    {
        readyToMove = false;
        isMoving = true;

        startPosition = transform.position;
        endPosition = transform.position + ConvertDirectionToVector(faceDirection);
    }

    private bool Pickup()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, ConvertDirectionToVector(faceDirection), 1f, LayerMask.GetMask("items"));
        if (hit.collider == null)
            return false;
        StartCoroutine(PickpHelper(hit));

        return true;
    }

    private IEnumerator PickpHelper(RaycastHit2D hit)
    {
        readyToMove = false;
        carrying = true;
        payload = hit.collider.gameObject;

        Vector3 startPos = hit.collider.transform.position;
        hit.collider.enabled = false;
        float t = 0;

        while (t < 1)
        {
            yield return new WaitForFixedUpdate();
            t += Time.deltaTime / 0.5f;
            hit.collider.transform.position = Vector3.Lerp(startPos, transform.position, t);
        }

        readyToMove = true;
    }

    private bool PutDown()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, ConvertDirectionToVector(faceDirection), 1f, LayerMask.GetMask("Obstacles", "items"));
        if (hit.collider != null)
            return false;
        StartCoroutine(PutDownHelper());

        return true;
    }

    private IEnumerator PutDownHelper()
    {
        readyToMove = false;

        Vector3 startPos = payload.transform.position;
        float t = 0;

        while (t < 1)
        {
            yield return new WaitForFixedUpdate();
            t += Time.deltaTime / 0.5f;
            payload.transform.position = Vector3.Lerp(startPos, startPos + ConvertDirectionToVector(faceDirection), t);
        }

        payload.transform.Rotate(-payload.transform.rotation.eulerAngles);
        payload.GetComponent<BoxCollider2D>().enabled = true;
        carrying = false;
        payload = null;
        readyToMove = true;
    }
    #endregion
}
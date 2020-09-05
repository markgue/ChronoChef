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
    private bool readyToMove;
    private uint speed = 50;
    #endregion

    #region Start/Update
    // Start is called before the first frame update
    void Start()
    {
        // Assign Variables
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        readyToMove = true;

        // Snap player to grid
        transform.position = tilemap.GetCellCenterWorld(tilemap.WorldToCell(transform.position));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int direction = 5;
        if (readyToMove)
        {
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
                StartCoroutine(MovePlayer());
            }
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

    private bool CheckInFront(Vector3Int position, Vector3Int direction)
    {
        return true;
    }

    private void FaceForward()
    {
        switch (faceDirection)
        {
            case 0: this.transform.Rotate(Vector3.zero - transform.eulerAngles, Space.Self); break;
            case 1: this.transform.Rotate(Vector3.back * 90 - transform.eulerAngles, Space.Self); break;
            case 2: this.transform.Rotate(Vector3.forward * 180 - transform.eulerAngles, Space.Self); break;
            case 3: this.transform.Rotate(Vector3.forward * 90 - transform.eulerAngles, Space.Self); break;
            default:
                Debug.Log("Passed illegal direction to faceDirection");
                throw new System.Exception();
        }
    }

    private IEnumerator MovePlayer()
    {
        readyToMove = false;

        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(1f / speed);
            this.transform.position += ConvertDirectionToVector(faceDirection) / 8;
        }

        readyToMove = true;
    }
    #endregion
}
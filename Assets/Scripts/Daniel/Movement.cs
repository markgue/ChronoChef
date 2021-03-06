﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{ 
    #region Private Variables
    private int faceDirection = 0;
    private Tilemap tilemap;
    public int FaceDirection { get {return faceDirection;} }
    private bool readyToMove;
    private bool isMoving;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private float timer;
    private float moveTime = 0.2f;
    private Animator animator;
    #endregion

    #region Start/Update
    // Start is called before the first frame update
    void Start()
    {
        // Assign Variables
        tilemap = GameObject.FindObjectOfType<Tilemap>();
        readyToMove = true;
        isMoving = false;
        animator = GetComponent<Animator>();
        SetAnimator(false, 0);

        // Snap player to grid
        transform.position = tilemap.GetCellCenterWorld(tilemap.WorldToCell(transform.position));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position == endPoint && timer != 0)
        {
            readyToMove = true;
            timer = 0;
            isMoving = false;
            SetAnimator(false, faceDirection);
        }

        int direction = 5;
        if (readyToMove)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                faceDirection = 0;
                FaceForward();
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                faceDirection = 1;
                FaceForward();
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                faceDirection = 2;
                FaceForward();
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                faceDirection = 3;
                FaceForward();
            }
            else if (Input.GetKey("w"))
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
            transform.position = Vector3.Lerp(startPoint, endPoint, timer);
        }
    }
    #endregion

    #region Public Functions
    public Vector3 DirectionToVector(int direction)
    {
        switch (direction)
        {
            case 0: return Vector3Int.up;
            case 1: return Vector3Int.right;
            case 2: return Vector3Int.down;
            case 3: return Vector3Int.left;
            default:
                Debug.Log("Passed in illegal direction to DirectionToVector: " + direction);
                throw new System.Exception();
        }
    }

    public bool CheckInFront()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position + DirectionToVector(faceDirection) * 0.4f, DirectionToVector(faceDirection), 0.6f, LayerMask.GetMask("Obstacle", "Item", "NPC", "Puddle", "Lava"));
        //if (hit.collider != null && !hit.collider.CompareTag("walkable"))
        //    return false;
        //return true;
        RaycastHit2D[] raycastResults = Physics2D.RaycastAll(transform.position + DirectionToVector(faceDirection) * 0.04f, DirectionToVector(faceDirection), 1f,
            LayerMask.GetMask("Obstacle", "Item", "NPC", "Puddle", "Lava"));
        foreach (RaycastHit2D result in raycastResults)
            if (result.collider != null && !result.collider.CompareTag("walkable"))
                return false;
        return true;
    }
    public bool RequestFreeze()
    {
        if (readyToMove)
        {
            readyToMove = false;
            return true;
        }
        else
            return false;
    }

    public void ReleaseFreeze()
    {
        readyToMove = true;
    }

    public void ReleaseFreeze(float sec)
    {
        Invoke("ReleaseFreeze", sec);
    }
    #endregion

    #region Private Functions
    private void FaceForward()
    {
        //Vector3 rotationAmount = new Vector3(0, 0, -90 * faceDirection) - transform.eulerAngles;
        //this.transform.Rotate(rotationAmount, Space.Self);
        animator.SetInteger("Direction", faceDirection);
    }

    private void MovePlayer()
    {
        readyToMove = false;
        isMoving = true;
        SetAnimator(true, faceDirection);

        startPoint = transform.position;
        endPoint = transform.position + DirectionToVector(faceDirection);
    }

    private void SetAnimator(bool moving, int dir)
    {
        if (dir > 3 || dir < 0)
            Debug.Log("SetAnimator: Passed illegal parameter: " + dir);

        animator.SetBool("Moving", moving);
        animator.SetInteger("Direction", dir);
    }
    #endregion
}
﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class RobotMoveScript : MonoBehaviour
{
    //--------Player Variables--------
    [SerializeField] private Transform playerPos;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] public Animator roboAnimator;
    private Vector2 playerVectorPos;
    private Rigidbody2D playerRb;
    private float horizontalInput;
    private int multiplier = 5;
    private bool canCrawl;

    public RaycastHit2D playerRaycastHitDoor, hitVent;
    //--------Player Variables--------

    //--------Door Temp Variables--------
    private GameObject doubleDoorLeft, doubleDoorRight;
    private Animator tempRightAnimator, tempLeftAnimator, tempRightAnimatorDoubleDoors, tempLeftAnimatorDoubleDoors;
    private Collider2D tempRightBoxCol2D, tempLeftBoxCol2D, doubleDoorCol2D;
    private Vector2 tempRightUpperDoor, tempLeftUpperDoor, tempRightUpperDoubleDoor, tempLeftUpperDoubleDoor;
    //--------Door Temp Variables--------

    //--------Colliders & Contacts--------
    [SerializeField] private ContactFilter2D doorZoneFilter;
    [SerializeField] public BoxCollider2D[] boxColliders = new BoxCollider2D[0];
    [SerializeField] public CircleCollider2D circleCollider;
    private Collider2D[] doorZoneDetectionResults = new Collider2D[16];
    //--------Colliders & Contacts--------

    //--------Door Boolean Variables--------
    private bool isPlayerInsideDoorZone;
    private bool rightDoor, leftDoor, rightDoubleDoor, leftDoubleDoor;
    private bool openDoorPulse, closeDoorPulse;
    //--------Door Boolean Variables--------

    //--------Door Light--------
    [SerializeField] GameObject[] doorLight = new GameObject[0];
    private void Awake()
    {
        
        if (PlayerData.playerTransformPos != null)
        {
            transform.position = PlayerData.playerTransformPos;
        }

        if (PlayerData.facingStatic == false)
        {
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        else if (PlayerData.facingStatic == true)
        {
            transform.localRotation = new Quaternion(0, 180, 0, 0);
        }
        if (PlayerData.slideTaskDone)
        {
            doorLight[0].GetComponent<Light2D>().color = new Color(0, 1, 0);
            doorLight[1].GetComponent<Light2D>().color = new Color(0, 1, 0);
        }

    }

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {


        playerVectorPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

        horizontalInput = Input.GetAxisRaw("Horizontal");


        DoorController();

        UpdateIsInsideDoorZone();

        if (canCrawl)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                roboAnimator.SetBool("crawl", true);
                boxColliders[0].enabled = false;
                boxColliders[1].enabled = true;
                circleCollider.enabled = false;

                if (Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Horizontal") > 0)
                {
                    float move = Input.GetAxis("Horizontal");
                    roboAnimator.SetBool("hold", true);
                }
                else
                {
                    roboAnimator.SetBool("hold", false);
                }
            }
        }
        else if (!canCrawl)
        {
            roboAnimator.SetBool("crawl", false);
            boxColliders[0].enabled = true;
            boxColliders[1].enabled = false;
            circleCollider.enabled = true;
        }

    }
    private void FixedUpdate()
    {
        MoveRobo();
    }

    private void OpenDoor()
    {
        if (playerRaycastHitDoor.rigidbody.tag == "OpenableDoorRight")
        {

            tempRightAnimator = playerRaycastHitDoor.collider.GetComponent<Animator>();
            tempRightBoxCol2D = playerRaycastHitDoor.collider.GetComponent<BoxCollider2D>();
            tempRightUpperDoor = playerRaycastHitDoor.collider.gameObject.GetComponentInChildren<Transform>().Find("Ovi_ylä").GetComponent<Transform>().position;
            tempRightAnimator.SetBool("rightDoorIsOpening", true);
            if (tempRightUpperDoor.y > 3.5f)
            {
                tempRightBoxCol2D.enabled = false;
                closeDoorPulse = true;
                openDoorPulse = false;
            }
        }
        else if (playerRaycastHitDoor.rigidbody.tag == "OpenableDoorLeft")
        {

            tempLeftAnimator = playerRaycastHitDoor.collider.GetComponent<Animator>();
            tempLeftBoxCol2D = playerRaycastHitDoor.collider.GetComponent<BoxCollider2D>();
            tempLeftUpperDoor = playerRaycastHitDoor.collider.gameObject.GetComponentInChildren<Transform>().Find("Ovi_ylä").GetComponent<Transform>().position;
            tempLeftAnimator.SetBool("leftDoorIsOpening", true);
            if (tempLeftUpperDoor.y > 3.5f)
            {
                tempLeftBoxCol2D.enabled = false;
                closeDoorPulse = true;
                openDoorPulse = false;
            }
        }
    }

    private void OpenBoth()
    {
        if (playerRaycastHitDoor.rigidbody.tag == "OpenableDoorDouble")
        {
            if (playerRaycastHitDoor.rigidbody.GetComponent<DoorIndex>().doorIndex == 1)
            {
                doubleDoorRight = GameObject.Find("/DoubleDoorCollider/Oik_ovi");
                doubleDoorLeft = GameObject.Find("/DoubleDoorCollider/Vas_ovi");
            }
            else if (playerRaycastHitDoor.rigidbody.GetComponent<DoorIndex>().doorIndex == 2 && PlayerData.slideTaskDone)
            {
                doubleDoorRight = GameObject.Find("/DoubleDoorCollider_1/Oik_ovi");
                doubleDoorLeft = GameObject.Find("/DoubleDoorCollider_1/Vas_ovi");
            }
            else
            {
                return;
            }


            doubleDoorCol2D = playerRaycastHitDoor.collider;

            tempRightAnimatorDoubleDoors = doubleDoorRight.GetComponent<Animator>();
            //tempRightBoxCol2D = playerRaycastHitDoor.collider.GetComponent<BoxCollider2D>();
            tempRightUpperDoubleDoor = doubleDoorRight.GetComponentInChildren<Transform>().Find("Ovi_ylä").GetComponent<Transform>().position;
            tempRightAnimatorDoubleDoors.SetBool("rightDoorIsOpening", true);

            tempLeftAnimatorDoubleDoors = doubleDoorLeft.GetComponent<Animator>();
            //tempLeftBoxCol2D = playerRaycastHitDoor.collider.GetComponent<BoxCollider2D>();
            tempLeftUpperDoubleDoor = doubleDoorLeft.GetComponentInChildren<Transform>().Find("Ovi_ylä").GetComponent<Transform>().position;
            tempLeftAnimatorDoubleDoors.SetBool("leftDoorIsOpening", true);

            if (tempRightUpperDoubleDoor.y > 3.5f && tempLeftUpperDoubleDoor.y > 3.5f)
            {
                doubleDoorCol2D.enabled = false;
                closeDoorPulse = true;
                openDoorPulse = false;
            }
        }
    }
    private void CloseDoorRight()
    {
        tempRightBoxCol2D.enabled = true;
        tempRightAnimator.SetBool("rightDoorIsOpening", false);
    }
    private void CloseDoorLeft()
    {
        tempLeftBoxCol2D.enabled = true;
        tempLeftAnimator.SetBool("leftDoorIsOpening", false);
    }
    private void CloseDoorBoth()
    {
        doubleDoorCol2D.enabled = true;
        tempRightAnimatorDoubleDoors.SetBool("rightDoorIsOpening", false);
        tempLeftAnimatorDoubleDoors.SetBool("leftDoorIsOpening", false);
    }


    private void DoorController()
    {
        if (openDoorPulse == false)
        {
            CheckHitRaycastDoor();
        }
        if (openDoorPulse == true && rightDoubleDoor == true && leftDoubleDoor == true)
        {
            OpenBoth();
        }
        if (closeDoorPulse == true && !isPlayerInsideDoorZone == true && leftDoubleDoor == true && rightDoubleDoor == true)
        {
            CloseDoorBoth();
        }
        //Left Door
        if (playerRaycastHitDoor && openDoorPulse && leftDoor == true)
        {
            OpenDoor();
        }
        if (closeDoorPulse && !isPlayerInsideDoorZone && leftDoor)
        {
            CloseDoorLeft();
        }
        //Right Door

        if (playerRaycastHitDoor && openDoorPulse && rightDoor == true)
        {
            OpenDoor();
        }

        if (closeDoorPulse && !isPlayerInsideDoorZone && rightDoor)
        {
            CloseDoorRight();
        }



    }

    private void UpdateIsInsideDoorZone()
    {
        isPlayerInsideDoorZone = playerCollider.OverlapCollider(doorZoneFilter, doorZoneDetectionResults) > 0;

    }

    private void CheckHitRaycastDoor()
    {

        Debug.DrawRay(playerPos.position, transform.TransformDirection(Vector2.right) * 2f, Color.red);
        LayerMask maskRoom = LayerMask.GetMask("Room");
        LayerMask maskVent = LayerMask.GetMask("CrawlZone");
        playerRaycastHitDoor = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 2f, maskRoom);
        hitVent = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.right), 2f, maskVent);

        if (playerRaycastHitDoor)
        {
            if (playerRaycastHitDoor.rigidbody.CompareTag("OpenableDoorDouble"))
            {
                openDoorPulse = true;
                rightDoubleDoor = true;
                leftDoubleDoor = true;

            }
            else if (playerRaycastHitDoor.rigidbody.CompareTag("OpenableDoorRight"))
            {
                rightDoor = true;
                openDoorPulse = true;
            }
            else if (playerRaycastHitDoor.rigidbody.CompareTag("OpenableDoorLeft"))
            {
                leftDoor = true;
                openDoorPulse = true;
            }
        }
        if (hitVent)
        {
            canCrawl = true;
        }
        else
        {
            canCrawl = false;
        }
    }

    private void Flip()
    {
        if (horizontalInput > 0)
        {
            transform.localRotation = new Quaternion(0, 0, 0, 0);
            PlayerData.facingStatic = false;
        }
        if (horizontalInput < 0)
        {
            transform.localRotation = new Quaternion(0, 180, 0, 0);
            PlayerData.facingStatic = true;
        }
    }
    private void MoveRobo()
    {
        if (horizontalInput > 0)
        {
            Flip();
            roboAnimator.SetBool("move", true);
            playerRb.transform.Translate(Vector2.right * horizontalInput * multiplier * Time.deltaTime);
            

        }

        else if (horizontalInput < 0)
        {
            Flip();
            roboAnimator.SetBool("move", true);
            playerRb.transform.Translate(Vector2.left * horizontalInput * multiplier * Time.deltaTime);
            
        }

        else
        {
            roboAnimator.SetBool("move", false);
        }
    }
}


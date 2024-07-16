using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine.VFX;

public class PlayManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField]
    private CinemachineVirtualCamera P1;//player 1 cam

    [SerializeField]
    private CinemachineVirtualCamera P2;//player 2 cam
    [Header("Scores")]
    [SerializeField]
    private int P1Score = 0;

    [SerializeField]
    private int P2Score = 0;
    
    private bool turnState = false;// player 1 goes first(false) then player 2(true)
    private bool ai = true;
    public bool start = false;
    float cameratimer = 0;
    float aiturntimer = 0;

    //BoardManager board;
    private void OnEnable()
    {
        BoardUIManager.OnstartChange += BoardUIManager_OnstartChange;
    }
    private void BoardUIManager_OnstartChange(bool game)
    {
        start = game;
    }
   
    private void OnDisable()
    {
        BoardUIManager.OnstartChange -= BoardUIManager_OnstartChange;

    }


    // Update is called once per frame
    void Update()
    {
        gameStart();
        //camControl();
    }
    private void gameStart()
    {  //player 1 starts the game move camera over
       if (start) { camControl(); }
        

        if (turnState == false)
        {
            playerTurn();
        }//player 2
        else if(turnState == true && ai==true)
        {
            AITurn();
        }

    }

    private void AITurn()
    {   //random tile selecting for ai
        int AIrow = UnityEngine.Random.Range(0, 10);
        int AIcol = UnityEngine.Random.Range(0, 10);
        
        if (AIcol < 10 && AIrow < 10)
        {   //get the info of the tile on the board
            BoardUnitInfo bUnitInfo = GameObject.Find("BoardManager")
                .GetComponent<BoardManager>().
                boardPlayer[AIrow,AIcol].GetComponent<BoardUnitInfo>();
            //when the tile is not occupied
            if (!bUnitInfo.occupied && aiturntimer <= 0)
            {
                //set the color of the tile to black
                bUnitInfo.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.9f);
                bUnitInfo.gameObject.transform.Find("Water Splash").gameObject.SetActive(true);
                //the other player initiates his turn
                turnState = false;
                //move the camera to to player 2 board
                cameratimer = 2f;
            }//when tile is occupied && has not been attacked
            else if (bUnitInfo.occupied && bUnitInfo.attacked == false && aiturntimer <= 0)
            {   //color is changed to red
                bUnitInfo.GetComponent<Renderer>().material.color = new Color(1f, 0.0f, 0.0f, 0.9f);
                bUnitInfo.attacked = true;
                //camera stays on current location
                turnState = true;
                aiturntimer = 3f;
               // bUnitInfo.gameObject.transform.Find("Fire").gameObject.SetActive(true);
                bUnitInfo.gameObject.transform.Find("Smoke").gameObject.SetActive(true);
                PlayerPrefs.SetInt("Player2Score", ++P2Score);

            }
                       
            camControl();
            //use to delay ai turn and let camera position itself on the p1 board
            if (aiturntimer > 0.0f)
            {
                // Subtract the difference from last time it was called
                aiturntimer -= Time.deltaTime;;

            }

        }
    }

        //player turn logic
    private void playerTurn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        //check if it hit a tile on the player 2
        if (Physics.Raycast(ray, out hitInfo, 100) && hitInfo.transform.parent.name.Equals("Player 2") && hitInfo.transform.tag.Equals("Base"))
        {
            //Debug.Log(hitInfo.transform.GetComponent<BoardUnitInfo>());
            //get tile info
            BoardUnitInfo boardUnit = hitInfo.transform.GetComponent<BoardUnitInfo>();
            //when the user clicks and the board is not occupied
            if (Input.GetMouseButtonDown(0) && !boardUnit.occupied)
            {
                //set the color of the tile to black
                boardUnit.GetComponent<Renderer>().material.color = new Color(0.0f, 0.0f, 0.0f, 0.9f);
                boardUnit.gameObject.transform.Find("Water Splash").gameObject.SetActive(true);
                //the other player initiates his turn
                turnState = true;
                cameratimer = 2f;
                aiturntimer = 3f;
            }//when tile is occupied && has not been attacked
            else if (Input.GetMouseButtonDown(0) && boardUnit.occupied && boardUnit.attacked==false)
            {   //color is changed to red
                boardUnit.GetComponent<Renderer>().material.color = new Color(1f, 0.0f, 0.0f, 0.9f);
                boardUnit.attacked = true;
                PlayerPrefs.SetInt("Player1Score", ++P1Score);
                //boardUnit.gameObject.transform.Find("Fire").gameObject.SetActive(true);
                boardUnit.gameObject.transform.Find("Smoke").gameObject.SetActive(true);
                //camera stays on current location
                turnState = false;
                
            }

            camControl();

        }
    }

    public void camControl()
    {
        if (cameratimer > 0f)
        {
            // Subtract the difference from last time it was called
            cameratimer -= Time.deltaTime;
        }
        //move the camera to player 2 board, for player one to take their turn
        if (start && turnState == false && cameratimer <= 0)
        {
            //turnState = true;
            Debug.Log("move to P2 cam");
            P1.Priority = 0;
            P2.Priority = 1;
            //cameratimer = 2f;
           
        }//move the camera to player 1 board, for player two to take their turn
        else if (start && turnState == true && cameratimer<=0)
        {

            //turnState = false;
            //Debug.Log("move to P1 cam");
            P1.Priority = 1;
            P2.Priority = 0;
            //cameratimer = 2f;
        }
    }
}

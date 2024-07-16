//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoardManager : MonoBehaviour
{   //used to load prefabs and create each player board
    [Header("Board Prefabs")]
    public GameObject BoardTile;
    public GameObject Indicator;
    private GameObject board1;
    private GameObject board2;

    private bool AIPlayer = true;
    private bool start;

    
    //ship prefab creation object
    public GameObject[] ShipPrefabs = new GameObject[5];
    public bool[] shipCount = new bool[5];//use for single ship placement of P1
    public bool[] shipKount = new bool[5];//use for single ship placement of P2

    private int blockSize = 0;//holds ship size
    private int shipId = -1;//ship Id#
    private bool shipOrientation = false;//vertical or horizontal
    
        
    //private bool AIorientation = false;//AI vertical or horizontal
    private bool collision = false;//use for ship placement collision
    private bool Allow_Place = true;//ship placing boolean
    
    //block holder is where the display cubes for ship size are stored
    GameObject BlockHolder = null;

    //update on player board for ship positioning
    public GameObject[,] boardPlayer = new GameObject[10, 10];


    public GameObject[,] boardPlayer2 = new GameObject[10, 10];
     

    private void OnEnable()
    {
        BoardUIManager.OnBoardPieceChange += BoardUIManager_OnBoardPieceChange;

        BoardUIManager.OnOrientationChange += BoardUIManager_OnOrientationChange;
    }
    private void BoardUIManager_OnBoardPieceChange(int size, int Id)
    {
        blockSize = size;
        shipId = Id;
    }
    private void BoardUIManager_OnOrientationChange(bool orientation) {
        shipOrientation = orientation;
    }
    private void OnDisable()
    {
        BoardUIManager.OnBoardPieceChange -= BoardUIManager_OnBoardPieceChange;
        BoardUIManager.OnOrientationChange -= BoardUIManager_OnOrientationChange;

    }
    void Start()
    {
        board1 = new GameObject();
        board2 = new GameObject();
        //Display = new GameObject();
        board1.name = "Player 1";
        board2.name = "Player 2";
        //Display.name = "Display";

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                boardPlayer[i, j] = null;
                boardPlayer2[i, j] = null;
            }
        }

        int row = 1, col = 1;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject tmp = GameObject.Instantiate(this.BoardTile,
                                new Vector3(i, 0, j), this.BoardTile.transform.rotation) as GameObject;


                BoardUnitInfo tmpUI = tmp.GetComponent<BoardUnitInfo>();
                string name = $"{row},{col}";
                boardPlayer[i, j] = tmp;
                tmpUI.UpdateUnitDisplay(row, col);
                tmp.name = name;
                col++;
                tmp.transform.SetParent(board1.transform);
            }
            col = 1;
            row++;

        }
        row = 1; col = 1;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject tmp = GameObject.Instantiate(this.BoardTile,
                                new Vector3(i, 0, j), this.BoardTile.transform.rotation) as GameObject;


                BoardUnitInfo tmpUI = tmp.GetComponent<BoardUnitInfo>();
                string name = $"{row},{col}";
                boardPlayer2[i, j] = tmp;
                tmpUI.UpdateUnitDisplay(row, col);
                tmp.name = name;
                col++;
                tmp.transform.SetParent(board2.transform);
            }
            col = 1;
            row++;

        }
        board2.transform.SetPositionAndRotation(new Vector3(14.7f, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
    }

    // Update is called once per frame
    void Update()
    {   
        start = GameObject.Find("PlayManager")
                .GetComponent<PlayManager>().start;

        if (shipId >= 0 && blockSize > 0 && start ==false)
        {
            userBoard();
        }
        else if (AIPlayer && shipKount[0] != true && start == false) { AIPlace(); }
        
        //Debug.Break();
    }
    

    private void AIPlace()
    {
        //ship ids, and size
        int[,] ships = new int[5, 2]{
            {0 ,2 }, { 1, 5 }, { 2, 3 }, { 3, 4 }, { 4, 4 }
            };
        bool collision = false;
        int sid = 0;
        while (sid < 5)
        {
            int AIrow = Random.Range(0, 10);
            int AIcol = Random.Range(0, 10);

            if (AIPlayer)
            {   //true= horizontal, false= vertical
                shipOrientation = (Random.value > 0.5f);
            }
            bool allow = true;//for ship placement
            //from the coordinate check horizontally    
            if (shipOrientation==true && (AIcol + ships[sid, 1]) < 10 && (AIrow + ships[sid, 1] < 10))
            {
                for (int j = 0; j < ships[sid, 1]; j++)
                {
                    allow = true;
                    //get the current spot on the board then next run grab the next one
                    BoardUnitInfo bUnitInfo = (boardPlayer2[AIrow + j, AIcol]).GetComponent<BoardUnitInfo>();
                    if (bUnitInfo.occupied == true)
                    {//not free

                        //doesnt allow placement n collides
                        collision = true;
                        j = ships[sid, 1];
                        allow = false;
                    }
                    else
                    {//free to place
                        collision = false;//reset collision
                    }
                }//end of for
                if (collision != true && shipKount[sid] != true && allow == true)
                {
                    //when there is no collision, the ship hasnt been placed, and placement is allowed
                     for (int j = 0; j < ships[sid, 1]; j++)
                        {
                        //horizontal occupied update
                        if (shipOrientation==true && (AIcol + ships[sid, 1]) < 10&& (AIrow + ships[sid, 1] < 10))
                        {
                            BoardUnitInfo bUnitInfo = (boardPlayer2[AIrow + j, AIcol]).GetComponent<BoardUnitInfo>();
                            //bUnitInfo.GetComponent<Renderer>().material.color = Color.green;
                            bUnitInfo.occupied = true;//updates the internal list not the actual gameboard
                        }
                    }//end for set occupied
                }
                }//end of horizontal boundary check
                 //from the coordinate check vertically    
                if (shipOrientation == false && (AIcol + ships[sid, 1]) < 10 && (AIrow + ships[sid, 1] < 10))
                {
                for (int j = 0; j < ships[sid, 1]; j++)
                {
                    allow = true;
                    //get the current spot on the board then next run grab the next one
                    BoardUnitInfo bUnitInfo = (boardPlayer2[AIrow, AIcol + j]).GetComponent<BoardUnitInfo>();
                    if (bUnitInfo.occupied == true)
                    {//not free

                        //doesnt allow placement n collides
                        collision = true;
                        j = ships[sid, 1];
                        allow = false;
                    }
                    else
                    {//free to place
                        collision = false;//reset collision
                    }
                }//end of for
                if (collision != true && shipKount[sid] != true && allow == true)
                {
                    //when there is no collision, the ship hasnt been placed, and placement is allowed
                    for (int j = 0; j < ships[sid, 1]; j++)
                    {
                        //vertical occupied update
                        if (shipOrientation == false && (AIcol + ships[sid, 1]) < 10 && (AIrow + ships[sid, 1] < 10))
                        {
                            BoardUnitInfo bUnitInfo = (boardPlayer2[AIrow, AIcol + j]).GetComponent<BoardUnitInfo>();
                            //bUnitInfo.GetComponent<Renderer>().material.color = Color.green;
                            bUnitInfo.occupied = true;//updates the internal list not the actual gameboard
                        }
                    }//end for set occupied
                }
            }//end of horizontal boundary check 

            if (allow == true && collision != true && shipKount[sid] != true && (AIrow + ships[sid,1])<10 && (AIcol + ships[sid, 1]) < 10)
                    {
                        //ShipTypePlaced(AIrow + 14.7f, AIcol, sid, shipKount);
                        shipKount[sid] = true;
                        shipOrientation = false;
                        collision = false;
                        sid++;
                    }
                }//end of while
            }//end of function


    private void userBoard()
    {
        //get the mouse position on the board
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo = new RaycastHit();
        //if it hits an object, find what it is, check for board and which board it is on

        if (Physics.Raycast(ray, out hitInfo, 100) && hitInfo.transform.tag.Equals("Base")
        && hitInfo.transform.parent.name.Equals("Player 1"))
        {   //when its a piece on the board with tag base
            //find the board piece it hit
            BoardUnitInfo boardUnit = hitInfo.transform.GetComponent<BoardUnitInfo>();
            //destroy block holder if it is empty
            if (BlockHolder != null)
            { Destroy(this.BlockHolder); }

            BlockHolder = new GameObject();
            BlockHolder.name = "Size of ship";

            //edit attributes of block holder for the scene

            //ship visualizer for orientation and placement check
            if (shipOrientation && (boardUnit.Row <= 11 - blockSize))
            {   //create cubes to display ship size
                for (int i = 0; i < blockSize; i++)
                {   //Instatiate number of cubes to display
                    GameObject visual = GameObject.Instantiate(Indicator, new Vector3((boardUnit.Row - 1) + i, 1, boardUnit.Col - 1), Indicator.transform.rotation);
                    //set an object out of what is in boardplayer at that location
                    GameObject bPlayer = boardPlayer[(boardUnit.Row - 1) + i, boardUnit.Col - 1];

                    BoardUnitInfo bUnitInfo = bPlayer.GetComponent<BoardUnitInfo>();
                    //Debug.Log($"bUnitInfo: {bUnitInfo}");
                    if (!bUnitInfo.occupied && shipCount[shipId] == false)
                    {//green
                        visual.GetComponent<Renderer>().material.color = new Color(0f, 1.0f, 0.0f, 0.4f);//allowed to place
                        Allow_Place = true;
                    }
                    else if (bUnitInfo.occupied && shipCount[shipId] == false)
                    {//yellow
                        visual.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 0.4f);//not-allowed to place
                        Allow_Place = false;
                        collision = true;
                    }
                    else
                    {//red
                        visual.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.4f);//not-allowed to place
                        Allow_Place = false;
                    }
                    visual.transform.parent = BlockHolder.transform;
                }
            }//ship horizontal
            if (!shipOrientation && (boardUnit.Col <= 11 - blockSize))
            {
                for (int i = 0; i < blockSize; i++)
                {
                    GameObject visual = GameObject.Instantiate(Indicator, new Vector3(boardUnit.Row - 1, 1, (boardUnit.Col - 1) + i), Indicator.transform.rotation);
                    GameObject bPlayer = boardPlayer[boardUnit.Row - 1, (boardUnit.Col - 1) + i];
                    BoardUnitInfo bUnitInfo = bPlayer.GetComponent<BoardUnitInfo>();
                    if (!bUnitInfo.occupied && shipCount[shipId] == false)
                    {//green
                        visual.GetComponent<Renderer>().material.color = new Color(0f, 1.0f, 0.0f, 0.4f);//allowed to place
                        Allow_Place = true;
                    }
                    else if (bUnitInfo.occupied && shipCount[shipId] == false)
                    {//yellow
                        visual.GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 0.0f, 0.4f);//not-allowed to place
                        Allow_Place = false;
                        collision = true;
                    }
                    else
                    {//red
                        visual.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.4f);//not-allowed to place
                        Allow_Place = false;
                    }
                    visual.transform.parent = BlockHolder.transform;
                }
            }//ship vertical



            if (Input.GetMouseButton(0) && Physics.Raycast(ray, out hitInfo, 100))
            {
                // Debug.Log(hitInfo.transform.gameObject.name);
                if (!collision && Allow_Place == true && shipCount[shipId] == false)
                {
                    if (shipOrientation)
                    {
                        for (int i = 0; i < this.blockSize; i++)
                        {
                            GameObject shipBoard = boardPlayer[(boardUnit.Row - 1) + i, (boardUnit.Col - 1)];
                            shipBoard.GetComponent<Renderer>().material.color = Color.green;
                            shipBoard.GetComponent<BoardUnitInfo>().occupied = true;

                            boardPlayer[(boardUnit.Row - 1) + i, (boardUnit.Col - 1)] = shipBoard;
                        }
                    }
                    if (!shipOrientation)
                    {
                        for (int i = 0; i < this.blockSize; i++)
                        {
                            GameObject shipBoard = boardPlayer[(boardUnit.Row - 1), (boardUnit.Col - 1) + i];
                            shipBoard.GetComponent<Renderer>().material.color = Color.green;
                            shipBoard.GetComponent<BoardUnitInfo>().occupied = true;
                            boardPlayer[(boardUnit.Row - 1), (boardUnit.Col - 1) + i] = shipBoard;
                        }
                    }
                    ShipTypePlaced(boardUnit.Row - 1, boardUnit.Col - 1, shipId, shipCount);
                }
                else { collision = false; }

            }

        }//end of board piece check
        else
        {
            Destroy(BlockHolder);
        }
    }


    private void ShipTypePlaced(float row, int col, int Id, bool[] ships)
        {

            switch (Id)
            {
                case 0://patrol ship
                    {
                        if (shipOrientation)
                        {
                            // place it as vertical
                            GameObject testingVisual = GameObject.Instantiate(ShipPrefabs[Id], // this.MRVikhrIFQ,
                                                                              new Vector3((row) + 0.5f,
                                                                                          ShipPrefabs[Id].transform.position.y,
                                                                                          col),
                                                                              ShipPrefabs[Id].transform.rotation) as GameObject;
                            testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
                            //shipCount[Id] = true;
                        }
                        else
                        {//horizonal
                            GameObject testingVisual = GameObject.Instantiate(ShipPrefabs[Id], //this.MRVikhrIFQ,
                                                                              new Vector3(row,
                                                                                          ShipPrefabs[Id].transform.position.y,
                                                                                          (col) + 0.5f),
                                                                              ShipPrefabs[Id].transform.rotation) as GameObject;
                        }

                        break;
                    }
                case 1:
                    {//vertical
                        if (shipOrientation)
                        {

                            GameObject testingVisual = GameObject.Instantiate(ShipPrefabs[Id], // this.MRVikhrIFQ,
                                                                              new Vector3((row) + 1.8f,
                                                                                          Mathf.Round(ShipPrefabs[Id].transform.position.y),
                                                                                          col),
                                                                              ShipPrefabs[Id].transform.rotation) as GameObject;
                            testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
                        }
                        else
                        {//horizontal
                            GameObject testingVisual = GameObject.Instantiate(ShipPrefabs[Id], //this.MRVikhrIFQ,
                                                                              new Vector3(row,
                                                                                          Mathf.Round(ShipPrefabs[Id].transform.position.y),
                                                                                          (col) + 1.8f),
                                                                              ShipPrefabs[Id].transform.rotation) as GameObject;
                        }

                        break;
                    }
                case 2://destroyer ship
                    {
                        if (shipOrientation)
                        {
                            // place it as vertical
                            GameObject testingVisual = GameObject.Instantiate(ShipPrefabs[Id], // this.MRVikhrIFQ,
                                                                              new Vector3((row) + 1f,
                                                                                          ShipPrefabs[Id].transform.position.y,
                                                                                          col),
                                                                              ShipPrefabs[Id].transform.rotation) as GameObject;
                            testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
                            //shipCount[Id] = true;
                        }
                        else
                        {//horizonal
                            GameObject testingVisual = GameObject.Instantiate(ShipPrefabs[Id], //this.MRVikhrIFQ,
                                                                              new Vector3(row,
                                                                                          ShipPrefabs[Id].transform.position.y,
                                                                                          (col) + 1f),
                                                                              ShipPrefabs[Id].transform.rotation) as GameObject;
                        }

                        break;
                    }
                case 3://battleship 1
                    {
                        if (shipOrientation)
                        {
                            // place it as vertical
                            GameObject testingVisual = GameObject.Instantiate(ShipPrefabs[Id], // this.MRVikhrIFQ,
                                                                              new Vector3((row) + 1.3f,
                                                                                          ShipPrefabs[Id].transform.position.y,
                                                                                          col),
                                                                              ShipPrefabs[Id].transform.rotation) as GameObject;
                            testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
                            //shipCount[Id] = true;
                        }
                        else
                        {//horizonal
                            GameObject testingVisual = GameObject.Instantiate(ShipPrefabs[Id], //this.MRVikhrIFQ,
                                                                              new Vector3(row,
                                                                                          ShipPrefabs[Id].transform.position.y,
                                                                                          (col) + 1.3f),
                                                                              ShipPrefabs[Id].transform.rotation) as GameObject;
                        }

                        break;
                    }
                case 4://battleship 2
                    {
                        if (shipOrientation)
                        {
                            // place it as vertical
                            GameObject testingVisual = GameObject.Instantiate(ShipPrefabs[Id], // this.MRVikhrIFQ,
                                                                              new Vector3((row) + 1.4f,
                                                                                          ShipPrefabs[Id].transform.position.y,
                                                                                          col),
                                                                              ShipPrefabs[Id].transform.rotation) as GameObject;
                            testingVisual.transform.RotateAround(testingVisual.transform.position, Vector3.up, 90.0f);
                        }
                        else
                        {//horizonal
                            GameObject testingVisual = GameObject.Instantiate(ShipPrefabs[Id], //this.MRVikhrIFQ,
                                                                              new Vector3(row,
                                                                                          ShipPrefabs[Id].transform.position.y,
                                                                                          (col) + 1.4f),
                                                                              ShipPrefabs[Id].transform.rotation) as GameObject;
                        }

                        break;
                    }
            }
            ships[Id] = true;
        }


} 



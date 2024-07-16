using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
//using UnityEngine.UIElements;
using static UnityEngine.Tilemaps.Tilemap;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

public class BoardUIManager : MonoBehaviour
{
    //set orientation of ship boolean
    // Start is called before the first frame update


    public delegate void BoardPieceChange(int size, int pieceID);
    public static event BoardPieceChange OnBoardPieceChange;
    
    public delegate void OrientationChange(bool orientation);
    public static event OrientationChange OnOrientationChange;

    public delegate void startGame(bool start);
    public static event startGame OnstartChange;
    //public int[] pieceIndex;
    private int pieceID =-1, pieceSize =-1;
    public bool orientation = false, prevorientation = false;
    [SerializeField]
    private bool startG = false;


    //game menu objects 
    [Header("Menus")]
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject ShipMenu;
    [SerializeField]
    private GameObject OptionsMenu;
    [SerializeField]
    private Button startbtn;
    [SerializeField]
    private GameObject Win;
    [SerializeField]
    private GameObject Lose;

    [Header("Board Control")]
    [SerializeField]
    private GameObject boardManager;
    [SerializeField]
    private GameObject playManager;
    [SerializeField]
    private GameObject Scores; 
    [SerializeField]
    private Toggle cheats;

    [SerializeField]
    bool [] shipsplaced = new bool[5];
    private void Start()
    {
        PlayerPrefs.SetInt("Player1Score",0);

        PlayerPrefs.SetInt("Player2Score", 0);
    }
    private void Update() { 
        //pausing the game with escape
        pauseIt();
        startInter();
        scoreupdater();

    }

    private void scoreupdater()
    {
        if(startG == true) {
            //get score values
            int P1 = PlayerPrefs.GetInt("Player1Score");
            int P2 = PlayerPrefs.GetInt("Player2Score");
            //get labels for scores
            TextMeshProUGUI P1score = Scores.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            P1score.text = "Player 1 Score: " + P1;
            TextMeshProUGUI P2score = Scores.gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            P2score.text = "Player 2 Score: " + P2;
            //have a winner?
            if (P1 >= 18)
            {
                Time.timeScale = 0.0f;
                Win.SetActive(true);
            }
            else if(P2>=18) {
                Lose.SetActive(true); 
                    }
        }
    }

    //sets the button to be interactable
    void startInter() {
        //use to check that all ships were placed
        if (startG==false) {
            for (int i = 0; i < shipsplaced.Length; i++)
            {
                shipsplaced[i] = boardManager.GetComponent<BoardManager>().shipCount[i];
            }//if any of the ships has not been place
            if (shipsplaced[0] == false || shipsplaced[1] == false
                || shipsplaced[2] == false || shipsplaced[3] == false
                || shipsplaced[4] == false)
            {   //keep the start button as a non interactible
                startbtn.interactable = false;
            }
            else
            {//if all ships were placed make it interactable 
                startbtn.interactable = true;
            }
        }
        
    }
    //set ship size
    public void setShipSize(int size) {
        pieceSize = size;
    }
    //set ship id
    public void setShipId(int Id){  
        pieceID = Id;
    }
    //send the info to the board manager
    public void sendShipInfo() {
        OnBoardPieceChange?.Invoke(pieceSize, pieceID);
    }
    //change ship orientation
    public void ChangeOrientation(Button button) {
        if (orientation==false && prevorientation==orientation)
        {
            button.transform.Rotate(0, 0, -90);
            prevorientation = orientation;
            orientation = true;
        }
        else if(orientation==true && prevorientation!=orientation) {
            button.transform.Rotate(0, 0, 90);
            prevorientation=!orientation;
            orientation = false;
        }//send ship orientation to board manager       
        OnOrientationChange?.Invoke(orientation);      

    }
    //send bool that game has begun
    public void startIt() {
        startG = true;
        OnstartChange?.Invoke(startG);
    }

    //scene changer for main menu and game scene
    public void changescene(string scene) {
        SceneManager.LoadScene(scene);
    }
    
    //used to pause the game
    private void pauseIt()
    {   //check for it the game has started
        startG = playManager.GetComponent<PlayManager>().start;
        //Debug.Log("Pause menu start check:"+startG);
        //check if the key is pressed, if the pause and options menu is inactive
        if (Input.GetKeyDown(KeyCode.Escape) && pauseScreen.activeSelf == false && OptionsMenu.activeSelf == false)
        {   //check ship menu and boardmanager object are active then set it to inactive
            if (ShipMenu.activeSelf == true) { ShipMenu.SetActive(false); boardManager.SetActive(false); }
            Time.timeScale = 0.0f;//pause anything happening in the game
            pauseScreen.SetActive(true);//show the pause screen

        }//check key is pressed, pause menu is active but the options menu is inactive 
        else if (Input.GetKeyDown(KeyCode.Escape) && pauseScreen.activeSelf == true && OptionsMenu.activeSelf == false)
        {   //if ship menu and bool to start the game are false, activate the ship menu and boardmanager
            if (ShipMenu.activeSelf == false && startG==false) { ShipMenu.SetActive(true); boardManager.SetActive(true); }
            Time.timeScale = 1.0f;//set game back to normal
            pauseScreen.SetActive(false);//make pause menu inactive
        }
    }
    //ui element to resum the game
    public void resume()
    {
        Time.timeScale = 1.0f;//set game time back
        //if ship menu and bool to start the game are false, activate the ship menu and boardmanager 
        if (ShipMenu.activeSelf == false && startG == false) { ShipMenu.SetActive(true); boardManager.SetActive(true); }
        pauseScreen.SetActive(false);//make pause menu inactive
    }
    public void cheatMode() {
        bool cheat = cheats.isOn;
        GameObject[,] cheatboard = boardManager.GetComponent<BoardManager>().boardPlayer2;
        if (cheat==true) {
            for (int i =0; i<10;i++) {
                for (int j = 0; j < 10; j++) {
                    if (cheatboard[i, j].GetComponent<BoardUnitInfo>().occupied) {
                        cheatboard[i, j].GetComponent<Renderer>().material.color = Color.green;
                    }
                }
            }       
        }
        else if (cheat==false)
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (cheatboard[i, j].GetComponent<BoardUnitInfo>().occupied)
                    {
                        cheatboard[i, j].GetComponent<Renderer>().material.color = new Color(0.2039f, 0.4235f, 0.4627f, 0.8078f); ;
                    }
                }
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    //The two players (real and graffiti), and the camera
    [Header("Object Prefabs")]
    public CutsceneHandler cutsceneHandler;
    public PlayerController realPlayer;
    public PlayerController graffitiPlayer;

    //Couple puzzle stuff
    public GameObject graffitiSpot;
    public GameObject couple;
    public Sprite happyCouple;
    public GameObject brokenHeart;
    public GameObject fullHeart;
    public MinigameManager coupleManager;
    public GameObject drawingMinigameCouple;
    [SerializeField] GameObject customTagMaker;
    [SerializeField] MinigameManager customTagMinigame;
    [SerializeField] CustomScreenshotter customScreenshotter;

    //A bunch of UI stuff
    [Header("UI Prefabs")]
    [SerializeField] private TextMeshProUGUI swapText;
    [SerializeField] private TextMeshProUGUI selectionText;
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] private Image sprayCanOnUI;
    [SerializeField] private Image sprayCanOffUI;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button endTagButton;

    [Header("Player")]
    public int playerActive = 1; //1 = realPlayer, 2 = graffitiPlayer
    [Tooltip("The camera's position, the higher the number the higher above the player")]
    public float cameraOffset = 2;

    //These are used in checking collision so you can't swap characters if you would end up inside collision
    //NOTE: This doesn't actually work I think. It doesn't matter much currently, you snap out of collision when you swap inside of it anyways
    [SerializeField] private LayerMask toGraffiti;
    [SerializeField] private LayerMask toReal;

    [Space, Header("Swapping")]
    public float defaultSwapCooldown = 0.3f;
    public float swapCooldown = 0;
    [Tooltip("If enabled, player can only swap forms while on the ground")]
    public bool groundedSwap = false; //Enable to forbid swapping in the air
    private bool hasShiftedBefore = false;

    [Space, Header("Graffiti-ing")]
    public bool canGraffiti = true;
    public bool playerNearGraffiti = false;
    public float proximityThreshold = 1f; //How close player must be to graffiti to be able to interact with it
    private bool playerInMinigame = false;

    [Space, Header("World")]
    private bool coupleMinigameCompleted = false;

    [Space, Header("Meta")]
    public bool gameRunning = false;
    public bool gameHasStarted = false;
    //public bool gameWon = false;
    public int currentCutscene = 0; //0 = standard gameplay. Updated in SceneController and CutsceneHandler

    void Start()
    {
        realPlayer.rb.simulated = false;
        graffitiPlayer.rb.simulated = false;
    }


    void Update()
    {
        if (gameRunning)
        {
            //Camera position update
            cutsceneHandler.updateCutscene(currentCutscene);

            //UI update
            if (playerActive == 1 && playerInMinigame == false)
            {
                if (canGraffiti) isPlayerNearGraffitiSpot();
            }
            if (swapCooldown > 0)
            {
                swapCooldown -= Time.deltaTime;
                if (swapCooldown < 0) swapCooldown = 0;
            }
            if (swapCooldown == 0) swapText.text = "Swap ready! [SHIFT]";
            else swapText.text = "Recharge in " + (int)swapCooldown;
            
            if (Input.GetKeyDown(KeyCode.E) && playerInMinigame == false)
            {
                makeGraffiti();
            }
        }
    }

    public void makeGraffiti() //1 = platform, 2 = warp
    {
        if (playerActive == 1 && playerNearGraffiti && coupleMinigameCompleted == false) //Player is in the real world and can graffiti
        {
            currentCutscene = 1;
            coupleManager.StartMinigameOne();
            playerInMinigame = true;
            cutsceneHandler.changeCamera(new Vector2(drawingMinigameCouple.transform.position.x, drawingMinigameCouple.transform.position.y + cameraOffset), 5f);
            
        }
    }

    //NOTE! This is currently very hardcoded and only for the one puzzle, tis a Wizard-Of-Oz setup for the demo, to be changed later
    public void isPlayerNearGraffitiSpot()
    {
        //player is close enough
        if (Vector2.Distance(realPlayer.transform.position, graffitiSpot.transform.position) <= proximityThreshold && playerActive == 1 && coupleMinigameCompleted == false)
        {
            playerNearGraffiti = true;
            sprayCanOffUI.gameObject.SetActive(false);
            sprayCanOnUI.gameObject.SetActive(true);
            selectionText.text = "[E] Graffiti";
        }
        else if (playerActive == 1)
        {
            playerNearGraffiti = false;
            sprayCanOffUI.gameObject.SetActive(true);
            sprayCanOnUI.gameObject.SetActive(false);
            selectionText.text = "[T] Tag";
        }
    }

    public void couplePuzzleDone()
    {
        fullHeart.SetActive(true);
        couple.GetComponent<SpriteRenderer>().sprite = happyCouple;
        couple.GetComponent<BoxCollider2D>().isTrigger = true;
        graffitiSpot.SetActive(false);
        playerInMinigame = false;
        cutsceneHandler.changeCamera(realPlayer.transform.position, 3f);
        currentCutscene = 0;
        coupleMinigameCompleted = true;
        //graffitiTutorialNote.SetActive(false);
    }

    public void warpPlayer(GameObject destination)
    {
        realPlayer.transform.position = destination.transform.position;
        realPlayer.gameObject.SetActive(true);
        graffitiPlayer.gameObject.SetActive(false);
        playerActive = 1;
    }

    //Swaps between the real and graffiti player
    public void switchCharacter()
    {
        if (playerInMinigame == false)
        {
            bool collisionCheck;
            if (playerActive == 1) //Swapping to graffiti player
            {
                if (groundedSwap == false || (groundedSwap && realPlayer.IsGrounded()))
                {
                    collisionCheck = Physics.CheckBox(realPlayer.col.bounds.center, graffitiPlayer.col.bounds.size * 0.5f, Quaternion.identity, toGraffiti);
                    if (!collisionCheck)
                    {
                        graffitiPlayer.transform.position = realPlayer.col.bounds.center;
                        graffitiPlayer.gameObject.SetActive(true);
                        realPlayer.gameObject.SetActive(false);
                        playerActive = 2;

                        swapCooldown = defaultSwapCooldown;
                        if (hasShiftedBefore == false) swapText.gameObject.SetActive(true);
                        sprayCanOffUI.gameObject.SetActive(false);
                        sprayCanOnUI.gameObject.SetActive(false);
                        selectionText.gameObject.SetActive(false);

                        cutsceneHandler.changeCamera(new Vector2(graffitiPlayer.transform.position.x, graffitiPlayer.transform.position.y + cameraOffset), 2.5f);

                        if (graffitiPlayer.facingRight != realPlayer.facingRight)
                        {
                            graffitiPlayer.changeFacingDirection();
                        }
                    }
                }
            }
            else if (playerActive == 2) //Swapping to real player
            {
                if (groundedSwap == false || (groundedSwap && graffitiPlayer.IsGrounded()))
                {
                    collisionCheck = Physics.CheckBox(graffitiPlayer.col.bounds.center, realPlayer.col.bounds.size * 0.5f, Quaternion.identity, toReal);
                    if (!collisionCheck)
                    {
                        realPlayer.transform.position = graffitiPlayer.col.bounds.center;
                        realPlayer.gameObject.SetActive(true);
                        graffitiPlayer.gameObject.SetActive(false);
                        playerActive = 1;
                        swapCooldown = defaultSwapCooldown;

                        cutsceneHandler.changeCamera(new Vector2(realPlayer.transform.position.x, realPlayer.transform.position.y + cameraOffset), 3f);

                        if (canGraffiti)
                        {
                            isPlayerNearGraffitiSpot();
                            selectionText.gameObject.SetActive(true);
                        }

                        if (graffitiPlayer.facingRight != realPlayer.facingRight)
                        {
                            realPlayer.changeFacingDirection();
                        }
                    }
                }
            }
        }
    }

    public void wallChange(bool wallBehind)
    {
        realPlayer.inFrontOfWall = wallBehind;
        graffitiPlayer.inFrontOfWall = wallBehind;
        if (playerActive == 2 && wallBehind == false)
        {
            switchCharacter();
        }
    }

    public void pauseGame()
    {
        //Unpauses if already paused
        if (gameRunning == false)
        {
            resumeGame();
        }
        else
        {
            menuText.gameObject.SetActive(true);
            gameRunning = false;
            realPlayer.rb.simulated = false;
            graffitiPlayer.rb.simulated = false;
            menuText.text = "PAUSED";
            resumeButton.gameObject.SetActive(true);
        }
    }

    //Unpauses the game
    public void resumeGame()
    {
        //Some initial game setup
        if (gameHasStarted == false)
        {
            gameHasStarted = true;
            resumeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Resume";
            if (canGraffiti)
            {
                selectionText.gameObject.SetActive(true);
                isPlayerNearGraffitiSpot();
            }
            if (hasShiftedBefore)
            {
                swapText.gameObject.SetActive(true);
            }

        }
        gameRunning = true;
        realPlayer.rb.simulated = true;
        graffitiPlayer.rb.simulated = true;
        resumeButton.gameObject.SetActive(false);
        menuText.gameObject.SetActive(false);
    }

    // starts the game and tag creation
    public void StartCustomTagCreation()
    {
        gameHasStarted = true;
        customTagMaker.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);
        endTagButton.gameObject.SetActive(true);
        menuText.gameObject.SetActive(false);
    }

    public void MakeTag()
    {
        customScreenshotter.MakeTag();
    }

    // ends the tag creation and activates player
    public void EndCustomTagCreation()
    {
        gameRunning = true;
        customTagMinigame.TagComplete();
        endTagButton.gameObject.SetActive(false);
        realPlayer.rb.simulated = true;
        graffitiPlayer.rb.simulated = true;
    }

    public void restartGame() // Hi Doin here i think we should just reload the scene instead of all this
    {
        restartButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        gameRunning = true;
        realPlayer.rb.simulated = true;
        graffitiPlayer.rb.simulated = true;
        gameHasStarted = true;
        menuText.gameObject.SetActive(false);
        realPlayer.transform.position = new Vector2(-4.7f, -3f);
        realPlayer.gameObject.SetActive(true);
        graffitiPlayer.gameObject.SetActive(false);
        playerActive = 1;

        sprayCanOffUI.gameObject.SetActive(false);
        sprayCanOnUI.gameObject.SetActive(false);
        selectionText.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    //The two players (real and graffiti), and the camera
    [Header ("Object Prefabs")]
    public GameObject cameraPoint;
    public PlayerController realPlayer;
    public PlayerController graffitiPlayer;
    public GameObject paintsprayGate;
    //Couple puzzle stuff
    public GameObject graffitiSpot;
    public GameObject couple;
    public Sprite happyCouple;
    public GameObject graffitiBlockade;
    public GameObject brokenHeart;
    public GameObject fullHeart;
    public MinigameManager coupleManager;
    public GameObject drawingMinigameCouple;
    [SerializeField] private GameObject graffitiTutorialNote;
    [SerializeField] GameObject customTagMaker;

    //A bunch of UI stuff
    [Header("UI Prefabs")]
    [SerializeField] private TextMeshProUGUI swapText;
    [SerializeField] private TextMeshProUGUI graffitiText;
    [SerializeField] private TextMeshProUGUI selectionText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] private TextMeshProUGUI cheekyText;
    [SerializeField] private Image sprayCanOnUI;
    [SerializeField] private Image sprayCanOffUI;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private TextMeshProUGUI controlIndicator;


    [Header("Player")]
    public int playerActive = 1; //1 = realPlayer, 2 = graffitiPlayer
    [Tooltip("The camera's position, the higher the number the higher above the player")]
    public float cameraOffset = 2;

    //These are used in checking collision so you can't swap characters if you would end up inside collision
    //NOTE: This doesn't actually work I think. It doesn't matter much currently, you snap out of collision when you swap inside of it anyways
    [SerializeField] private LayerMask toGraffiti;
    [SerializeField] private LayerMask toReal;

    [Space, Header("Swapping")]
    public float defaultSwapCooldown = 1.2f;
    public float swapCooldown = 0;
    [Tooltip("If enabled, player can only swap forms while on the ground")]
    public bool groundedSwap = false; //Enable to forbid swapping in the air
    private bool hasShiftedBefore = false;

    [Space, Header("Graffiti-ing")]
    public float defaultGraffitiCooldown = 1f;
    public float graffitiCooldown = 0;
    public bool canGraffiti = false;
    public bool playerNearGraffiti = false;
    public float proximityThreshold = 1f; //How close player must be to graffiti to be able to interact with it
    private bool playerInMinigame = false;

    [Space, Header("World")]
    //List of all warp points, IE places the player may be teleported to. Could be used if say we want the player to enter a building through a door or similar
    public List<GameObject> warps = new List<GameObject>();
    private bool coupleMinigameCompleted = false;

    [Space, Header("Meta")]
    public float timer = 0;
    public bool gameRunning = false;
    public bool gameHasStarted = false;
    public bool gameWon = false;
    public float bestTime = 0;

    void Start()
    {
        realPlayer.rb.simulated = false;
        graffitiPlayer.rb.simulated = false;
    }


    void Update()
    {
        if (gameRunning)
        {
            //Timer update
            timer += Time.deltaTime;
            timerText.text = "Time: " + (int)timer;

            //Camera position update
            if (playerActive == 1 && playerInMinigame == false)
            {
                cameraPoint.transform.position = new Vector2(realPlayer.transform.position.x, realPlayer.transform.position.y + cameraOffset);
                if (canGraffiti) isPlayerNearGraffitiSpot();
            }
            else if (playerActive == 2)
            {
                cameraPoint.transform.position = new Vector2(graffitiPlayer.transform.position.x, graffitiPlayer.transform.position.y + cameraOffset);
            }

            //Swap cooldown update
            if (swapCooldown > 0)
            {
                swapCooldown -= Time.deltaTime;
                if (swapCooldown < 0) swapCooldown = 0;
            }
            if (swapCooldown == 0) swapText.text = "Swap ready! [SHIFT]";
            else swapText.text = "Recharge in " + (int)swapCooldown;

            if (graffitiCooldown > 0)
            {
                graffitiCooldown -= Time.deltaTime;
                if (graffitiCooldown < 0) graffitiCooldown = 0;
            }
            if (graffitiCooldown == 0) graffitiText.text = "Tag ready! [E]";
            else graffitiText.text = "Recharge in " + (int)graffitiCooldown;
            
            if (Input.GetKeyDown(KeyCode.E) && playerInMinigame == false)
            {
                makeGraffiti();
            }
        }
    }

    public void makeGraffiti() //1 = platform, 2 = warp
    {
        if (playerActive == 1 && graffitiCooldown <= 0 && playerNearGraffiti && coupleMinigameCompleted == false) //Player is in the real world and can graffiti
        {
            coupleManager.StartMinigameOne();
            playerInMinigame = true;
            cameraPoint.transform.position = new Vector2(drawingMinigameCouple.transform.position.x, drawingMinigameCouple.transform.position.y + cameraOffset);
            cameraPoint.GetComponentInChildren<Camera>().orthographicSize = 5f;
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
            selectionText.text = "[e] Tag!";
        }
        else if (playerActive == 1)
        {
            playerNearGraffiti = false;
            sprayCanOffUI.gameObject.SetActive(true);
            sprayCanOnUI.gameObject.SetActive(false);
            selectionText.text = "Can't Tag";
        }
    }

    public void couplePuzzleDone()
    {
        fullHeart.SetActive(true);
        couple.GetComponent<SpriteRenderer>().sprite = happyCouple;
        couple.GetComponent<BoxCollider2D>().isTrigger = true;
        graffitiBlockade.SetActive(false);
        graffitiSpot.SetActive(false);
        playerInMinigame = false;
        cameraPoint.GetComponentInChildren<Camera>().orthographicSize = 3f;
        coupleMinigameCompleted = true;
        graffitiTutorialNote.SetActive(false);
    }

    //Was used by the warps in my demo, I'm keeping it in since we may be able to reuse it for area transitions or similar -Nico
    public void warpPlayer(GameObject warpHit)
    {
        if (warps.Count > 1)
        {
            int currentIndex = warps.IndexOf(warpHit);
            int nextIndex = currentIndex + 1;
            if (nextIndex >= warps.Count) nextIndex = 0;
            realPlayer.transform.position = warps[nextIndex].transform.position;
            realPlayer.gameObject.SetActive(true);
            graffitiPlayer.gameObject.SetActive(false);
            playerActive = 1;
            swapCooldown = 0;
        }
    }

    //Was used by the fire obstacles in my demo. Keeping it in in case we want a harmful graffiti object. -Nico
    //Forces the player into 'real' mode, with a longer cooldown than normal before being able to swap
    public void hitFire()
    {
        if (playerActive == 2)
        {
            realPlayer.transform.position = graffitiPlayer.col.bounds.center;
            realPlayer.gameObject.SetActive(true);
            graffitiPlayer.gameObject.SetActive(false);
            playerActive = 1;
            swapCooldown = 2.5f;
            if (canGraffiti)
            {
                isPlayerNearGraffitiSpot();
                selectionText.gameObject.SetActive(true);
            }
        }
    }

    //Was used to swap the type of tag you made in my demo. Could be used if we want to let the player swap paint colors? -Nico
    public void updateSelectionUI(int selected)
    {
        switch (selected)
        {
            case 1: 
                break;
            case 2: 
                break;
            case 0: 
            default:
                break;
        }
    }

    //Ditto as above, was used to update the UI of the big paint can.
    public void updatePaintcanUI()
    {
        
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
                        if (canGraffiti)
                        {
                            isPlayerNearGraffitiSpot();
                            selectionText.gameObject.SetActive(true);
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

    //Triggered when the player picks up the paint spray
    public void getPaintspray()
    {
        paintsprayGate.SetActive(true);
        sprayCanOffUI.gameObject.SetActive(true);
        selectionText.gameObject.SetActive(true);
        canGraffiti = true;
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
            if (gameWon) //game ended
            {
                //Most of this I've left unchanged from my original demo, I'll update it later, it's a low priority -Nico
                cheekyText.gameObject.SetActive(true);
                if (timer < bestTime)
                {
                    menuText.text = "Congratulations!\nYou reached the end of the game :)\nYour time was " + (int)timer + "!\nThat's a new best time!!! :D\nPress Restart to play again";
                    bestTime = timer;
                    bestTimeText.text = "Best: " + (int)bestTime;
                }
                else
                {
                    if (bestTime > 0)
                        menuText.text = "Congratulations!\nYou reached the end of the game :)\nYour time was " + (int)timer + "\nYour best time is " + (int)bestTime + ".\nPress Restart to play again";
                    else
                    {
                        menuText.text = "Congratulations!\nYou reached the end of the game :)\nYour time was " + (int)timer + "!\nThat is now your time to beat ;)\nPress Restart to play again";
                        bestTime = timer;
                        bestTimeText.text = "Best: " + (int)bestTime;
                        bestTimeText.gameObject.SetActive(true);
                    }
                }
                restartButton.gameObject.SetActive(true);
                if (bestTime == 0) bestTime = timer;
            }
            else //game paused
            {
                menuText.text = "";
                resumeButton.gameObject.SetActive(true);
            }
        }
    }

    //Unpauses / starts the game
    public void resumeGame()
    {
        //Some initial game setup
        if (gameHasStarted == false)
        {
            gameHasStarted = true;
            timer = 0;
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
            //paintcanUI.gameObject.SetActive(true);
            //selectionText.gameObject.SetActive(true);
            //swapText.gameObject.SetActive(true);
            //graffitiText.gameObject.SetActive(true);
            //timerText.gameObject.SetActive(true);
            //controlIndicator.gameObject.SetActive(true);

        }
        gameRunning = true;
        realPlayer.rb.simulated = true;
        graffitiPlayer.rb.simulated = true;
        resumeButton.gameObject.SetActive(false);
        menuText.gameObject.SetActive(false);
    }

    public void EndCustomTagCreation()
    {
        gameRunning = true;
        customTagMaker.SetActive(false);
        realPlayer.rb.simulated = true;
        graffitiPlayer.rb.simulated = true;
    }

    public void restartGame()
    {
        restartButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        gameRunning = true;
        realPlayer.rb.simulated = true;
        graffitiPlayer.rb.simulated = true;
        gameHasStarted = true;
        timer = 0;
        gameWon = false;
        menuText.gameObject.SetActive(false);
        cheekyText.gameObject.SetActive(false);
        realPlayer.transform.position = new Vector2(-4.76999998f, -3.04999995f);
        realPlayer.gameObject.SetActive(true);
        graffitiPlayer.gameObject.SetActive(false);
        playerActive = 1;

        sprayCanOffUI.gameObject.SetActive(false);
        sprayCanOnUI.gameObject.SetActive(false);
        selectionText.gameObject.SetActive(false);
        controlIndicator.gameObject.SetActive(false);

        //Used to remove graffiti made by the player when restarting. Commenting it out for now until I got that function in the game -Nico
        /*
        GraffitiScript[] graffitis= FindObjectsOfType<GraffitiScript>();
        for (int x = 0; x < graffitis.Length; x++)
        {
            GraffitiScript a = graffitis[x];
            if (a.defaultSpawn == false && (a.type == 1 || a.type == 2))
            {
                Destroy(a.gameObject);
            }
        }*/
    }
}

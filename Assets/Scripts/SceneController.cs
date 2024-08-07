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
    public List<InteractableObject> interactables = new List<InteractableObject>();
    public List<GameObject> graffitiSpots = new List<GameObject>();
    public PuzzleHandler puzzleHandler;

    //Minigame / tagging stuff
    [SerializeField] GameObject customTagMaker;
    [SerializeField] MinigameManager customTagMinigame;
    [SerializeField] CustomScreenshotter customScreenshotter;

    //A bunch of UI stuff
    [Header("UI Prefabs")]
    public TextMeshProUGUI swapText;
    [SerializeField] private TextMeshProUGUI selectionText;
    [SerializeField] private TextMeshProUGUI menuText;
    [SerializeField] private Image sprayCanOnUI;
    [SerializeField] private Image sprayCanOffUI;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button endTagButton;
    public GameObject shiftTutorialPrompt;
    [SerializeField] GameObject customTagUI;
    [SerializeField] GameObject endText;

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
    [SerializeField] AudioSource swapSound;

    [Space, Header("Graffiti-ing")]
    public bool canGraffiti = true;
    public bool playerNearGraffiti = false;
    public float proximityThreshold = 1f; //How close player must be to graffiti to be able to interact with it
    public bool playerInMinigame = false;

    [Space, Header("World & Progression")]
    public bool canSwap = false;
    private InteractableObject nearestInteractable; //the closest interactable to the player
    [SerializeField] private bool inRangeOfInteractable = false;
    public bool hasUsedFirstLift = false;
    public bool hasUsedSecondLift = false;
    public bool inFinalRoom = false;
    public bool gameBeaten = false;
    [SerializeField] GameObject secretWall;

    [Space, Header("Meta")]
    public bool gameRunning = false;
    public bool gameHasStarted = false;
    public int currentCutscene = 0; //0 = standard gameplay. Updated in SceneController and CutsceneHandler

    void Start()
    {
        realPlayer.rb.simulated = false;
        graffitiPlayer.rb.simulated = false;
        nearestInteractable = FindFirstObjectByType<InteractableObject>();
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
                checkNearestInteractable();
            }
            if (swapCooldown > 0)
            {
                swapCooldown -= Time.deltaTime;
                if (swapCooldown < 0) swapCooldown = 0;
            }
            if (swapCooldown == 0) swapText.text = "Swap [SHIFT]";
            else swapText.text = "Swap [SHIFT]";//"Wait "+ (int)swapCooldown;

            if (Input.GetKeyDown(KeyCode.E) && playerInMinigame == false && playerActive == 1)
            {
                //Attempt to graffiti. If fail, attempt to interact with an object
                if (makeGraffiti() == false)
                {
                    if (inRangeOfInteractable)
                    {
                        nearestInteractable.interactedWith();
                    }
                }
            }
        }
        //Pause/Unpause game (Esc / Backspace)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            /*
            if (gameBeaten)
                Application.Quit();
            else
                pauseGame();*/
        }
    }

    public bool makeGraffiti()
    {
        if (playerNearGraffiti) //Player is in the real world and can graffiti
        {
            currentCutscene = 1;
            playerInMinigame = true;
            puzzleHandler.startMinigame();
            return true;
        }
        else return false;
    }

    //NOTE! This is currently very hardcoded and only for the one puzzle, tis a Wizard-Of-Oz setup for the demo, to be changed later
    public void isPlayerNearGraffitiSpot()
    {
        //player is close enough
        if (Vector2.Distance(realPlayer.transform.position, puzzleHandler.nextGraffitiSpot.transform.position) <= proximityThreshold && playerActive == 1)
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

    public void checkNearestInteractable()
    {
        for (int x = 0; x < interactables.Count; x++)
        {
            if (Vector2.Distance(interactables[x].transform.position, realPlayer.transform.position) <= Vector2.Distance(nearestInteractable.transform.position, realPlayer.transform.position))
            {
                nearestInteractable = interactables[x];
            }
            interactables[x].prompt.SetActive(false);
        }
        if (Vector2.Distance(nearestInteractable.transform.position, realPlayer.transform.position) < 1)
        {
            inRangeOfInteractable = true;
            nearestInteractable.prompt.SetActive(true);
        }
        else inRangeOfInteractable = false;
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
                        swapSound.Play();
                        realPlayer.gameObject.SetActive(false);
                        playerActive = 2;

                        swapCooldown = defaultSwapCooldown;
                        if (hasShiftedBefore == false)
                        {
                            shiftTutorialPrompt.SetActive(false);
                            currentCutscene = 0;
                            canSwap = false;
                            hasShiftedBefore = true;
                            secretWall.SetActive(true);
                        }
                        sprayCanOffUI.gameObject.SetActive(false);
                        sprayCanOnUI.gameObject.SetActive(false);
                        selectionText.gameObject.SetActive(false);

                        graffitiPlayer.GetComponent<Rigidbody2D>().simulated = false;
                        currentCutscene = 2;
                        List<Vector2> destinations = new List<Vector2>() { Vector2.zero };
                        List<float> timers = new List<float>() { 0.25f };
                        List<float> zooms = new List<float>();
                        List<float> moveSpeed = new List<float>() { 0 };
                        List<float> zoomSpeed = new List<float>() { 5f };

                        if (hasUsedFirstLift && !hasUsedSecondLift)
                            zooms.Add(3.7f);
                        else if (hasUsedSecondLift)
                            zooms.Add(6.5f);
                        else
                            zooms.Add(2.5f);
                        cutsceneHandler.startCutscene(timers, destinations, zooms, moveSpeed, zoomSpeed);

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
                        //swapSound.Play();
                        graffitiPlayer.gameObject.SetActive(false);
                        playerActive = 1;
                        swapCooldown = defaultSwapCooldown;

                        realPlayer.GetComponent<Rigidbody2D>().simulated = false;
                        currentCutscene = 2;
                        List<Vector2> destinations = new List<Vector2>() { Vector2.zero };
                        List<float> timers = new List<float>() { 0.25f }; //was 0.5
                        List<float> zooms = new List<float>();
                        List<float> moveSpeed = new List<float>() { 0 };
                        List<float> zoomSpeed = new List<float>() { 5f }; //was 2.5

                        if (hasUsedFirstLift && !hasUsedSecondLift)
                            zooms.Add(4.2f);
                        else if (hasUsedSecondLift)
                            zooms.Add(7f);
                        else
                            zooms.Add(3f);
                        cutsceneHandler.startCutscene(timers, destinations, zooms, moveSpeed, zoomSpeed);

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
        customTagUI.SetActive(true);
        customTagMaker.gameObject.SetActive(true);
        startButton.gameObject.SetActive(false);
        endTagButton.gameObject.SetActive(true);
        menuText.gameObject.SetActive(false);
    }

    public void MakeTag()
    {
        customTagUI.SetActive(false);
        customScreenshotter.MakeTag();
        swapSound.Play();
    }

    //Triggered when the player places their final tag
    public void finalCutscene()
    {
        currentCutscene = 2;
        List<Vector2> destinations = new List<Vector2>() { Vector2.zero, new Vector2(realPlayer.transform.position.x, realPlayer.transform.position.y + 7), new Vector2(realPlayer.transform.position.x, realPlayer.transform.position.y + 20) };
        List<float> timers = new List<float>() { 1.5f, 3f, 7f };
        List<float> zooms = new List<float>() { 0, 10, 10};
        List<float> moveSpeed = new List<float>() { 0, 0.3f, 1.2f};
        List<float> zoomSpeed = new List<float>() { 0, 0.1f, 0.3f};
        cutsceneHandler.startCutscene(timers, destinations, zooms, moveSpeed, zoomSpeed);
    }

    public void endTheGame()
    {
        gameBeaten = true;
        //menuText.gameObject.SetActive(true);
        //menuText.color = Color.white;
        //menuText.text = "Congratulations! \nYou have reached the end of the game. Thanks to you, the city will wake up with a little more color than before... \n \nPress ESC now to close the game.";
    }

    // ends the tag creation and activates player
    public void EndCustomTagCreation()
    {
        gameRunning = true;
        customTagMinigame.TagComplete();
        endTagButton.gameObject.SetActive(false);
        realPlayer.rb.simulated = true;
        graffitiPlayer.rb.simulated = true;
        selectionText.gameObject.SetActive(true);
        //MakeTag();

        cutsceneHandler.changeCamera(realPlayer.transform.position, 1f);
        currentCutscene = 2;
        realPlayer.animator.SetBool("isJumping", false);
        realPlayer.animator.SetFloat("xVelocity", 0);
        realPlayer.animator.SetFloat("yVelocity", 0);
        List<Vector2> destinations = new List<Vector2>() { Vector2.zero, Vector2.zero };
        List<float> timers = new List<float>() { 1f, 1.5f};
        List<float> zooms = new List<float>() { 0, 3 };
        List<float> moveSpeed = new List<float>() { 0, 0 };
        List<float> zoomSpeed = new List<float>() { 0, 1.3f };
        cutsceneHandler.startCutscene(timers, destinations, zooms, moveSpeed, zoomSpeed);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    private Player player;
<<<<<<< Updated upstream
    //private Enemy enemy;
    private FightManager fightManager;
    public int maxNumTurns;  // here for the example fight handler
    bool playerTurn = true; //start with player turn
    int turnNum = 0;
=======
    private Enemy enemy;
    private DeckManager deck;
    //private HUDManager HUD;
    //private Enemy enemy;
    private GameObject fightManager;
    public int NumBattles;
    public int MaxFights;
    public List<Card> playerDeck = new List<Card>();
    public List<Card> enemyDeck = new List<Card>();
    public bool firstLoad = true;
>>>>>>> Stashed changes


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);  // not sure if this is neccessary, but would be nice
    }

    public static GameManager instance()
    {
        return _instance;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        switch (SceneManager.GetActiveScene().name)  // needs to be here to assure that player is initialized
        {
            case "FightScene":
                // InitiateFight();  // maybe change to initiate a fight in a fight manager
                // don't need to call it b/c it will be iitialized with the scene
                break;
            case "ShopScene":
            default: break;
        }
    }

    //Scene Functions
    public void LoadShop()
    {
        playerDeck = player.deck;
        fightManager = GameObject.Find("FightManager");
        fightManager.SetActive(false);
        deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<DeckManager>();
        //HUD = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>();
        deck.DisableDeckChildren();
        //HUD.DisableHUDChildren();
        deck.UnparentDeck();
        //HUD.UnparentHUD();
        DontDestroyOnLoad(deck.gameObject);
        //DontDestroyOnLoad(HUD.gameObject);
        SceneManager.LoadScene("Shop");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadWin()
    {
        SceneManager.LoadScene("WinScreen");
    }

    public void LoadLoss()
    {
        SceneManager.LoadScene("LoseScreen");
    }

    public void LoadFightScene()
    {
        playerDeck = player.deck;
        deck = GameObject.FindGameObjectWithTag("Deck").GetComponent<DeckManager>();
        //HUD = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>();
        deck.DisableDeckChildren();
        //HUD.EnableHUDChildren();
        deck.UnparentDeck();
        //HUD.UnparentHUD();
        DontDestroyOnLoad(deck.gameObject);
        //DontDestroyOnLoad(HUD.gameObject);
        SceneManager.LoadScene("FightScene");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
<<<<<<< Updated upstream
=======
        Start();
        if (scene.name == "Shop")
        {
            deck.ReparentDeckToCanvas();
            //HUD.ReparentHUDToCanvas();
            // Unsubscribe from the event to prevent memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        else if(scene.name == "FightScene" && deck!=null /*&&HUD!=null*/)
        {
            deck.ReparentDeckToCanvas();
            //HUD.ReparentHUDToCanvas();
            fightManager.SetActive(true);
            // Unsubscribe from the event to prevent memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
>>>>>>> Stashed changes
    }

    // initiates turn order
    void InitiateFight()
    {
        Debug.Log("Fight Initiated >:)");
        // get enemy in fight here
        //enemy = null;

        // example of how a fight manager might handle the turns
        
        // get enemy
        while (player.IsAlive() && turnNum < maxNumTurns)
        {
            if (playerTurn)
            {
                // do player turn here
                playerTurn = false;
                turnNum++;
            }
            else
            {
                // do enemy turn here

                playerTurn = true;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    private Player player;
    //private Enemy enemy;
    public int maxNumTurns;  // here for the example fight handler
    private Enemy enemy;
    private DeckManager deck;
    private HUDManager HUD;
    //private Enemy enemy;
    private GameObject fightManager;
    public int NumBattles;
    public int MaxFights;
    public List<Card> playerDeck = new List<Card>();
    public List<Card> enemyDeck = new List<Card>();
    public bool firstLoad = true;
    public AudioSource theme;
    private bool isPlaying;

    public AudioSource fightSound;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        NumBattles = MaxFights;
        isPlaying = false;
    }

    public static GameManager instance()
    {
        return _instance;
    }

    void Start()
    {
        switch (SceneManager.GetActiveScene().name)  // needs to be here to assure that player is initialized
        {
            case "MainMenu":
                Debug.Log("MainMenu");
                if (!isPlaying)
                {
                    Debug.Log("Playing theme");
                    Debug.Log(isPlaying);
                    theme.Play();
                    isPlaying = true;
                }
                break;
            case "FightScene":
                if (player == null)
                {
                    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                    playerDeck = player.deck;
                }
                else
                {
                    player.deck = playerDeck;
                }
                enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
                enemyDeck = enemy.enemyDeck;
                NumBattles--;
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
        HUD = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>();
        deck.DisableDeckChildren();
        HUD.DisableHUDChildren();
        deck.UnparentDeck();
        HUD.UnparentHUD();
        DontDestroyOnLoad(deck.gameObject);
        DontDestroyOnLoad(HUD.gameObject);
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
        HUD = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>();
        deck.DisableDeckChildren();
        HUD.EnableHUDChildren();
        deck.UnparentDeck();
        HUD.UnparentHUD();
        DontDestroyOnLoad(deck.gameObject);
        DontDestroyOnLoad(HUD.gameObject);
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
        Start();
        if (scene.name == "Shop")
        {
            deck.ReparentDeckToCanvas();
            HUD.ReparentHUDToCanvas();
            // Unsubscribe from the event to prevent memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        else if (scene.name == "FightScene" && deck != null && HUD!=null)
        {
            deck.ReparentDeckToCanvas();
            HUD.ReparentHUDToCanvas();
            fightManager.SetActive(true);
            // Unsubscribe from the event to prevent memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void playFightSound()
    {
        fightSound.Play();
    }
}

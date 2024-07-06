using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    private Player player;
    private Enemy enemy;
    //private Enemy enemy;
    private FightManager fightManager;
    public int NumBattles;
    public int MaxFights;
    public List<Card> playerDeck = new List<Card>();
    public List<Card> enemyDeck = new List<Card>();


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        NumBattles = MaxFights;
    }

    public static GameManager instance()
    {
        return _instance;
    }

    void Start()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "FightScene":
                if(player == null)
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
            default: break;
        }
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
    }

    public void AddCardToPlayerDeck(Card c)
    {
        playerDeck.Add(c);
    }
    public List<Card> GetEnemyDeck()
    {
        return enemyDeck;
    }
}

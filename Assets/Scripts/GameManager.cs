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
    private FightManager fightManager;
    public int maxNumTurns;  // here for the example fight handler
    bool playerTurn = true; //start with player turn
    int turnNum = 0;

    //deck variables, arrays, and lists
    public List<Card> deck = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    //action point variables
    public int actionPoints;
    public int numOfActionPoints;

    public Image[] actionPointsPips;
    public Sprite APFull;
    public Sprite APEmpty;


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
    }

    // initiates turn order
    void InitiateFight()
    {
        Debug.Log("Fight Initiated >:)");
        // get enemy in fight here
        enemy = null;

        // example of how a fight manager might handle the turns
        
        // get enemy
        while (player.IsAlive() && turnNum < maxNumTurns)
        {
            if (playerTurn)
            {
                // do player turn here
                DrawCards();
                AddActionPoints();
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

    public void EndTurn()
    {
        Debug.Log("End Turn");
    }

    public void DrawCards()
    {
        for (int totalDraw = 0; totalDraw < 3; totalDraw++) //always tries to draw 3 cards
        {
            if (deck.Count >= 1)
            {
                Card randCard = deck[Random.Range(0, deck.Count)];
                for (int i = 0; i < availableCardSlots.Length; i++)
                {
                    if (availableCardSlots[i] == true)
                    {
                        randCard.gameObject.SetActive(true);
                        randCard.transform.position = cardSlots[i].position;
                        availableCardSlots[i] = false;
                        deck.Remove(randCard);
                        return;
                    }
                }
            }
        }
    }

    public void AddActionPoints()
    {
        actionPoints = actionPoints + 3; //adds 3 ap everytime it is called
        for(int i=0; i < actionPointsPips.Length; i++)
        {
            if (i < actionPoints)
            {
                actionPointsPips[i].sprite = APFull;
            }
            else
            {
                actionPointsPips[i].sprite = APEmpty;
            }
            /*
            if(i<numOfActionPoints)
            {
                actionPointsPips[i].enabled = true;
            }
            else
            {
                actionPointsPips[i].enabled = false;
            }*/
        }
    }
}

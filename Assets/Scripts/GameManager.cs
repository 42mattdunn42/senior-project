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
    private FightManager fightManager;
    public int maxNumTurns;  // here for the example fight handler
    bool playerTurn = true; //start with player turn
    int turnNum = 0;


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
        switch (SceneManager.GetActiveScene().name)
        {
            case "FightScene":
                if(player == null)
                {
                    player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                }
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

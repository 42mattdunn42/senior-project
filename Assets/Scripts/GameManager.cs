using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    private Player player;
    private Enemy enemy;
    public int maxNumTurns;  // here for the example fight handler

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
                InitiateFight();  // maybe change to initiate a fight in a fight manager
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
        bool playerTurn = true;
        int turnNum = 0;
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

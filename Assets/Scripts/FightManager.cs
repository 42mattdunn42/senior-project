using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FightManager : MonoBehaviour
{
    private DiceRoller roller;
    private Player player;
    private Enemy enemy;
    bool playerTurn = true; //start with player turn
    bool playerAutomaticActions = false; //checks if the automatic actions have been completed yet
    bool enemyAutomaticActions = false; //same but for enemy

    //deck variables, arrays, and lists
    public List<Card> deck = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    //enemy deck variables, arrays, and lists
    public List<Card> enemyDeck = new List<Card>();
    public Transform[] enemyCardSlots;
    public bool[] enemyAvailableCardSlots;

    //action point variables
    public int actionPoints;
    public int numOfActionPoints;
    private int maxActionPoints = 5;

    public Image[] actionPointsPips;
    public Sprite APFull;
    public Sprite APEmpty;

    //enemy action point variables
    public int enemyActionPoints;
    public int enemyNumOfActionPoints;
    private int maxEnemyActionPoints = 5;

    public Image[] enemyActionPointsPips;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        roller = GameObject.FindGameObjectWithTag("Roller").GetComponent<DiceRoller>();
        enemy = new Enemy(100);  // for now until we have an enemy
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTurn && player.IsAlive() && enemy.IsAlive())  // player turn
        {
            // clear dice results
            // roller.ClearDice();

            // draw cards
            // add AP
            if(!playerAutomaticActions)
            {
                for (int i = 0; i<3; i++)
                {
                    DrawCards();
                }
                AddActionPoints();
                playerAutomaticActions = true;
            }
            // calculate and deal damage
            //enemy.TakeDamage(CalculateDamage());  // dice rolling is currently occuring here. Cards currently have no effect
            //Debug.Log(CalculateDamage());
            //playerTurn = false;
        }
        else if(!playerTurn && player.IsAlive() && enemy.IsAlive())  // enemy turn
        {
            // clear dice results
            // roller.ClearDice();

            // enemy stuff
            if (!enemyAutomaticActions)
            {
                for (int i = 0; i < 3; i++)
                {
                    DrawCards();
                }
                AddActionPoints();
                enemyAutomaticActions = true;
            }
            // calculate and deal damage
            //player.TakeDamage(CalculateDamage());  // dice rolling is currently occuring here
            //Debug.Log(CalculateDamage());
            playerAutomaticActions = false;
            enemyAutomaticActions = false;
            playerTurn = true;

        }
        else  // someone was defeated
        {
            if (!player.IsAlive())  // player lost
            {
                // do lose stuff
                Debug.Log("Enemy Won");
            }
            else  // player won
            {
                // do win stuff
                Debug.Log("Player Won");
            }
        }
    }

    private int CalculateDamage()
    {
        // roll dice and get results
        roller.Roll();
        int[] rolls = roller.results;
        Array.Sort(rolls);

        /* Valid Damage Rolls
         * 3 of a kind: add total of dice in three of a kind (ex: a roll of 3,3,3,1,6 results in 9 damage)
         * 4 of a kind: add total of dice in four of a kind (ex: a roll of 4,4,4,4,2,5 results in 16 damage)
         * Full House: 25 damage (ex: 2,2,2,3,3)
         * Small Straight: 30 damage (ex: 1,2,3,4 and 2,3,4,5 and 3,4,5,6)
         * Large Straight: 40 damage (ex 1,2,3,4,5 and 2,3,4,5,6)
         * 5 of a kind: 50 damage (ex: 1,1,1,1,1)
         */

        // currently ignoring card effects

        // check for straights
        int maxNumConsecutive = 0;
        int currCount = 1;
        for (int i = 1; i < rolls.Length; i++)
        {
            if (rolls[i] == rolls[i - 1] + 1)
            {
                currCount++;
            }
            else if (rolls[i] != rolls[i - 1])
            {
                currCount = 1;
            }

            if (currCount > maxNumConsecutive) { maxNumConsecutive = currCount; }
        }
        if (maxNumConsecutive == 5)
        {
            return 40;
        }
        else if (maxNumConsecutive == 4)
        {
            return 30;
        }

        // Check for x of a kind
        int maxNumSame = 0;
        int val = 0;
        for (int i = 0; i < rolls.Length; i++)
        {
            int numSame = 0;
            for(int k = 0; k < rolls.Length; k++)
            {
                if (rolls[i] == rolls[k])
                {
                    numSame++;
                }
            }
            if (numSame > maxNumSame)
            {
                maxNumSame = numSame;
                val = rolls[i];
            }
        }

        if (maxNumSame == 5)  // 5 of a kind
        {
            return 50;
        }
        else if (maxNumSame == 4)  // 4 of a kind
        {
            return 4 * val;
        }
        else if (maxNumSame == 3)  // determine if 3 of a kind or full house
        {
            // check for a pair (full house)
            for (int i = 0; i < rolls.Length; i++)
            {
                int numSame = 0;
                for (int k = 0; k < rolls.Length; k++)
                {
                    if (rolls[i] != val && rolls[i] == rolls[k])
                    {
                        numSame++;
                    }
                    if (numSame >= 2)
                    {
                        return 25;
                    }
                }
            }

            // 3 of a kind
            return 3 * val;
        }

        return 0;
    }

    public void DebugCalculatedamage()
    {
        Debug.Log(CalculateDamage());
    }

    public void EndTurn()
    {
        Debug.Log("End Turn");
        playerTurn = false;
    }

    public void DrawCards()
    {
        if (playerTurn == true)
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
        else if(playerTurn==false) 
        {
            if (enemyDeck.Count >= 1)
            {
                Card randCard = enemyDeck[Random.Range(0, enemyDeck.Count)];
                for (int i = 0; i < enemyAvailableCardSlots.Length; i++)
                {
                    if (enemyAvailableCardSlots[i] == true)
                    {
                        randCard.gameObject.SetActive(true);
                        randCard.transform.position = enemyCardSlots[i].position;
                        enemyAvailableCardSlots[i] = false;
                        enemyDeck.Remove(randCard);
                        return;
                    }
                }
            }
        }
    }

    public void AddActionPoints()
    {
        if(playerTurn==true)
        {
            if (actionPoints <= 2)
            {
               actionPoints = actionPoints + 3;
            }
            else
            {
                actionPoints = maxActionPoints;
            }
            for (int i = 0; i < actionPointsPips.Length; i++)
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
        if(playerTurn==false) //add AP for enemy turns
        {
            if (enemyActionPoints <= 2)
            {
                enemyActionPoints = enemyActionPoints + 3;
            }
            else
            {
                enemyActionPoints = maxEnemyActionPoints;
            }
            for (int i = 0; i < enemyActionPointsPips.Length; i++)
            {
                if (i < enemyActionPoints)
                {
                    enemyActionPointsPips[i].sprite = APFull;
                }
                else
                {
                    enemyActionPointsPips[i].sprite = APEmpty;
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
}

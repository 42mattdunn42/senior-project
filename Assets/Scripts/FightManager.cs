using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using TMPro;

public class FightManager : MonoBehaviour
{
    private DiceRoller roller;
    private Player player;
    private Enemy enemy;
    public bool playerTurn = true; //start with player turn
    bool playerAutomaticActions = false; //checks if the automatic actions have been completed yet
    bool enemyAutomaticActions = false; //same but for enemy
    bool firstTurnActions = false;

    //deck variables, arrays, and lists
    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    //enemy deck variables, arrays, and lists
    public List<Card> enemyDeck = new List<Card>();
    public Transform[] enemyCardSlots;
    public bool[] enemyAvailableCardSlots;

    //action point variables
    public int actionPoints;
    public int numOfActionPoints;
    public int maxActionPoints = 5;

    public Image[] actionPointsPips;
    public Sprite APFull;
    public Sprite APEmpty;

    //enemy action point variables
    public int enemyActionPoints;
    public int enemyNumOfActionPoints;
    private int maxEnemyActionPoints = 5;

    public Image[] enemyActionPointsPips;

    public TextMeshProUGUI diceresult;

    // Damage delay
    private int damageDelay = 0;  // must be initialized to zero to prevent damage from being dealt prematurely
    public TextMeshProUGUI incomingDamage;
    public TextMeshProUGUI outgoingDamage;

    // Shield
    public RawImage playerShield;
    public RawImage enemyShield;
    


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        roller = GameObject.FindGameObjectWithTag("Roller").GetComponent<DiceRoller>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTurn && player.IsAlive() && enemy.IsAlive())  // player turn
        {
            // draw cards
            // add AP
            // Roll dice
            if(!playerAutomaticActions)
            {
                for (int i = 0; i<3; i++)
                {
                    DrawCards();
                }
                AddActionPoints();
                UpdateActionPoints();
                roller.Roll();
                playerAutomaticActions = true;
            }

            //playerTurn = false;
        }
        else if(!playerTurn && player.IsAlive() && enemy.IsAlive())  // enemy turn
        {
            // enemy stuff
            if (!enemyAutomaticActions)
            {
                for (int i = 0; i < 3; i++)
                {
                    DrawCards();
                }
                AddActionPoints();
                UpdateActionPoints();
                roller.Roll();
                enemyAutomaticActions = true;
            }

            // damage delay stuff
            enemy.TakeDamage(damageDelay);
            damageDelay = CalculateDamage();
            incomingDamage.text = "Incoming: " + damageDelay;
            outgoingDamage.text = "";
            EndTurn();
        }
        else  // someone was defeated
        {
            if (!player.IsAlive())  // player lost
            {
                // do lose stuff
                SceneManager.LoadScene("LoseScreen");
            }
            else  // player won
            {
                // do win stuff
                SceneManager.LoadScene("WinScreen");
            }
        }
    }

    /// <summary>
    /// Calculates the damage from the dice rolling to be dealt.
    /// </summary>
    /// <returns>The amount of damage to be dealt</returns>
    public int CalculateDamage()
    {
        // get dice results
        int[] rolls = (int[])roller.results.Clone();
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
        int min = rolls[0];
        for (int i = 1; i < rolls.Length; i++)
        {
            if (rolls[i] == rolls[i - 1] + 1)
            {
                currCount++;
            }
            else if (rolls[i] != rolls[i - 1])  // checks for duplicates here
            {
                min = rolls[i];
                currCount = 1;
            }

            if (currCount > maxNumConsecutive) { maxNumConsecutive = currCount; }
        }
        if (maxNumConsecutive == 5)  // large straight
        {
            roller.ColorLargeStraight();
            diceresult.text = "Large Straight";
            return 40;
        }
        else if (maxNumConsecutive == 4)  // small straight
        {
            roller.ColorSmallStraight(min);
            diceresult.text = "Small Straight";
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
            roller.ColorXOfAKind(val);
            diceresult.text = "5 of a Kind";
            return 50;
        }
        else if (maxNumSame == 4)  // 4 of a kind
        {
            roller.ColorXOfAKind(val);
            diceresult.text = "4 of a Kind";
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
                        roller.ColorFullHouse(val);
                        diceresult.text = "Full House";
                        return 25;
                    }
                }
            }

            // 3 of a kind
            roller.ColorXOfAKind(val);
            diceresult.text = "3 of a Kind";

            return 3 * val;
        }

        diceresult.text = "";
        return 0;
    }

    public void DebugCalculatedamage()
    {
        Debug.Log(CalculateDamage());
    }

    public void EndTurn()
    {
        if (playerTurn)  // due to ddamage delay, at the end of the player's turn they will take damage and vice versa
        {
            player.TakeDamage(damageDelay);
            damageDelay = CalculateDamage();
            outgoingDamage.text = "Incoming: " + damageDelay;
            incomingDamage.text = "";
            Debug.Log("Player End Turn");
            playerTurn = false;
            playerAutomaticActions = false;
        }
        else
        {
            enemy.TakeDamage(damageDelay);
            damageDelay = CalculateDamage();
            incomingDamage.text = "Incoming: " + damageDelay;
            outgoingDamage.text = "";
            Debug.Log("Enemy End Turn");
            playerTurn = true;
            enemyAutomaticActions = false;
        }
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
                        randCard.handIndex = i;
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

    public void AddActionPoints() //Need to make a function that removes AP when card is played
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
        }
    }
    public void UpdateActionPoints()
    {
        if (playerTurn == true)
        {
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
            }
        }
        if (playerTurn == false) //add AP for enemy turns
        {
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
            }
        }
    }

    public void ActivateShield(int amt, int numTurns)
    {
        if (playerTurn)
        {
            player.shield = amt;
            player.shieldLength = numTurns;
            playerShield.gameObject.SetActive(true);
            playerShield.GetComponentInChildren<TextMeshProUGUI>().text = amt.ToString();
        }
        else
        {
            enemy.shield = amt;
            enemy.shieldLength = numTurns;
            enemyShield.gameObject.SetActive(true);
            enemyShield.GetComponentInChildren<TextMeshProUGUI>().text = amt.ToString();
        }
    }

    public void UpdateShield(bool IsPlayer, int amt)
    {
        if (IsPlayer)
        {
            playerShield.GetComponentInChildren<TextMeshProUGUI>().text = amt.ToString();
        }
        else
        {
            enemyShield.GetComponentInChildren<TextMeshProUGUI>().text = amt.ToString();
        }
    }

    public void DeactivateShield(bool IsPlayer)
    {
        if (IsPlayer)
        {
            playerShield.gameObject.SetActive(false);
        }
        else
        {
            enemyShield.gameObject.SetActive(false);
        }
        
    }
}

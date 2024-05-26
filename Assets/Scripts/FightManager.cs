using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FightManager : MonoBehaviour
{
    private DiceRoller roller;
    private Player player;
    private Enemy enemy;
    bool playerTurn = true; //start with player turn

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

            // calculate and deal damage
            //enemy.TakeDamage(CalculateDamage());  // dice rolling is currently occuring here. Cards currently have no effect
            //Debug.Log(CalculateDamage());
            playerTurn = false;
        }
        else if(!playerTurn && player.IsAlive() && enemy.IsAlive())  // enemy turn
        {
            // clear dice results
            // roller.ClearDice();

            // enemy stuff

            // calculate and deal damage
            //player.TakeDamage(CalculateDamage());  // dice rolling is currently occuring here
            //Debug.Log(CalculateDamage());
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FightManager : MonoBehaviour
{
    private Player player;
    private Enemy enemy;
    bool playerTurn = true; //start with player turn

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        enemy = null;  // for now until we have an enemy
        if (playerTurn && player.IsAlive() && enemy.IsAlive())  // player turn
        {
            // draw cards
            // add AP

            playerTurn = false;
        }
        else if(!playerTurn && player.IsAlive() && enemy.IsAlive())  // enemy turn
        {
            // enemy stuff
            playerTurn = true;
        }
        else  // someone was defeated
        {
            if (!player.IsAlive())  // player lost
            {
                // do lose stuff
            }
            else  // player won
            {
                // do win stuff
            }
        }
    }
}

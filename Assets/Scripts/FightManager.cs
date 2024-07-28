using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class FightManager : MonoBehaviour
{
    private DiceRoller roller;
    private Player player;
    private Enemy enemy;
    private GameManager gm;
    public bool playerTurn = true; //start with player turn
    bool playerAutomaticActions = false; //checks if the automatic actions have been completed yet
    bool enemyAutomaticActions = false; //same but for enemy

    //player deck
    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    //enemy deck
    public Transform[] enemyCardSlots;
    public bool[] enemyAvailableCardSlots;

    //action point variables
    public Image[] actionPointsPips;
    public Sprite APFull;
    public Sprite APEmpty;

    //enemy action point variables
    public Image[] enemyActionPointsPips;

    //card zones
    public RectTransform playRectTransform;
    public RectTransform burnRectTransform;
    public RectTransform upgradeRectTransform;

    public TextMeshProUGUI diceresult;

    // Damage delay
    public int damageDelay = 0;  // must be initialized to zero to prevent damage from being dealt prematurely
    public TextMeshProUGUI incomingDamage;
    public TextMeshProUGUI outgoingDamage;

    // Shield
    public RawImage playerShield;
    public RawImage enemyShield;

    //Card Effects
    public bool doubleDamage = false; //starts with damage being normal at start
    public bool reflectDamage = false;
    public int reflectAmount = 0;

    // pause menu and UI variables
    [SerializeField] public GameObject pauseMenu;
    private bool isPaused = false;
    [SerializeField] public GameObject helpMenu;
    private bool isHelp = false;
    public Button endTurnButton;

    public AudioSource clickSound;
    public AudioSource endTurnSound;
    public List<AudioSource> Draws;
    public List<AudioSource> Shuffles;
    public AudioSource playCardSound;
    public AudioSource burnCardSound;

    private void Awake()
    {
        FindObjects();
        FullReset();
    }
    // Start is called before the first frame update
    void Start()
    {
        //FindObjects();
        //FullReset();
        Shuffles[Random.Range(0, Shuffles.Count)].Play();
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
        //if (scene.name == "FightScene")
        //{
        //    gm = GameManager.instance();
        //    player.deck = gm.playerDeck;
        //    RemoveObjects();
        //    FindObjects();
        //}
        RemoveObjects();
        FindObjects();
        player.deck = gm.playerDeck;
    }


    public void FindObjects()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        roller = GameObject.FindGameObjectWithTag("Roller").GetComponent<DiceRoller>();
        enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
        endTurnButton = GameObject.Find("End Turn Button").GetComponent<Button>();
        playRectTransform = GameObject.Find("Play Area").GetComponent<RectTransform>();
        burnRectTransform = GameObject.Find("Burn Card Area").GetComponent<RectTransform>();
        incomingDamage = GameObject.FindGameObjectWithTag("IncomingDamage").GetComponent<TextMeshProUGUI>();
        outgoingDamage = GameObject.FindGameObjectWithTag("OutgoingDamage").GetComponent<TextMeshProUGUI>();
        //pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        //helpMenu = GameObject.FindGameObjectWithTag("HelpMenu");
    }

    public void RemoveObjects()
    {
        gm = null;
        player = null;
        roller = null;
        enemy = null;
        endTurnButton = null;
        playRectTransform = null;
        burnRectTransform = null;
        incomingDamage = null;
        outgoingDamage = null;
        //pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        //helpMenu = GameObject.FindGameObjectWithTag("HelpMenu");
    }

    // Update is called once per frame
    void Update()
    {
        // Pause menu stuff
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            clickSound.Play();
            if (isHelp)
            {
                ToggleHelpMenu();
            }
            else
            {
                isPaused = !isPaused;
                Time.timeScale = isPaused ? 0 : 1;
                pauseMenu.SetActive(isPaused);
            }
        }
        if (isPaused)
        {
            return;
        }

        //TURN CYCLING
        if (playerTurn && player.IsAlive() && enemy.IsAlive())  // player turn
        {
            // draw cards
            // add AP
            // Roll dice
            if (!playerAutomaticActions)
            {
                roller.ResetDiceNumbers();
                for (int i = 0; i < 3; i++)
                {
                    DrawCards();
                }
                AddActionPoints();
                UpdateActionPoints();
                roller.Roll();
                playerAutomaticActions = true;
            }

            if (player.bound == true) //If the player is bound, then end their turn immediately
            {
                EndTurn();
                player.bound = false;
            }
            //playerTurn = false;
        }
        else if (!playerTurn && player.IsAlive() && enemy.IsAlive())  // enemy turn
        {
            // enemy stuff
            if (!enemyAutomaticActions)
            {
                roller.ResetDiceNumbers();
                for (int i = 0; i < 3; i++)
                {
                    DrawCards();
                }
                AddActionPoints();
                UpdateActionPoints();
                roller.Roll();
                enemyAutomaticActions = true;

                if (enemy.bound == true) //If enemy is bound, they immediately end their turn
                {
                    EndTurn();
                    enemy.bound = false;
                }
                else
                {
                    if (gm.NumBattles == 1)
                    {
                        enemy.EnemyPlayLogic();
                    }
                    if (gm.NumBattles == 2)
                    {
                        enemy.EnemyPlayLogic2();
                    }
                    EndTurn();
                }
            }
        }
        else  // someone was defeated
        {
            if (!player.IsAlive())  // player lost
            {
                // do lose stuff
                //SceneManager.LoadScene("LoseScreen");
                //gm.NumBattles = gm.MaxFights;
                FullReset();
                gm.LoadLoss();
            }
            else  // player won
            {
                // do win stuff
                //SceneManager.LoadScene("WinScreen");
                gm.NumBattles++;
                if (gm.NumBattles >= gm.MaxFights)
                {
                    //gm.NumBattles = gm.MaxFights;
                    FullReset();
                    gm.LoadWin();
                }
                else
                {
                    FullReset();
                    gm.LoadShop();
                }
            }
        }
    }
    public void EndTurn()
    {
        endTurnSound.Play();
        if (playerTurn)  // due to damage delay, at the end of the player's turn they will take damage and vice versa
        {
            endTurnButton.interactable = false;
            if (reflectDamage==false)
            {
                player.TakeDamage(damageDelay);
                damageDelay = CalculateDamage(); //calculates damage for the enemy turn
            }
            else
            {
                reflectAmount = damageDelay;
                damageDelay = CalculateDamage();
                damageDelay += reflectAmount;
                reflectAmount = 0;
                reflectDamage = false;
            }

            if (enemy.poisoned)
            {
                damageDelay += enemy.poisonedAmt;
            }

            if (doubleDamage)
            {
                damageDelay = damageDelay * 2;
                doubleDamage = false;
            }
            outgoingDamage.text = "Incoming: " + damageDelay;
            incomingDamage.text = "";
            Debug.Log("Player End Turn");
            playerTurn = false;
            playerAutomaticActions = false;
        }
        else
        {
            StartCoroutine(WaitForEnemyCardAnimations(() =>
            {
                if(reflectDamage == false)
                {
                    enemy.TakeDamage(damageDelay);
                    damageDelay = CalculateDamage();
                }
                else
                {
                    reflectAmount = damageDelay;
                    damageDelay = CalculateDamage();
                    damageDelay += reflectAmount;
                    reflectAmount = 0;
                    reflectDamage = false;
                }
                
                if (player.poisoned)
                {
                    damageDelay += player.poisonedAmt;
                }

                if (doubleDamage)
                {
                    damageDelay = damageDelay * 2;
                    doubleDamage = false;
                }
                incomingDamage.text = "Incoming: " + damageDelay;
                outgoingDamage.text = "";
                Debug.Log("Enemy End Turn");
                StartCoroutine(DelayBeforePlayerTurn());
            }));
        }
    }

    //GameEndStuff
    public void FullReset()
    {
        player.hp = player.maxhp;
        enemy.hp = enemy.maxhp;
        player.actionPoints = 0;
        player.maxActionPoints = 5;
        enemy.enemyActionPoints = 0;
        enemy.maxEnemyActionPoints = 5;
        playerTurn = true;
        playerAutomaticActions = false;
        for (int i = 0; i < availableCardSlots.Length; i++)
        {
            availableCardSlots[i] = true;
            enemyAvailableCardSlots[i] = true;
        }
        DiscardPileToDeck(); //probably need to reset enemy deck too
    }

    //CARD STUFF
    public void DrawCards()
    {
        if (playerTurn == true)
        {
            if (player.deck.Count >= 1)
            {
                Card randCard = player.deck[Random.Range(0, player.deck.Count)];
                for (int i = 0; i < availableCardSlots.Length; i++)
                {
                    if (availableCardSlots[i] == true)
                    {
                        randCard.gameObject.SetActive(true);
                        randCard.handIndex = i;
                        randCard.transform.position = cardSlots[i].position;
                        availableCardSlots[i] = false;
                        player.hand.Add(randCard);
                        player.deck.Remove(randCard);

                        Draws[Random.Range(0, Draws.Count)].Play();
                        return;
                    }
                }
            }
        }
        else if (playerTurn == false)
        {
            if(gm.NumBattles == 1)
            {
                if (enemy.enemyDeck.Count >= 1)
                {
                    Card randCard = enemy.enemyDeck[Random.Range(0, enemy.enemyDeck.Count)];
                    for (int i = 0; i < enemyAvailableCardSlots.Length; i++)
                    {
                        if (enemyAvailableCardSlots[i] == true)
                        {
                            randCard.gameObject.SetActive(true);
                            randCard.handIndex = i;
                            randCard.transform.position = enemyCardSlots[i].position;
                            enemyAvailableCardSlots[i] = false;
                            enemy.enemyHand.Add(randCard);
                            enemy.enemyDeck.Remove(randCard);
                            //Debug.Log($"Enemy drew card: {randCard.name} to slot {i}");

                            Draws[Random.Range(0, Draws.Count)].Play();
                            return;
                        }
                    }
                }
            }
            else if(gm.NumBattles == 2)
            {
                if (enemy.enemyDeck2.Count >= 1)
                {
                    Card randCard = enemy.enemyDeck2[Random.Range(0, enemy.enemyDeck2.Count)];
                    for (int i = 0; i < enemyAvailableCardSlots.Length; i++)
                    {
                        if (enemyAvailableCardSlots[i] == true)
                        {
                            randCard.gameObject.SetActive(true);
                            randCard.handIndex = i;
                            randCard.transform.position = enemyCardSlots[i].position;
                            enemyAvailableCardSlots[i] = false;
                            enemy.enemyHand.Add(randCard);
                            enemy.enemyDeck2.Remove(randCard);
                            //Debug.Log($"Enemy drew card: {randCard.name} to slot {i}");

                            Draws[Random.Range(0, Draws.Count)].Play();
                            return;
                        }
                    }
                }
            }
        }
    }
    public void DiscardPileToDeck()
    {
        player.deck.AddRange(player.discardPile); //Discard Pile back to Deck
        player.deck.AddRange(player.hand); //Hand back to Deck
        player.discardPile.Clear();
        player.hand.Clear();
        if (gm.NumBattles == 1)
        {
            enemy.enemyDeck.AddRange(enemy.enemyDiscardPile);
            enemy.enemyDeck.AddRange(enemy.enemyHand);
            enemy.enemyDiscardPile.Clear();
            enemy.enemyHand.Clear();
        }
        else if (gm.NumBattles == 2)
        {
            enemy.enemyDeck2.AddRange(enemy.enemyDiscardPile);
            enemy.enemyDeck2.AddRange(enemy.enemyHand);
            enemy.enemyDiscardPile.Clear();
            enemy.enemyHand.Clear();
        }
        else
        {
            enemy.enemyDeck.AddRange(enemy.enemyDiscardPile);
            enemy.enemyDeck.AddRange(enemy.enemyHand);
            enemy.enemyDiscardPile.Clear();
            enemy.enemyHand.Clear();
        }
    }

    public void ResetCardSlots()
    {
        if (playerTurn)
        {
            for(int i=0; i<availableCardSlots.Length;i++)
            availableCardSlots[i] = true;
        }
        else if(!playerTurn)
        {
            for (int i = 0; i < enemyAvailableCardSlots.Length; i++)
                enemyAvailableCardSlots[i] = true;
        }
    }

    //ACTION POINT STUFF
    public void AddActionPoints()
    {
        if (playerTurn == true)
        {
            if (player.actionPoints <= player.maxActionPoints - 3)
            {
                player.actionPoints = player.actionPoints + 3;
            }
            else
            {
                player.actionPoints = player.maxActionPoints;
            }
        }
        if (playerTurn == false) //add AP for enemy turns
        {
            if (enemy.enemyActionPoints <= 2)
            {
                enemy.enemyActionPoints = enemy.enemyActionPoints + 3;
            }
            else
            {
                enemy.enemyActionPoints = enemy.maxEnemyActionPoints;
            }
        }
    }
    public void UpdateActionPoints()
    {
        //if (playerTurn == true || energyDrain == true)
        //{
        for (int i = 0; i < player.maxActionPoints; i++)
        {
            if (i < player.actionPoints)
            {
                if (!actionPointsPips[i].enabled)
                {
                    actionPointsPips[i].enabled = true;
                }
                actionPointsPips[i].sprite = APFull;
            }
            else
            {
                if (!actionPointsPips[i].enabled)
                {
                    actionPointsPips[i].enabled = true;
                }
                actionPointsPips[i].sprite = APEmpty;
            }
        }
        //}
        //if (playerTurn == false || energyDrain == true) //add AP for enemy turns
        //{
        for (int i = 0; i < enemy.maxEnemyActionPoints; i++)
        {
            if (i < enemy.enemyActionPoints)
            {
                if (!enemyActionPointsPips[i].enabled)
                {
                    enemyActionPointsPips[i].enabled = true;
                }
                enemyActionPointsPips[i].sprite = APFull;
            }
            else
            {
                if (!enemyActionPointsPips[i].enabled)
                {
                    enemyActionPointsPips[i].enabled = true;
                }
                enemyActionPointsPips[i].sprite = APEmpty;
            }
        }
        //}
    }
    //DAMAGE STUFF
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
            for (int k = 0; k < rolls.Length; k++)
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

    public void DebugCalculatedamage()
    {
        Debug.Log(CalculateDamage());
    }


    //Delay and Pause
    public void Resume()
    {
        clickSound.Play();
        isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(isPaused);
    }

    public void ToggleHelpMenu()
    {
        clickSound.Play();
        isHelp = !isHelp;
        helpMenu.SetActive(isHelp);
        pauseMenu.SetActive(!isHelp);
    }

    public void ResetRun()
    {
        clickSound.Play();
        FullReset();
        SceneManager.LoadScene("FightScene");
    }

    public void MainMenu()
    {
        clickSound.Play();
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        clickSound.Play();
        Application.Quit();
    }

    //Animation and Delay Stuff
    private IEnumerator WaitForEnemyCardAnimations(Action onComplete)
    {
        // Wait for all enemy card animations to complete
        EnemyCardAnimation[] animators = FindObjectsOfType<EnemyCardAnimation>();
        foreach (var animator in animators)
        {
            yield return new WaitUntil(() => animator.AnimationComplete);
        }
        onComplete?.Invoke();
    }

    private IEnumerator DelayBeforePlayerTurn()
    {
        yield return new WaitForSeconds(1.5f);
        playerTurn = true;
        enemyAutomaticActions = false;
        endTurnButton.interactable = true;
    }

    public void playPlayCardSound()
    {
        playCardSound.Play();
    }
    public void playBurnCardSound()
    {
        burnCardSound.Play();
    }
}

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Scene = UnityEngine.SceneManagement.Scene;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //Class Stuff
    public string cardName;
    public int cardID;
    public int apCost;
    public string effectText;
    public bool playerCard;
    private bool upgradable;

    //Tooltip Stuff
    public TextMeshProUGUI tooltipText;
    public GameObject tooltipPanel;
    private bool isTooltipActive;
    Vector3 offScreenPosition = new Vector3(-10000, -10000, 0);
    //Adjust these numbers if you need to change where the tooltip is
    private const float tooltipOffsetX = 0f; // Tooltip Offset in the X axis
    private const float tooltipOffsetY = 9.5f; // Tooltip Offset in the Y axis

    public bool hasBeenPlayed;
    public int handIndex;
    private FightManager fm;
    private Player player;
    private Enemy enemy;
    private DiceRoller diceRoller;
    private Dictionary<int, Func<bool>> cardDictionary;

    //Particle FX
    public ParticleSystem playEfx;
    public ParticleSystem burnEfx;

    public int shopCost;

    private Vector3 prevPos;

    void Awake()
    {
        FindRef();
    }

    void FindRef()
    {
        IntializeCardDictionary();
        fm = FindObjectOfType<FightManager>();
        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();
        diceRoller = FindObjectOfType<DiceRoller>();
        tooltipPanel = GameObject.Find("TooltipPanel");
        tooltipText = tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
    }
    void RemoveRef()
    {
        fm = null;
        player = null;
        enemy = null;
        diceRoller = null;
        tooltipPanel = null;
        tooltipText = null;
    }

    void OnEnable()
    {
        FindRef();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        // Clear old references and update new references
        if (scene.name == "FightScene")
        {
            RemoveRef();
            FindRef();
        }
        //Find ref again after scene change
    }
    // Start is called before the first frame update
    void Start()
    {
        HideTooltip();
    }
    void Update()
    {
        if (isTooltipActive)
        {
            UpdateTooltipPosition();
        }
    }

    //ALL PLAYING/BURNING/CARD CONTROLLERS//
    void IntializeCardDictionary()
    {
        cardDictionary = new Dictionary<int, Func<bool>>()
        {
            { 0, () => NoEffect()},
            { 1, () => Heal(10)}, //Recover
            { 2, () => Reroll(5)}, //Loaded Dice
            { 3, () => GrantFullHouse()}, //Steady Aim
            { 4, () => ActivateShield(20, 1)}, //Fortify
            { 5, () => ChangeMaxAP(1) }, //Booster Energy
            { 6, () => DoubleDamage() },  //Deadeye
            { 7, () => ChooseReroll(3,false) },  //Weighted Dice
            { 8, () => ChooseReroll(3,true) },  // 3 of diamonds
            { 9, () => EnergyDrain(1) }, //EnergyDrain
            { 10,() => Poison() }, //Poison
            { 11,() => ReflectDamage() }, //Reflect
            { 12,() => Bind() }, //Bind
            { 13,() => Redraw() } //Redraw
        };
    }
    public bool ApplyEffect()
    {
        if (cardDictionary.TryGetValue(cardID, out Func<bool> effect))
        {
            return effect.Invoke();
        }
        else
        {
            Debug.LogWarning($"No effect found for cardID: {cardID}");
            return false;
        }
    }
    void PlayCard(int actionPointCost)
    {
        if (actionPointCost <= player.actionPoints)
        {
            fm.playPlayCardSound();
            if (ApplyEffect() == true)
            {
                Vector3 cardPosition = transform.position;
                player.hand.Remove(this);
                player.discardPile.Add(this);
                gameObject.SetActive(false);
                Instantiate(playEfx, cardPosition, Quaternion.identity);

                HideTooltip();
                for (int i = 0; i < actionPointCost; i++)
                {
                    player.actionPoints--;
                    fm.UpdateActionPoints();
                }
                hasBeenPlayed = true;
                fm.availableCardSlots[handIndex] = true;
                Debug.Log("Card played");
            }
            else
            {
                Debug.Log("Card conditions not met and cannot be played!");
                this.transform.position = prevPos;
                ShowTooltip(effectText);
            }
        }
        else
        {
            Debug.Log("Card cannot be played due to insufficient AP!");
            this.transform.position = prevPos;
            ShowTooltip(effectText);
        }
    }
    void BurnCard()
    {
        if (player.actionPoints < player.maxActionPoints)
        {
            fm.playBurnCardSound();
            player.actionPoints = player.actionPoints + 1;
            fm.UpdateActionPoints();
            Debug.Log("Card burned");
        }
        else
        {
            Debug.Log("AP already at maximum value. Discarding Card.");
        }
        Vector3 cardPosition = transform.position;
        player.hand.Remove(this);
        player.discardPile.Add(this);
        gameObject.SetActive(false);
        Instantiate(burnEfx, cardPosition, Quaternion.identity);
        HideTooltip();
        hasBeenPlayed = true;
        fm.availableCardSlots[handIndex] = true;
    }

    void UpgradeCard(bool canUpgrade, int cardID)
    {
        if (canUpgrade)
        {
            //do stuff
            player.deck.Remove(this);
            gameObject.SetActive(false);
        }
    }

    //ALL CARD EFFECTS FUNCTIONS//
    bool Heal(int amount) //needs to check if you are allowed to play the card
    {
        if (fm.playerTurn == true)
        {
            if(player.hp == player.maxhp && player.poisoned==true)
            {
                player.poisoned = false;
                player.poisonedAmt = 0;
                return true;
            }
            else if (player.hp == player.maxhp)
            {
                return false;
            }
            else if (player.hp + amount >= player.maxhp)
            {
                player.hp = player.maxhp;
                player.healthBars.updatePlayerBar();
                player.poisoned = false;
                player.poisonedAmt = 0;
                return true;
            }
            else
            {
                player.hp = player.hp + amount;
                player.healthBars.updatePlayerBar();
                player.poisoned = false;
                player.poisonedAmt = 0;
                return true;
            }
        }
        else if (fm.playerTurn == false)
        {
            if (enemy.hp == enemy.maxhp && enemy.poisoned==true)
            {
                enemy.poisoned = false;
                enemy.poisonedAmt = 0;
                return true;
            }
            else if (enemy.hp == enemy.maxhp)
            {
                return false;
            }
            else if (enemy.hp + amount >= enemy.maxhp)
            {
                enemy.poisoned = false;
                enemy.poisonedAmt = 0;
                enemy.hp = enemy.maxhp;
                enemy.healthBars.updateEnemyBar();
                return true;
            }
            else
            {
                enemy.poisoned = false;
                enemy.poisonedAmt = 0;
                enemy.hp = enemy.hp + amount;
                enemy.healthBars.updateEnemyBar();
                return true;
            }
        }
        else { return false; }
    }

    // rerolls all dice
    bool Reroll(int numberOfDice)
    {
        diceRoller.allowRerolls(numberOfDice, false);
        //probably doesn't need to check for player or enemy turn?
        for (int i = 0; i < numberOfDice; i++)
        {
            diceRoller.ReRoll(i);
        }
        return true; //always returns true b/c I see no reason why it cant always reroll? AP is checked elsewhere
    }

    bool GrantFullHouse()
    {
        diceRoller.SetDiceValue(0, 3);
        diceRoller.SetDiceValue(1, 3);
        diceRoller.SetDiceValue(2, 3);
        diceRoller.SetDiceValue(3, 2);
        diceRoller.SetDiceValue(4, 2);
        return true;
    }
    bool ChangeDiceFace(int numberOfDice)
    {
        for (int i = 0; i < numberOfDice; i++)
        {
            diceRoller.SetDiceValue(i, 3);
        }
        return true;
    }
    bool ActivateShield(int amt, int numTurns)
    {
        fm.ActivateShield(amt, numTurns);
        return true;
    }

    bool ChangeMaxAP(int amount)
    {
        if (fm.playerTurn == true)
        {
            if (amount + player.maxActionPoints <= 10)
            {
                player.maxActionPoints++;
                fm.UpdateActionPoints();
                Debug.Log("Action Point Slot Added");
                return true;
            }
            else
            {
                Debug.Log("Cannot add any more AP! Max AP is 10!");
                return false;
            }
        }
        else
        {
            if (amount + enemy.maxEnemyActionPoints <= 10)
            {
                enemy.maxEnemyActionPoints++;
                fm.UpdateActionPoints();
                Debug.Log("Enemy Action Point Slot Added");
                return true;
            }
            else
            {
                Debug.Log("Cannot add any more AP! Max AP is 10!");
                return false;
            }
        }
    }

    bool DoubleDamage()
    {
        {
            if (fm.CalculateDamage() > 0)
            {
                fm.doubleDamage = true;
                return true;
            }
            else
            {
                Debug.Log("No outgoing damage! Card cannot be played!");
                return false;
            }
        }

    }

    bool Poison()
    {
        if(fm.playerTurn == true)
        {
            enemy.poisoned = true;
            enemy.poisonedAmt += 3;
            //Make it activate an icon near HP bar
        }
        else if(fm.playerTurn == false)
        {
            player.poisoned = true;
            player.poisonedAmt += 3;
            //Make it activate an icon near HP bar
        }
        return true;
    }

    bool ReflectDamage()
    {
        if (fm.damageDelay == 0)
        {
            Debug.Log("No damage to reflect! Cannot be played!");
            return false;
        }
        else
        {
            fm.reflectDamage = true;
            return true;
        }
    }

    bool Bind()
    {
        if (fm.playerTurn && enemy.bound == false)
        {
            enemy.bound = true;
            return true;
        }
        else if (fm.playerTurn && enemy.bound == true)
        {
            Debug.Log("Enemy is already bound!");
            return false;
        }
        else if (fm.playerTurn == false && player.bound == false)
        {
            player.bound = true;
            return true;
        }
        else if(fm.playerTurn==false && player.bound == true)
        {
            Debug.Log("Player is already bound!");
            return false;
        }
        else
        {
            Debug.LogWarning("Error in Bind!");
            return false;
        }
    }

    bool Redraw()
    {
        int count = 0;
        if (fm.playerTurn)
        {
            if (player.hand.Count == 1)
            {
                Debug.Log("Cannot play! Only one card in hand");
                return false;
            }
            else
            {
                for (int i = player.hand.Count - 1; i >= 0; i--)
                {
                    Card card = player.hand[i];
                    if (card != this)
                    {
                        player.hand.RemoveAt(i); // Remove the card by index
                        player.discardPile.Add(card);
                        card.gameObject.SetActive(false);
                        count++;
                    }
                }
                fm.ResetCardSlots();
                for (int i = 0; i < count; i++)
                {
                    fm.DrawCards();
                }
                count = 0;
                return true;
            }
        }
        else if (!fm.playerTurn)
        {
            if (enemy.enemyHand.Count == 1)
            {
                Debug.Log("Cannot play! Only one card in hand");
                return false;
            }
            else
            {
                for (int i = enemy.enemyHand.Count - 1; i >= 0; i--)
                {
                    Card card = enemy.enemyHand[i];
                    if (card != this)
                    {
                        enemy.enemyHand.RemoveAt(i); // Remove the card by index
                        enemy.enemyDiscardPile.Add(card);
                        card.gameObject.SetActive(false);
                        count++;
                    }
                }
                fm.ResetCardSlots();
                for (int i = 0; i < count; i++)
                {
                    fm.DrawCards();
                }
                count = 0;
                return true;
            }
        }
        Debug.LogWarning("Error in Redraw");
        return false;
    }


    bool ChooseReroll(int numDie, bool allowSameRerolls)
    {
        diceRoller.allowRerolls(numDie, allowSameRerolls);
        return true;
    }

    bool EnergyDrain(int num)
    {
        if (fm.playerTurn)
        {
            if (enemy.enemyActionPoints > 0)
            {
                enemy.enemyActionPoints = enemy.enemyActionPoints - num;
                fm.UpdateActionPoints();
                return true;
            }
            else
            {
                Debug.Log("Cannot Drain AP. Enemy has no AP!");
                return false;
            }
        }
        else
        {
            if (player.actionPoints > 0)
            {
                player.actionPoints = player.actionPoints - num;
                fm.UpdateActionPoints();
                return true;
            }
            else
            {
                Debug.Log("Cannot Drain AP. Player has no AP!");
                return false;
            }
        }
    }

    bool NoEffect()
    {
        return true;
    }

    //ALL DRAG/UI FUNCTIONS//
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (playerCard)
        {
            HideTooltip();
            prevPos = this.transform.position;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (playerCard)
        {
            Vector3 worldPoint;
            RectTransform canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out worldPoint))
            {
                transform.position = worldPoint;
            }
            HideTooltip();
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (playerCard)
        {
            if (IsPointerOverUIObject(eventData, fm.playRectTransform))
            {
                PlayCard(apCost);
            }
            else if (IsPointerOverUIObject(eventData, fm.burnRectTransform))
            {
                BurnCard();
            }
            /*else if(IsPointerOverUIObject(eventData, fm.upgradeRectTransform))
            {
                UpgradeCard(upgradable, cardID);
            }*/
            else
            {
                //Debug.Log("No Transform Found!");
                this.transform.position = prevPos;
                ShowTooltip(effectText);
            }
        }
    }
    private bool IsPointerOverUIObject(PointerEventData eventData, RectTransform target)
    {
        if (target == null)
        {
            return false;
        }

        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            target,
            eventData.position,
            eventData.pressEventCamera,
            out localMousePosition);

        return target.rect.Contains(localMousePosition);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse entered card area");
        ShowTooltip(effectText);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Mouse exited card area");
        HideTooltip();
    }

    //ALL TOOLTIP FUNCTIONS//
    void ShowTooltip(string message)
    {
        if(tooltipPanel == null || tooltipText ==null)
        {
            tooltipPanel = GameObject.Find("TooltipPanel");
            tooltipText = tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (playerCard)
        {
            //Debug.Log($"Showing tooltip with message: {message}");
            tooltipText.text = message;
            tooltipPanel.SetActive(true);
            isTooltipActive = true;
            UpdateTooltipPosition();
        }
    }

    void HideTooltip()
    {
        if (tooltipPanel == null || tooltipText == null)
        {
            tooltipPanel = GameObject.Find("TooltipPanel");
            tooltipText = tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (playerCard)
        {
            //tooltipPanel.SetActive(false);
            tooltipPanel.transform.position = offScreenPosition;
            isTooltipActive = false;
        }
    }
    void UpdateTooltipPosition()
    {
        if (tooltipPanel == null || tooltipText == null)
        {
            tooltipPanel = GameObject.Find("TooltipPanel");
            tooltipText = tooltipPanel.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (playerCard)
        {
            // Calculate the position of the tooltip in world space relative to the card's position
            Vector3 newPos = transform.position + new Vector3(tooltipOffsetX, tooltipOffsetY, 0f);

            // Update the tooltip position directly in world space
            tooltipPanel.transform.position = newPos;
        }
    }
}

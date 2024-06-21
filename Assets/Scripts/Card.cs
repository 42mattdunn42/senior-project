using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    //Class Stuff
    public string cardName;
    public int cardID;
    public int apCost;
    public string effectText;
    public bool playerCard;

    //Tooltip Stuff
    public TextMeshProUGUI tooltipText;
    public GameObject tooltipPanel;
    private bool isTooltipActive;
    //Adjust these numbers if you need to change where the tooltip is
    private const float tooltipOffsetX = 0f; // Tooltip Offset in the X axis
    private const float tooltipOffsetY = 7.5f; // Tooltip Offset in the Y axis

    public bool hasBeenPlayed;
    public int handIndex;
    private FightManager fm;
    private Player player;
    private Enemy enemy;
    private DiceRoller diceRoller;
    private RectTransform playRectTransform;
    private RectTransform burnRectTransform;
    private Dictionary<int, Func<bool>> cardDictionary;

    //Particle FX
    public ParticleSystem playEfx;


    void Awake()
    {
        IntializeCardDictionary();
    }
    // Start is called before the first frame update
    void Start()
    {
        fm = FindObjectOfType<FightManager>();
        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();
        diceRoller = FindObjectOfType<DiceRoller>();
        playRectTransform = GameObject.Find("Play Area").GetComponent<RectTransform>();
        burnRectTransform = GameObject.Find("Burn Card Area").GetComponent<RectTransform>();
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
            { 1, () => Heal(10)}, //Jack of Clubs
            { 2, () => Reroll(5)}, //King of Diamonds
            { 3, () => ChangeDiceFace(3)}, //Queen of Hearts
            { 4, () => ActivateShield(20, 1)} // Ace of Hearts
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
        if(actionPointCost <= fm.actionPoints)
        {
            if (ApplyEffect()==true)
            {
                Vector3 cardPosition = transform.position;
                fm.discardPile.Add(this);
                gameObject.SetActive(false);
                Instantiate(playEfx, cardPosition, Quaternion.identity);

                HideTooltip();
                for (int i = 0; i < actionPointCost; i++)
                {
                    fm.actionPoints--;
                    fm.UpdateActionPoints();
                }
                hasBeenPlayed = true;
                fm.availableCardSlots[handIndex] = true;
                Debug.Log("Card played");
            }
            else
            {
                Debug.Log("Card conditions not met and cannot be played!");
                ShowTooltip(effectText);
            }
        }
        else
        {
            Debug.Log("Card cannot be played due to insufficient AP!");
            ShowTooltip(effectText);
        }
    }
    void BurnCard()
    {
        if (fm.actionPoints <= 4)
        {
            fm.actionPoints = fm.actionPoints + 1;
            fm.UpdateActionPoints();
            Debug.Log("Card burned");
        }
        else
        {
            Debug.Log("AP already at maximum value. Discarding Card.");
        }
        fm.discardPile.Add(this);
        gameObject.SetActive(false);
        HideTooltip();
        hasBeenPlayed = true;
        fm.availableCardSlots[handIndex] = true;
    }

    //ALL CARD EFFECTS FUNCTIONS//
    bool Heal(int amount) //needs to check if you are allowed to play the card
    {
        if(fm.playerTurn == true)
        {
            if (player.hp == player.maxhp)
            {
                return false;
            }
            else if(player.hp + amount >= player.maxhp)
            {
                player.hp = player.maxhp;
                player.healthBars.updatePlayerBar();
                return true;
            }
            else
            {
                player.hp = player.hp + amount;
                player.healthBars.updatePlayerBar();
                return true;
            }
        }
        else if(fm.playerTurn == false)
        {
            if (enemy.hp == enemy.maxhp)
            {
                return false;
            }
            else if (enemy.hp + amount >= enemy.maxhp)
            {
                enemy.hp = enemy.maxhp;
                return true;
            }
            else
            {
                enemy.hp = enemy.hp + amount;
                enemy.healthBars.updateEnemyBar();
                return true;
            }
        }
        else { return false; }
    }
    bool Reroll (int numberOfDice)
    {
        //probably doesn't need to check for player or enemy turn?
        for(int i = 0; i < numberOfDice; i++)
        {
            diceRoller.ReRoll(i);
        }
        return true; //always returns true b/c I see no reason why it cant always reroll? AP is checked elsewhere
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
            if (IsPointerOverUIObject(eventData, playRectTransform))
            {
                PlayCard(apCost);
            }
            else if (IsPointerOverUIObject(eventData, burnRectTransform))
            {
                BurnCard();
            }
            else
            {
                ShowTooltip(effectText);
            }
        }   
    }
    private bool IsPointerOverUIObject(PointerEventData eventData, RectTransform target)
    {
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
        //Debug.Log("Hiding tooltip");
        if (playerCard)
        {
            tooltipPanel.SetActive(false);
            isTooltipActive = false;
        }
    }
    void UpdateTooltipPosition()
    {
        if (playerCard)
        {
            // Calculate the position of the tooltip in world space relative to the card's position
            Vector3 newPos = transform.position + new Vector3(tooltipOffsetX, tooltipOffsetY, 0f);

            // Update the tooltip position directly in world space
            tooltipPanel.transform.position = newPos;

            /* Vector2 mousePos = Input.mousePosition;
            float tooltipWidth = tooltipPanel.GetComponent<RectTransform>().rect.width;
            float tooltipHeight = tooltipPanel.GetComponent<RectTransform>().rect.height;

            // Calculate new position with offset
            Vector2 newPos = new Vector2(mousePos.x + tooltipOffsetX, mousePos.y + tooltipOffsetY);

            // Adjust position to stay within screen bounds
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float maxX = screenWidth - tooltipWidth;
            float maxY = screenHeight - tooltipHeight;

            newPos.x = Mathf.Clamp(newPos.x, 0f, maxX);
            newPos.y = Mathf.Clamp(newPos.y, 0f, maxY);

            tooltipPanel.transform.position = newPos;*/
        }
    }
}

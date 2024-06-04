using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //Class Stuff
    public string cardName;
    public int cardID;
    public int apCost;
    public string effectText;


    public bool hasBeenPlayed;
    public int handIndex;
    private FightManager fm;
    private Player player;
    private Enemy enemy;
    private DiceRoller diceRoller;
    private RectTransform playRectTransform;
    private RectTransform burnRectTransform;
    private Dictionary<int, Func<bool>> cardDictionary;
    // Start is called before the first frame update
    void Start()
    {
        fm = FindObjectOfType<FightManager>();
        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();
        diceRoller = FindObjectOfType<DiceRoller>();
        playRectTransform = GameObject.Find("Play Area").GetComponent<RectTransform>();
        burnRectTransform = GameObject.Find("Burn Card Area").GetComponent<RectTransform>();

        IntializeCardDictionary();
    }

    void IntializeCardDictionary()
    {
        cardDictionary = new Dictionary<int, Func<bool>>()
        {
            { 1, () => Heal(10)}, //Jack of Clubs
            { 2, () => Reroll(5)} //King of Diamonds
        };
    }
    bool ApplyEffect()
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

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsPointerOverUIObject(eventData, playRectTransform))
        {
            
            PlayCard(apCost);
        }

        if (IsPointerOverUIObject(eventData, burnRectTransform))
        {
            Debug.Log("Card burned");
            BurnCard();
            hasBeenPlayed = true;
            fm.availableCardSlots[handIndex] = true;
        }
    }

    private bool IsPointerOverUIObject(PointerEventData eventData, RectTransform target)
    {
        Vector2 localMousePosition = target.InverseTransformPoint(eventData.position);
        return target.rect.Contains(localMousePosition);
    }

    void PlayCard(int actionPointCost)
    {
        if(actionPointCost <= fm.actionPoints)
        {
            if (ApplyEffect()==true)
            {
                fm.discardPile.Add(this);
                gameObject.SetActive(false);
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
            }
        }
        else
        {
            Debug.Log("Card cannot be played due to insufficient AP!");
        }
    }

    void BurnCard()
    {
        fm.discardPile.Add(this);
        gameObject.SetActive(false);
        if (fm.actionPoints <= 4)
        {
            fm.actionPoints = fm.actionPoints + 1;
        }
        else
        {
            fm.actionPoints =  fm.maxActionPoints;
        }
        fm.UpdateActionPoints();
    }

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
}

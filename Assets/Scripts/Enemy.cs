using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;
    public int maxhp;
    public HealthBarManager healthBars;
    public int shield;
    public int shieldLength;
    public FightManager fm;
    public ParticleSystem damageEfx;
    public Transform damageEfxSpawnPos;

    //enemy deck variables
    public List<Card> enemyDeck = new List<Card>();
    public List<Card> enemyHand = new List<Card>();
    public List<Card> enemyDiscardPile = new List<Card>();

    //enemy action point variables
    public int enemyActionPoints;
    public int enemyNumOfActionPoints;
    public int maxEnemyActionPoints = 5;

    //Card Dictionary
    private Dictionary<int, int> enemyCardCount;

    public Enemy(int hp) { 
        this.hp = hp; 
        maxhp = hp; 
        if (healthBars == null)
        {
            healthBars = GameObject.FindGameObjectWithTag("HealthManager").GetComponent<HealthBarManager>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        healthBars = GameObject.FindGameObjectWithTag("HealthManager").GetComponent<HealthBarManager>();
        fm = GameObject.FindGameObjectWithTag("FightManager").GetComponent<FightManager>();
        enemyCardCount = new Dictionary<int, int>();
    }

    public bool IsAlive() { 
        return hp > 0; 
    }
    public void TakeDamage(int damage)
    {
        if (shieldLength > 0)
        {
            if (damage > shield)
            {
                hp -= (damage - shield);
                fm.DeactivateShield(false);
            }
            else
            {
                shield -= damage;
                fm.UpdateShield(false, shield);
            }
            shieldLength--;
            if (shieldLength < 1)
            {
                fm.DeactivateShield(false);
            }
        }
        else
        {
            hp -= damage;
        }
        healthBars.updateEnemyBar();
        if (damage != 0)
        {
            Instantiate(damageEfx, damageEfxSpawnPos.position, transform.rotation);
        }
    }

    //Enemy Play Card Functions
    void EnemyCheckHand()
    {
        enemyCardCount.Clear();
        foreach (Card card in enemyHand)
        {
            if (enemyCardCount.ContainsKey(card.cardID))
            {
                enemyCardCount[card.cardID]++;
            }
            else
            {
                enemyCardCount[card.cardID] = 1;
            }
        }

        /** Print out the contents of the dictionary for debugging
        foreach (KeyValuePair<int, int> entry in enemyCardCount)
        {
            Debug.Log($"CardID: {entry.Key}, Count: {entry.Value}");
        }**/
    }

    public void EnemyPlayLogic()
    {
        EnemyCheckHand();

        if (enemyCardCount.ContainsKey(2) && enemyCardCount[2] > 0 && fm.CalculateDamage() == 0) //Will attempt to get damage using cheaper card first
        {
            foreach (Card card in enemyHand)
            {
                if (card.cardID == 2)
                {
                    if (card.apCost <= enemyActionPoints)
                    {
                        EnemyPlayCard(card, card.apCost);
                        Debug.Log("Enemy playing King of Diamonds");
                        break;
                    }
                }
            }
        }

        if (enemyCardCount.ContainsKey(3) && enemyCardCount[3] > 0 && fm.CalculateDamage() == 0) //Will guareentee damage if it cannot get a good roll with a King of Diamonds
        {
            foreach (Card card in enemyHand)
            {
                if (card.cardID == 3)
                {
                    if (card.apCost <= enemyActionPoints)
                    {
                        EnemyPlayCard(card, card.apCost);
                        Debug.Log("Enemy playing Queen of Hearts");
                        break;
                    }
                }
            }
        }

        if (enemyCardCount.ContainsKey(4) && enemyCardCount[4] > 0 && fm.damageDelay > 0) //Will shield itself first if it is going to take damage
        {
            foreach (Card card in enemyHand)
            {
                if (card.cardID == 4)
                {
                    if (card.apCost <= enemyActionPoints)
                    {
                        EnemyPlayCard(card, card.apCost);
                        Debug.Log("Enemy playing Ace of Hearts");
                        break;
                    }
                }
            }
        }

        if (enemyCardCount.ContainsKey(1) && enemyCardCount[1] > 0 && hp < 100) //Will heal itself if it has taken damage
        {
            foreach (Card card in enemyHand)
            {
                if (card.cardID == 1)
                {
                    if (card.apCost <= enemyActionPoints)
                    {
                        EnemyPlayCard(card, card.apCost);
                        Debug.Log("Enemy playing Jack of Clubs");
                        break;
                    }
                }
            }
        }
    }

    void EnemyPlayCard(Card playedCard, int actionPointCost)
    {
        if (actionPointCost <= enemyActionPoints)
        {
            if (playedCard.ApplyEffect())
            {
                enemyDiscardPile.Add(playedCard); // Add the card to the discard pile
                enemyHand.Remove(playedCard); //Remove card from hand
                playedCard.gameObject.SetActive(false); // Deactivate the GameObject
                for (int i = 0; i < actionPointCost; i++)
                {
                    enemyActionPoints--;
                    fm.UpdateActionPoints();
                }
                playedCard.hasBeenPlayed = true;
                fm.enemyAvailableCardSlots[playedCard.handIndex] = true;
                //Debug.Log($"Slot {playedCard.handIndex} re-enabled for use.");
                Debug.Log("Enemy card played");
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
    void EnemyPlayRandom()
    {
        Card randCard = enemyHand[Random.Range(0, enemyHand.Count)];
        //Debug.Log($"Enemy plays card: {randCard.name} from slot {randCard.handIndex}");
        EnemyPlayCard(randCard, randCard.apCost);
    }
}

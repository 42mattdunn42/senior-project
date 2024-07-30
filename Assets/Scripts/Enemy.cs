using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;
    public int maxhp;
    public HealthBarManager healthBars;
    public int shield;
    public int shieldLength;
    public FightManager fm;
    private Player player;
    public ParticleSystem damageEfx;
    public Transform damageEfxSpawnPos;

    //Status Effects
    public bool poisoned = false;
    public int poisonedAmt = 0;
    public bool bound = false;

    //enemy deck variables
    public List<Card> enemyDeck = new List<Card>();
    public List<Card> enemyDeck2 = new List<Card>();
    public List<Card> enemyDeck3 = new List<Card>();
    public List<Card> enemyHand = new List<Card>();
    public List<Card> enemyDiscardPile = new List<Card>();

    //enemy action point variables
    public int enemyActionPoints;
    public int enemyNumOfActionPoints;
    public int maxEnemyActionPoints = 5;

    //Card Dictionary
    private Dictionary<int, int> enemyCardCount;

    public Enemy(int hp)
    {
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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        enemyCardCount = new Dictionary<int, int>();
    }

    public bool IsAlive()
    {
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
        StartCoroutine(EnemyPlayLogicCoroutine());
    }

    public void EnemyPlayLogic2()
    {
        StartCoroutine(EnemyPlayLogicCoroutine2());
    }

    public void EnemyPlayLogic3()
    {
        StartCoroutine(EnemyPlayLogicCoroutine3());
    }

    private IEnumerator EnemyPlayLogicCoroutine()
    {
        EnemyCheckHand();

        // Try to play Loaded Dice (cardID 2)
        if (enemyCardCount.ContainsKey(2) && enemyCardCount[2] > 0 && fm.CalculateDamage() == 0)
        {
            yield return PlayCardIfPossible(2, "Loaded Dice");
        }

        // Try to play Steady Aim (cardID 3)
        if (enemyCardCount.ContainsKey(3) && enemyCardCount[3] > 0 && fm.CalculateDamage() == 0)
        {
            yield return PlayCardIfPossible(3, "Steady Aim");
        }

        // Try to play Fortify (cardID 4)
        if (enemyCardCount.ContainsKey(4) && enemyCardCount[4] > 0 && fm.damageDelay > 0)
        {
            yield return PlayCardIfPossible(4, "Fortify");
        }

        // Try to play Recover (cardID 1)
        if (enemyCardCount.ContainsKey(1) && enemyCardCount[1] > 0 && hp < 100)
        {
            yield return PlayCardIfPossible(1, "Recover");
        }
    }

    private IEnumerator EnemyPlayLogicCoroutine2()
    {
        EnemyCheckHand();

        // Try to play Booster Energy (cardID 5)
        if (enemyCardCount.ContainsKey(5) && enemyCardCount[5] > 0)
        {
            yield return PlayCardIfPossible(5, "Booster Energy");
        }

        // Try to play Energy Drain (cardID 9)
        if (enemyCardCount.ContainsKey(9) && enemyCardCount[9] > 0 && player.actionPoints>0)
        {
            yield return PlayCardIfPossible(9, "Energy Drain");
        }

        // Try to play Fortify (cardID 4)
        if ((enemyCardCount.ContainsKey(4) && enemyCardCount[4] > 0 && fm.damageDelay > 15) || (fm.damageDelay == 0 && enemyHand.Count > 4))
        {
            yield return PlayCardIfPossible(4, "Fortify");
        }

        // Try to play Steady Aim (cardID 3)
        if (enemyCardCount.ContainsKey(3) && enemyCardCount[3] > 0 && fm.CalculateDamage() == 0)
        {
            yield return PlayCardIfPossible(3, "Steady Aim");
        }

        // Try to play Deadeye (cardID 6)
        if (enemyCardCount.ContainsKey(6) && enemyCardCount[6] > 0 && fm.CalculateDamage() > 0)
        {
            yield return PlayCardIfPossible(6, "Deadeye");
        }

        // Try to play Recover (cardID 1)
        if (enemyCardCount.ContainsKey(1) && enemyCardCount[1] > 0 && hp < 100)
        {
            yield return PlayCardIfPossible(1, "Recover");
        }
    }

    private IEnumerator EnemyPlayLogicCoroutine3()
    {
        EnemyCheckHand();
        // Try to play Reflect (cardID 11)
        if (enemyCardCount.ContainsKey(11) && enemyCardCount[11] > 0 && fm.damageDelay >= 15)
        {
            yield return PlayCardIfPossible(11, "Reflect");
        }
        // Try to play Poison (cardID 10)
        if (enemyCardCount.ContainsKey(10) && enemyCardCount[10] > 0)
        {
            yield return PlayCardIfPossible(10, "Poison");
        }
        // Try to play Redraw (cardID 13)
        if (enemyCardCount.ContainsKey(13) && enemyCardCount[13] > 0)
        {
            yield return PlayCardIfPossible(13, "Redraw");
        }
        // Try to play Deadeye (cardID 6)
        if (enemyCardCount.ContainsKey(6) && enemyCardCount[6] > 0 && fm.CalculateDamage() >= 15)
        {
            yield return PlayCardIfPossible(6, "Deadeye");
        }
        // Try to play Steady Aim (cardID 3)
        if (enemyCardCount.ContainsKey(3) && enemyCardCount[3] > 0 && fm.CalculateDamage() == 0)
        {
            yield return PlayCardIfPossible(3, "Steady Aim");
        }
        // Try to play Fortify (cardID 4)
        if ((enemyCardCount.ContainsKey(4) && enemyCardCount[4] > 0 && fm.damageDelay > 15) || (fm.damageDelay == 0 && enemyHand.Count > 4))
        {
            yield return PlayCardIfPossible(4, "Fortify");
        }
        // Try to play Recover (cardID 1)
        if (enemyCardCount.ContainsKey(1) && enemyCardCount[1] > 0 && hp < 100 || poisoned == true)
        {
            yield return PlayCardIfPossible(1, "Recover");
        }
        
    }

    private IEnumerator PlayCardIfPossible(int cardID, string cardName)
    {
        foreach (Card card in enemyHand)
        {
            if (card.cardID == cardID)
            {
                if (card.apCost <= enemyActionPoints)
                {
                    EnemyPlayCard(card, card.apCost);
                    Debug.Log($"Enemy playing {cardName}");
                    yield return new WaitForSeconds(0.75f); // Adjust the delay as needed
                    break;
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
                playedCard.ShowCardFront();
                // Deduct action points immediately
                for (int i = 0; i < actionPointCost; i++)
                {
                    enemyActionPoints--;
                    fm.UpdateActionPoints();
                }
                // Start the animation
                var animator = playedCard.gameObject.AddComponent<EnemyCardAnimation>();
                animator.onAnimationComplete += () =>
                {
                    // Move card to discard pile and other post-play logic
                    enemyDiscardPile.Add(playedCard);
                    enemyHand.Remove(playedCard);
                    playedCard.hasBeenPlayed = true;
                    playedCard.gameObject.SetActive(false);
                    fm.enemyAvailableCardSlots[playedCard.handIndex] = true;
                    //Debug.Log("Enemy card played");
                };
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

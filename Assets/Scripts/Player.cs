using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int hp;
    public int maxhp;
    public HealthBarManager healthBars;
    public int shield;
    public int shieldLength;
    public FightManager fm;
    public ParticleSystem damageEfx;
    public Transform damageEfxSpawnPos;
    public int funds;

    //player deck variables
    public List<Card> deck = new List<Card>();
    public List<Card> discardPile = new List<Card>();

    //action point variables
    public int actionPoints;
    public int numOfActionPoints;
    public int maxActionPoints = 5;

    // Start is called before the first frame update
    void Start()
    {
        maxhp = hp;
        healthBars = GameObject.FindGameObjectWithTag("HealthManager").GetComponent<HealthBarManager>();
        fm = GameObject.FindGameObjectWithTag("FightManager").GetComponent<FightManager>();
    }

    public bool IsAlive()
    {
        return hp > 0;
    }

    public void TakeDamage(int damage)
    {
        if (shieldLength > 0 && damage != 0)
        {
            if(damage > shield)
            {
                hp -= (damage - shield);
                fm.DeactivateShield(true);
            }
            else
            {
                shield -= damage;
                fm.UpdateShield(true, shield);
            }
            shieldLength--;
            if(shieldLength < 1)
            {
                fm.DeactivateShield(true);
            }
        }
        else
        {
            hp -= damage;
        }
        healthBars.updatePlayerBar();
        if(damage != 0)
        {
            Instantiate(damageEfx, damageEfxSpawnPos.position, transform.rotation);
        }
    }
}

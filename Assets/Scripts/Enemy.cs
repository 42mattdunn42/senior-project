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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;
    public int maxhp;
    private HealthBarManager healthBars;

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
    }

    public bool IsAlive() { 
        return hp > 0; 
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
        healthBars.updateEnemyBar();
    }
}

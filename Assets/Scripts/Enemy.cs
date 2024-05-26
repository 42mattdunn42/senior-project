using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int hp;

    public Enemy(int hp) { this.hp = hp; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool IsAlive() { 
        return hp > 0; 
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
    }
}

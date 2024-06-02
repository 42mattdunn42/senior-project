using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int hp;
    public int maxhp;
    private HealthBarManager healthBars;

    // Start is called before the first frame update
    void Start()
    {
        maxhp = hp;
        healthBars = GameObject.FindGameObjectWithTag("HealthManager").GetComponent<HealthBarManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsAlive()
    {
        return hp > 0;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        healthBars.updatePlayerBar();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public Player player;
    public Enemy enemy;
    public bool isPlayerBar;
    private void Start()
    {
        healthBar = GetComponent<Slider>();
        if (isPlayerBar) {
            healthBar.maxValue = player.maxhp;
            healthBar.value = player.maxhp;
            Debug.Log(healthBar.maxValue);
        }
        else
        {
            healthBar.maxValue = enemy.maxhp;
            healthBar.value = enemy.maxhp;
        }
        
    }
    public void UpdateBar()
    {
        if (isPlayerBar)
        {
            healthBar.value = player.hp;
        }
        else
        {
            healthBar.value = enemy.hp;
        }
    }
}
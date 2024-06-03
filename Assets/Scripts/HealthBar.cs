using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI text;
    public Player player;
    public Enemy enemy;
    public bool isPlayerBar;
    private void Start()
    {
        healthBar = GetComponent<Slider>();
        //text =  GetComponent<TextMeshProUGUI>();
        if (isPlayerBar) {
            healthBar.maxValue = player.maxhp;
            healthBar.value = player.maxhp;
            text.text = player.maxhp.ToString();
        }
        else
        {
            healthBar.maxValue = enemy.maxhp;
            healthBar.value = enemy.maxhp;
            text.text = enemy.maxhp.ToString();
        }
        
    }
    public void UpdateBar()
    {
        if (isPlayerBar)
        {
            healthBar.value = player.hp;
            text.text = player.hp.ToString();
        }
        else
        {
            healthBar.value = enemy.hp;
            text.text = enemy.hp.ToString();
        }
    }
}
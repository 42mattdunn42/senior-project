using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public HealthBar enemyBar;
    public HealthBar playerBar;

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        enemyBar = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponent<HealthBar>();
        playerBar = GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<HealthBar>();
    }
    public void updateEnemyBar()
    {
        enemyBar.UpdateBar();
    }
    public void updatePlayerBar()
    {
        playerBar.UpdateBar();
    }
}

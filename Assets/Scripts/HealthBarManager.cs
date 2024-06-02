using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public HealthBar enemyBar;
    public HealthBar playerBar;

    public void updateEnemyBar()
    {
        enemyBar.UpdateBar();
    }
    public void updatePlayerBar()
    {
        playerBar.UpdateBar();
    }
}

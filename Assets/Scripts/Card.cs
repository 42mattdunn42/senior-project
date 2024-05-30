using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool hasBeenPlayed;
    public int handIndex;
    private FightManager fm;
    // Start is called before the first frame update
    void Start()
    {
        fm = FindObjectOfType<FightManager>();
    }

    private void OnMouseDown()
    {
        if (hasBeenPlayed == false)
        {
            
            if (fm.actionPoints >= 1) //checks if AP is available
            {
                transform.position += Vector3.up * 2;
                hasBeenPlayed = true;
                fm.availableCardSlots[handIndex] = true;
                Invoke("PlayCard", 2f);
            }
        }
    }

    void PlayCard()
    {
        fm.discardPile.Add(this);
        gameObject.SetActive(false);
        fm.actionPoints--;
        fm.UpdateActionPoints();
    }
}

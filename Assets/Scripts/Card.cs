using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool hasBeenPlayed;
    public int handIndex;
    private FightManager fm;
    private RectTransform playRectTransform;
    private RectTransform burnRectTransform;
    // Start is called before the first frame update
    void Start()
    {
        fm = FindObjectOfType<FightManager>();
        playRectTransform = GameObject.Find("Play Area").GetComponent<RectTransform>();
        burnRectTransform = GameObject.Find("Burn Card Area").GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsPointerOverUIObject(eventData, playRectTransform))
        {
            Debug.Log("Card played");
            PlayCard();
            hasBeenPlayed = true;
            fm.availableCardSlots[handIndex] = true;
        }

        if (IsPointerOverUIObject(eventData, burnRectTransform))
        {
            Debug.Log("Card burned");
            BurnCard();
            hasBeenPlayed = true;
            fm.availableCardSlots[handIndex] = true;
        }
    }


    /*private void OnMouseDown()
    {
        if (hasBeenPlayed == false)
        {
            
            if (fm.actionPoints >= 1) //checks if AP is available
            {
                transform.position += Vector3.up * 2;
                hasBeenPlayed = true;
                fm.availableCardSlots[handIndex] = true;
                Invoke("PlayCard", .5f);
            }
        }
    }*/

    private bool IsPointerOverUIObject(PointerEventData eventData, RectTransform target)
    {
        Vector2 localMousePosition = target.InverseTransformPoint(eventData.position);
        return target.rect.Contains(localMousePosition);
    }

    void PlayCard()
    {
        fm.discardPile.Add(this);
        gameObject.SetActive(false);
        fm.actionPoints--;
        fm.UpdateActionPoints();
    }

    void BurnCard()
    {
        fm.discardPile.Add(this);
        gameObject.SetActive(false);
        if (fm.actionPoints <= 4)
        {
            fm.actionPoints = fm.actionPoints + 1;
        }
        else
        {
            fm.actionPoints =  fm.maxActionPoints;
        }
        fm.UpdateActionPoints();
    }
}

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
    private RectTransform panelRectTransform;
    // Start is called before the first frame update
    void Start()
    {
        fm = FindObjectOfType<FightManager>();
        panelRectTransform = GameObject.Find("Play Area").GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsPointerOverUIObject(eventData, panelRectTransform))
        {
            Debug.Log("Card played");
            PlayCard();
            hasBeenPlayed = true;
            // Perform any additional logic for playing the card
        }
        Debug.Log("End Drag");
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private GameObject deck;
    private GameObject canvas;

    void Awake()
    {
        // Assign the current GameObject (DeckManager script is attached to) to 'deck'
        deck = gameObject;

        // Find the Canvas GameObject in the scene
        canvas = GameObject.Find("Canvas"); // Adjust this to find your Canvas GameObject by name
    }

    public void UnparentDeck()
    {
        // Check if the deck has a parent (i.e., it's not already unparented)
        if (deck.transform.parent != null)
        {
            // Unparent the deck from its current parent (Canvas)
            deck.transform.SetParent(null);
        }
    }

    public void DisableDeckChildren()
    {
        // Loop through all children of the deck and deactivate them
        foreach (Transform child in deck.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ReparentDeckToCanvas()
    {
        // Find the Canvas GameObject in case it has changed
        canvas = GameObject.Find("Canvas");

        // Reparent the deck under the Canvas GameObject
        deck.transform.SetParent(canvas.transform);
    }
}

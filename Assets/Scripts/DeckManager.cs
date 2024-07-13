using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private static DeckManager _instance;
    private GameObject deck;
    private GameObject canvas;
    private GameObject gm;

    void Awake()
    {
        // Implement singleton pattern
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        // Assign the current GameObject (DeckManager script is attached to) to 'deck'
        deck = gameObject;

        // Find the Canvas GameObject in the scene
        canvas = GameObject.Find("Canvas"); // Adjust this to find your Canvas GameObject by name

        gm = GameObject.Find("GameManager");
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

    public void ReparentDeckToGM()
    {
        // Find the Canvas GameObject in case it has changed
        gm = GameObject.Find("GameManager");

        // Reparent the deck under the Canvas GameObject
        deck.transform.SetParent(gm.transform);
    }
}

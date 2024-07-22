using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    private GameManager gm;
    public List<Card> availableCards = new List<Card>();
    private List<Card> cardsInShop = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardSlots;
    public List<TextMeshProUGUI> priceTags = new List<TextMeshProUGUI>();
    public TextMeshProUGUI credits;
    public AudioSource failedBuySound;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        Dictionary<int, bool> duplicateCatcher = new Dictionary<int, bool>();

        for (int i = 0; i < availableCardSlots.Length; i++)
        {
            bool cardSelected = false;
            int selection = 1;
            while (!cardSelected)
            {
                selection = Random.Range(0, availableCards.Count);
                try
                {
                    duplicateCatcher.Add(selection, false);
                    cardSelected = true;
                }
                catch
                {
                    Debug.Log("duplicate");
                }
            }

            Card randCard = availableCards[selection];
            if (availableCardSlots[i] == true)
            {
                // display card
                randCard.gameObject.SetActive(true);
                randCard.handIndex = i;
                randCard.transform.position = cardSlots[i].position;
                randCard.transform.localScale = new(0.5f,0.5f,0.5f);
                availableCardSlots[i] = false;

                // display price
                priceTags[i].text = randCard.shopCost.ToString();

                cardsInShop.Add(randCard);
            }
        }

        credits.text = "Credits: " + gm.getPlayerCredits();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            OnMouseUp();
        }
    }

    private void OnMouseUp()
    {
        Debug.Log("MouseUp");
        Vector2 mousePos = Input.mousePosition;

        for (int i = 0; i < cardsInShop.Count; i++)
        {
            Card c = cardsInShop[i];

            if (c.gameObject.activeSelf)
            {
                RectTransform rectTransform = c.transform.GetComponent<RectTransform>();

                // Check if the mouse is over the card
                if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos, Camera.main))
                {
                    // Check for funds
                    if (gm.getPlayerCredits() >= c.shopCost)
                    {
                        gm.playBuySound();

                        // Buy card
                        Debug.Log("Bought card: " + c.name);

                        gm.spendCredits(c.shopCost);
                        credits.text = "Credits: " + gm.getPlayerCredits();

                        // Add card to deck
                        gm.AddCardToDeck(c);

                        c.gameObject.SetActive(false);
                        priceTags[i].gameObject.SetActive(false);

                        return;
                    }
                    else
                    {
                        Debug.Log("Insufficient funds");
                        failedBuySound.Play();
                    }
                }
            }
        }
    }
}

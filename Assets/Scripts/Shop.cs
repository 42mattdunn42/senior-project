using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private GameManager gm;
    public List<Card> availableCards = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardSlots;
    public List<TextMeshProUGUI> priceTags = new List<TextMeshProUGUI>();
    public TextMeshProUGUI credits;

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
            }
        }

        credits.text = "Credits: " + gm.getPlayerCredits();
    }
}

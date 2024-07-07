using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private GameManager gm;
    private List<Card> availableCards = new List<Card>();
    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    // Start is called before the first frame update
    void Start()
    {
        //gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        /*Card randCard = availableCards[Random.Range(0, availableCards.Count)];
        for (int i = 0; i < availableCardSlots.Length; i++)
        {
            if (availableCardSlots[i] == true)
            {
                randCard.gameObject.SetActive(true);
                randCard.handIndex = i;
                randCard.transform.position = cardSlots[i].position;
                availableCardSlots[i] = false;
                return;
            }
        }*/
    }
}

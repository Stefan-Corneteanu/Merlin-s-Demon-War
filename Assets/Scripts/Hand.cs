using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hand
{
    private const int handSize = 3;
    public Card[] handCards = new Card[handSize];
    public Transform[] handCardPos = new Transform[handSize];
    public string[] animNames = new string[handSize];
    public bool isPlayerHand;
    
    public void RemoveCard(Card card)
    {
        for (int i = 0; i < 3; i++)
        {
            if (handCards[i] == card)
            {
                //destroy card
                GameObject.Destroy(handCards[i].gameObject);
                handCards[i] = null;

                //add new card
                if (isPlayerHand)
                {
                    GameController.instance.playerDeck.DealCard(this);
                }
                else
                {
                    GameController.instance.enemyDeck.DealCard(this);
                }
                break;
            } 
        }
    }

    internal void ClearHand()
    {
        for (int i = 0; i < handCards.Length; i++)
        {
            GameObject.Destroy(handCards[i].gameObject);
            handCards[i] = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    public List<CardData> deckCardsDatas = new List<CardData>();

    internal void Create()
    {
        //first create a list of card data for the pack
        List<CardData> cardDataInOrder = new List<CardData>();
        foreach (CardData cardData in GameController.instance.cards)
        {
            for (int i = 0; i < cardData.noOccsInDeck; i++)
            {
                cardDataInOrder.Add(cardData);
            }
        }

        //then randomize it
        while (cardDataInOrder.Count > 0)
        {
            int idx = Random.Range(0,cardDataInOrder.Count);
            deckCardsDatas.Add(cardDataInOrder[idx]);
            cardDataInOrder.RemoveAt(idx);
        }
    }

    private CardData RandomCard()
    {
        CardData result = null;

        if (deckCardsDatas.Count == 0)
        {
            Create();
        }

        result = deckCardsDatas[0];
        deckCardsDatas.RemoveAt(0);

        return result;
    }

    private Card CreateNewCard(Vector3 pos, string animName)
    {
        GameObject newCard = GameObject.Instantiate(GameController.instance.cardPrefab,
                                                    GameController.instance.canvas.gameObject.transform);

        newCard.transform.position = pos;
        Card card = newCard.GetComponent<Card>();
        if (card)
        {
            card.cardData = RandomCard();
            card.Initialise();
            Animator animator = newCard.GetComponentInChildren<Animator>();
            if (animator)
            {
                if (!card.gameObject.activeSelf)
                {
                    card.gameObject.SetActive(true);
                }
                animator.CrossFade(animName,0);
            }
            else
            {
                Debug.LogError("No animator component found!");
            }
            return card;
        }
        else
        {
            Debug.LogError("No card component found!");
            return null;
        }
    }

    internal void DealCard(Hand hand)
    {
        for (int i = 0; i < hand.handCards.Length; i++)
        {
            if (hand.handCards[i] == null)
            {
                if (hand.isPlayerHand)
                {
                    GameController.instance.player.PlayDealSound();
                }
                else
                {
                    GameController.instance.enemy.PlayDealSound();
                }
                hand.handCards[i] = CreateNewCard(hand.handCardPos[i].position, hand.animNames[i]);
                return;
            }
        }
    }
}

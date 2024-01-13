using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardData cardData = null;

    public Text titleText = null;
    public Text descText = null;
    public Image costImg = null;
    public Image dmgImg = null;
    public Image cardImg = null;
    public Image frameImg = null;
    public Image burnImg = null;
    public void Initialise()
    {
        if (cardData == null)
        {
            Debug.LogError("Card has no card data!");
            return;
        }

        titleText.text = cardData.cardTitle;
        descText.text = cardData.description;
        costImg.sprite = GameController.instance.healthNumbers[cardData.cost];
        dmgImg.sprite = GameController.instance.dmgNumbers[cardData.dmg];
        cardImg.sprite = cardData.cardImg;
        frameImg.sprite = cardData.frameImg;
    }
}

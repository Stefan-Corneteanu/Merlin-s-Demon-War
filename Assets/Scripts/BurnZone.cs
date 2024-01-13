using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BurnZone : MonoBehaviour, IDropHandler
{
    public AudioSource burnAudio = null;
    public void OnDrop(PointerEventData eventData)
    {
        if (GameController.instance.isPlayable)
        {
            GameObject obj = eventData.pointerDrag;
            Card card = obj.GetComponent<Card>();
            if (card != null)
            {
                GameController.instance.playerHand.RemoveCard(card);
                PlayBurnSound();
                GameController.instance.NextPlayersTurn();
            }
        }
    }

    internal void PlayBurnSound()
    {
        burnAudio.Play();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 origPos;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameController.instance.isPlayable)
        {
            origPos = transform.position;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameController.instance.isPlayable)
        {
            transform.position += (Vector3)eventData.delta;
            Card card = GetComponent<Card>();

            bool overCard = false;

            foreach (GameObject hover in eventData.hovered)
            {
                BurnZone bz = hover.GetComponent<BurnZone>();
                Player player = hover.GetComponent<Player>();

                if (bz != null)
                {
                    card.burnImg.gameObject.SetActive(true);
                }
                else
                {
                    card.burnImg.gameObject.SetActive(false);
                }

                if (player != null)
                {
                    if (GameController.instance.IsCardValid(card, player, GameController.instance.playerHand))
                    {
                        player.glowImg.gameObject.SetActive(true);
                        overCard = true;
                    }
                }
            }
            if (!overCard)
            {
                GameController.instance.player.glowImg.gameObject.SetActive(false);
                GameController.instance.enemy.glowImg.gameObject.SetActive(false);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = origPos;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}

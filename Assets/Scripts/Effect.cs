using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    public Player targetPlayer = null;
    public Card srcCard = null;
    public Image effectImg = null;

    public AudioSource iceAudio = null;
    public AudioSource fireballAudio = null;
    public AudioSource boomAudio = null;
    public void EndTrigger()
    {
        bool bounce = false;
        if (targetPlayer.HasMirror())
        {
            bounce = true;
            targetPlayer.SetMirror(false);
            targetPlayer.PlaySmashSound();
            if (targetPlayer.isPlayer)
            {
                GameController.instance.CastAttkEffect(srcCard,GameController.instance.enemy);
            }
            else
            {
                GameController.instance.CastAttkEffect(srcCard, GameController.instance.player);
            }
        }
        else
        {
            int dmg = srcCard.cardData.dmg;
            if (!targetPlayer.isPlayer)
            {
                if (srcCard.cardData.dmgType == CardData.DamageType.FIRE && targetPlayer.isFire
                 || srcCard.cardData.dmgType == CardData.DamageType.ICE && !targetPlayer.isFire)
                {
                    dmg /= 2;
                }
            }
            targetPlayer.health -= dmg;
            targetPlayer.playHitAnim();

            GameController.instance.UpdateHealths();
            
            if (targetPlayer.health <= 0)
            {
                targetPlayer.health = 0;
                if (targetPlayer.isPlayer)
                {
                    GameController.instance.PlayPlayerDieSound();
                }
                else
                {
                    GameController.instance.PlayDemonDieSound();
                }
            }

            if (!bounce)
            {
                GameController.instance.NextPlayersTurn();
            }
            GameController.instance.isPlayable = true;
        }

        Destroy(gameObject);
    }

    internal void PlayIceSound()
    {
        iceAudio.Play();
    }

    internal void PlayFireballSound()
    {
        fireballAudio.Play();
    }

    internal void PlayBoomSound()
    {
        boomAudio.Play();
    }
}

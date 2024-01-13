using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    public Sprite[] healthNumbers = new Sprite[10];
    public Sprite[] dmgNumbers = new Sprite[10];

    public List<CardData> cards = new List<CardData>();

    public Player player = null;
    public Player enemy = null;

    public Deck playerDeck = new Deck();
    public Deck enemyDeck = new Deck();

    public Hand playerHand = new Hand();
    public Hand enemyHand = new Hand();

    public GameObject cardPrefab = null;
    public GameObject effectLTRPrefab = null;
    public GameObject effectRTLPrefab = null;

    public Canvas canvas = null;

    public Sprite iceDemonSprite = null;
    public Sprite fireDemonSprite = null;
    public Sprite fireballSprite = null;
    public Sprite iceballSprite = null;
    public Sprite multiFireballSprite = null;
    public Sprite multiIceballSprite = null;
    public Sprite fireAndIceSprite = null;
    public Sprite destroySprite = null;

    public bool isPlayable = false;

    public bool isPlayersTurn = true;

    public Text turnText = null;
    public Text scoreText = null;

    public int score = 0;
    public int noDemonsKilled = 0;

    public Image enemySkipTurn = null;

    public AudioSource playerDieAudio = null;
    public AudioSource demonDieAudio = null;
    private void Awake()
    {
        instance = this;
        SetupNewEnemy();
        playerDeck.Create();
        enemyDeck.Create();
        StartCoroutine(DealHands());
    }
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }

    public void SkipTurn()
    {
        if (isPlayersTurn && isPlayable)
        {
            NextPlayersTurn();
        }
    }

    internal IEnumerator DealHands()
    {
        for (int i = 0; i < playerHand.handCards.Length; i++)
        {
            playerDeck.DealCard(playerHand);
            yield return new WaitForSeconds(0.25f);
        }

        for (int i = 0; i < enemyHand.handCards.Length; i++)
        {
            enemyDeck.DealCard(enemyHand);
            yield return new WaitForSeconds(0.25f);
        }
        isPlayable = true;
    }

    internal bool UseCard(Card card, Player targetPlayer, Hand srcHand)
    {
        if (IsCardValid(card, targetPlayer, srcHand))
        {
            isPlayable = false;
            CastCard(card, targetPlayer, srcHand);
            targetPlayer.glowImg.gameObject.SetActive(false);
            srcHand.RemoveCard(card);
            return true;
        }
        else
        {
            return false;
        }
    }

    internal bool IsCardValid(Card card, Player targetPlayer, Hand srcHand)
    {
        bool valid = false;
        if (card != null)
        {
            if (srcHand.isPlayerHand)
            {
                if (card.cardData.cost <= player.mana)
                {
                    if (targetPlayer.isPlayer == card.cardData.isDefCard) //defense card on yourself or offense card (!def) on enemy (!player)
                    {
                        valid = true;
                    }
                }
            }
            else //enemy hand
            {
                if (card.cardData.cost <= enemy.mana)
                {
                    if (targetPlayer.isPlayer != card.cardData.isDefCard) //defense card on enemy or offense card on player
                    {
                        valid = true;
                    }
                }
            }
        }
        return valid;
    }

    private void CastCard(Card card, Player targetPlayer, Hand srcHand)
    {
        if (card.cardData.isMirrorCard)
        {
            targetPlayer.SetMirror(true);
            targetPlayer.PlayMirrorSound();
            NextPlayersTurn();
            isPlayable = true;
        }
        else
        {
            if (card.cardData.isDefCard) //healthCard
            {
                targetPlayer.health += card.cardData.dmg;
                targetPlayer.PlayHealSound();
                if (targetPlayer.health > targetPlayer.maxHealth)
                {
                    targetPlayer.health = targetPlayer.maxHealth;
                }
                UpdateHealths();
                StartCoroutine(CastHealEffect(targetPlayer));
            }
            else //attk card
            {
                CastAttkEffect(card, targetPlayer);
            }
            
            if (srcHand.isPlayerHand)
            {
                score += card.cardData.dmg;
                UpdateScore();
            }
        }

        if (srcHand.isPlayerHand)
        {
            player.mana -= card.cardData.cost;
            player.UpdateManaBalls();
        }
        else
        {
            enemy.mana -= card.cardData.cost;
            enemy.UpdateManaBalls();
        }
    }

    private IEnumerator CastHealEffect(Player targetPlayer)
    {
        yield return new WaitForSeconds(0.25f);
        NextPlayersTurn();
        isPlayable = true;
    }

    internal void CastAttkEffect(Card card, Player targetPlayer)
    {
        GameObject effectGO = null;
        if (targetPlayer.isPlayer)
        {
            effectGO = Instantiate(effectRTLPrefab, canvas.gameObject.transform);
        }
        else
        {
            effectGO = Instantiate(effectLTRPrefab, canvas.gameObject.transform);
        }

        Effect effect = effectGO.GetComponent<Effect>();
        if (effect != null)
        {
            effect.targetPlayer = targetPlayer;
            effect.srcCard = card;

            switch (card.cardData.dmgType)
            {
                case CardData.DamageType.FIRE:
                    if (card.cardData.isMultiAttk)
                    {
                        effect.effectImg.sprite = multiFireballSprite;
                    }
                    else
                    {
                        effect.effectImg.sprite = fireballSprite;
                    }
                    effect.PlayFireballSound();
                    break;

                case CardData.DamageType.ICE:
                    if (card.cardData.isMultiAttk)
                    {
                        effect.effectImg.sprite = multiIceballSprite;
                    }
                    else
                    {
                        effect.effectImg.sprite = iceballSprite;
                    }
                    effect.PlayIceSound();
                    break;

                case CardData.DamageType.BOTH:
                    if (card.cardData.isDestruct)
                    {
                        effect.effectImg.sprite = destroySprite;
                        effect.PlayBoomSound();
                    }
                    else
                    {
                        effect.effectImg.sprite = fireAndIceSprite;
                        effect.PlayFireballSound();
                        effect.PlayIceSound();
                    }
                    break;
            }
        }
    }

    internal void UpdateHealths()
    {
        player.UpdateHealth();
        enemy.UpdateHealth();

        if (player.health <= 0)
        {
            StartCoroutine(GameOver());
        }

        if (enemy.health <= 0)
        {
            noDemonsKilled++;
            score += 100;
            UpdateScore();
            StartCoroutine(NewEnemy());
        }
    }

    private IEnumerator NewEnemy()
    {
        enemy.gameObject.SetActive(false);
        enemyHand.ClearHand();
        yield return new WaitForSeconds(1f);
        SetupNewEnemy();
        enemy.gameObject.SetActive(true);
        StartCoroutine(DealHands());
    }

    private void SetupNewEnemy()
    {
        enemy.mana = 0;
        enemy.health = enemy.maxHealth;
        enemy.UpdateHealth();
        enemy.UpdateManaBalls();
        enemy.isFire = new System.Random().Next(0,2)%2==0;
        if (enemy.isFire)
        {
            enemy.playerImg.sprite = fireDemonSprite;
        }
        else
        {
            enemy.playerImg.sprite = iceDemonSprite;
        }
    }

    private IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2);
    }

    internal void NextPlayersTurn()
    {
        bool enemyIsDead = enemy.health <= 0 ;
        if (!enemyIsDead)
        {
            isPlayersTurn = !isPlayersTurn;
        }
        
        if (isPlayersTurn)
        {
            turnText.text = "Merlin's Turn";
            if (player.mana < player.manaBalls.Length)
            {
                player.mana++;
            }
        }
        else
        {
            turnText.text = "Demon's Turn";
            if (!enemyIsDead)
            {
                if (enemy.mana < enemy.manaBalls.Length)
                {
                    enemy.mana++;
                }
            }
        }

        player.UpdateManaBalls();
        enemy.UpdateManaBalls();

        if (!isPlayersTurn && !enemyIsDead)
        {
            DemonAction();
        }
    }

    private void DemonAction()
    {
        Card card = AIChooseCard();
        StartCoroutine(DemonCastCard(card));
    }

    private Card AIChooseCard()
    {
        /*AI upgrade 1: rather than just checking if the card is playable, eliminate cards that wouldn't make sense to play ("dumb" choices)
          cases identified: 
          1) Playing mirror card if enemy already has mirror. 
          2) Playing health cards when enemy has full health.
          3) Playing large heals when small heals give full health.
        */
        Card[] validCards = Array.FindAll(enemyHand.handCards, delegate (Card card)
        {
            bool valid = card != null && card.cardData.cost <= enemy.mana && //non-null card with cost less than enemy's mana
            (!card.cardData.isDefCard //attack cards are always valid
            || (card.cardData.isMirrorCard && !enemy.HasMirror()) //mirror cards are valid unless enemy already has mirror enabled (else it is stupid to play)
            || (!card.cardData.isMirrorCard && card.cardData.isDefCard && card.cardData.dmg + enemy.health < enemy.maxHealth + 2));
            //!mirrorcard but def card = heal card. The formula for card dmg + health less than max health + 2 intends the following:
            //do not use small health if enemy has full health and do not use large health if enemy can fully heal with small health (dmg = 2)
            return valid;
        });

        if (validCards.Length == 0)
        {
            NextPlayersTurn();
            return null;
        }
        else
        {
            /*AI upgrade 2: rather than picking any "smart" choice (any choice not deemed dumb at filtering above), select the smartest choice
            (or random of smartest if multiple)
            smart choice rules:
            1) always play a card that can kill a player
            2) if you can't kill a player seek the effective benefit of the card (damage inflicted (ignore mirror), healing benefit, mirror benefit)
            */
            int[] cardScore = new int[validCards.Length];
            int maxScore = 0;
            List<Card> smartestChoices = new List<Card>();

            for (int i = 0; i < validCards.Length; i++)
            {
                if (!validCards[i].cardData.isDefCard && validCards[i].cardData.dmg >= player.health) //attack card can kill player
                {
                    cardScore[i] = 100; //big number = guarantee kill
                }
                else if (validCards[i].cardData.isMirrorCard)
                {
                    cardScore[i] = 5; //close to avg of dmg of all cards in deck (it was 4.1, rounded to 5 to give AI a sense of self preservation)
                }
                else if (validCards[i].cardData.isDefCard) //health card
                {
                    cardScore[i] = Math.Min(validCards[i].cardData.dmg, enemy.maxHealth-enemy.health); //minimum of full healing effect of card and dmg taken
                }
                else //attk card
                {
                    cardScore[i] = validCards[i].cardData.dmg; //just put the damage value
                }
                if (cardScore[i] > maxScore)
                {
                    maxScore = cardScore[i];
                }
            }
            for (int i = 0; i < validCards.Length; i++)
            {
                if (cardScore[i] == maxScore)
                {
                    smartestChoices.Add(validCards[i]);
                }
            }

            int randidx = UnityEngine.Random.Range(0,smartestChoices.Count);
            return smartestChoices[randidx];
        }
    }
    private IEnumerator DemonCastCard(Card card)
    {
        yield return new WaitForSeconds(1f);
        if (card != null)
        {
            TurnCard(card);

            yield return new WaitForSeconds(2f);

            if (card.cardData.isDefCard)
            {
                UseCard(card, enemy, enemyHand);
            }
            else
            {
                UseCard(card, player, enemyHand);
            }

            yield return new WaitForSeconds(1f);

            enemyDeck.DealCard(enemyHand);

            yield return new WaitForSeconds(1f);
        }
        else
        {
            enemySkipTurn.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            enemySkipTurn.gameObject.SetActive(false);
        }
    }

    internal void TurnCard(Card card)
    {
        Animator animator = card.GetComponentInChildren<Animator>();
        if (animator)
        {
            animator.SetTrigger("Flip");
        }
        else
        {
            Debug.LogError("Animator not found for turning card");
        }
    }
    private void UpdateScore()
    {
        scoreText.text = "Demons killed: " + noDemonsKilled.ToString() + " Score: " + score.ToString();
    }

    internal void PlayPlayerDieSound()
    {
        playerDieAudio.Play();
    }

    internal void PlayDemonDieSound()
    {
        demonDieAudio.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDropHandler
{

    public Image playerImg = null;
    public Image mirrorImg = null;
    public Image healthNoImg = null;
    public Image glowImg = null;
    public Image hitImg = null;

    public GameObject[] manaBalls = new GameObject[5];

    public int maxHealth = 5;
    public int health = 5; // current health
    public int mana = 1;

    public bool isPlayer;
    public bool isFire = false; //is enemy a fire monster?

    private Animator animator = null;

    public AudioSource dealAudio = null;
    public AudioSource mirrorAudio = null;
    public AudioSource smashAudio = null;
    public AudioSource healAudio = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateHealth();
        UpdateManaBalls();
    }

    internal void playHitAnim()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (GameController.instance.isPlayable)
        {
            GameObject obj = eventData.pointerDrag;
            if (obj != null)
            {
                Card card = obj.GetComponent<Card>();
                if (card != null)
                {
                    GameController.instance.UseCard(card,this, GameController.instance.playerHand);
                }
            }
        }
    }
    internal void UpdateHealth()
    {
        if (health >= 0 && health < GameController.instance.healthNumbers.Length)
        {
            healthNoImg.sprite = GameController.instance.healthNumbers[health];
        }
        else if (health < 0)
        {
            health = 0;
            healthNoImg.sprite = GameController.instance.healthNumbers[health];
        }
        else
        {
            health = 9;
            healthNoImg.sprite = GameController.instance.healthNumbers[health];
        }
    }

    internal void SetMirror(bool on)
    {
        mirrorImg.gameObject.SetActive(on);
    }

    internal bool HasMirror()
    {
        return mirrorImg.gameObject.activeInHierarchy;
    }

    internal void UpdateManaBalls()
    {
        for(int i = 0; i < manaBalls.Length; i++)
        {
            if (mana > i)
            {
                manaBalls[i].SetActive(true);
            }
            else
            {
                manaBalls[i].SetActive(false);
            }
        }
    }

    internal void PlayDealSound()
    {
        dealAudio.Play();
    }

    internal void PlayMirrorSound()
    {
        mirrorAudio.Play();
    }

    internal void PlaySmashSound()
    {
        smashAudio.Play();
    }

    internal void PlayHealSound()
    {
        healAudio.Play();
    }

}

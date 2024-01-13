using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Card",menuName = "CardGame/Card")]
public class CardData : ScriptableObject
{
    public enum DamageType
    {
        FIRE,
        ICE,
        BOTH
    }

    public string cardTitle;
    public string description;
    public int cost;
    public int dmg;
    public DamageType dmgType;
    public Sprite cardImg;
    public Sprite frameImg;
    public int noOccsInDeck;
    public bool isDefCard = false;
    public bool isMirrorCard = false;
    public bool isMultiAttk = false;
    public bool isDestruct = false;
}

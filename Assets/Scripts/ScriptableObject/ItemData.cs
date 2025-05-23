using System;
using UnityEngine;

// 아이템 종류
public enum ItemType
{
    Consumable,
    Resource
}

// 소비 아이템 적용효과 종류
public enum ConsumableType
{
    Health,
    MoveSpeed,
    JumpPower
}

// 소비 아이템의 적용효과 종류와 효과를 인스펙터에서 보여지도록 설정
[Serializable]
public class ItemDataConsumable
{
    [SerializeField] private ConsumableType type;
    [SerializeField] private float value;

    public ConsumableType Type => type;
    public float Value => value;
}

// 아이템 하나에 대한 모든 정보를 담는 ScriptableObject 클래스
[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPrefab;

    [Header("Stack")]
    public bool canStack;
    public int maxStackAmount;

    [Header("Consumable")]
    public ItemDataConsumable[] consumables;
}

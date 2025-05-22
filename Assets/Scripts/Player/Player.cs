using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller; // 플레이어 이동 및 입력 제어
    public PlayerCondition condition; // 플레이어 상태 관리

    public ItemData itemData; // 아이템 데이터
    public Action addItem; // 아이템 추가 시 실행할 델리게이트
    public Transform dropPosition; // 바닥에 드랍할 아이템 위치

    void Awake()
    {
        CharacterManager.Instance.Player = this; // 싱글톤 CharacterManager에 자신을 등록
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
    }
}

using System.Collections.Generic;
using UnityEngine;

public class BuffSlotPool : MonoBehaviour
{
    public GameObject buffSlotPrefab; // 생성할 버프 슬롯 프리팹
    public Transform buffSlotRoot; // 버프 슬롯이 붙을 부모 오브젝트
    private Queue<BuffSlot> pool = new Queue<BuffSlot>(); // 재사용할 버프 슬롯 담는 큐

    // 사용 가능한 버프 슬롯 가져옴, 없으면 새로 생성
    public BuffSlot GetBuffSlot()
    {
        BuffSlot slot;

        if (pool.Count > 0) // 기준 풀에서 꺼냄
        {
            slot = pool.Dequeue();
        }
        else // 없으면 생성
        {
            GameObject obj = Instantiate(buffSlotPrefab, buffSlotRoot);
            slot = obj.GetComponent<BuffSlot>();
        }

        slot.gameObject.SetActive(true);
        return slot;
    }

    // 사용 끝난 버프 슬롯 풀에 반환
    public void Return(BuffSlot slot)
    {
        slot.gameObject.SetActive(false);
        pool.Enqueue(slot);
    }
}

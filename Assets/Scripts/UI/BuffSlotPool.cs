using System.Collections.Generic;
using UnityEngine;

public class BuffSlotPool : MonoBehaviour
{
    public GameObject buffSlotPrefab;
    public Transform buffSlotRoot;
    private Queue<BuffSlot> pool = new Queue<BuffSlot>();

    public BuffSlot GetBuffSlot()
    {
        BuffSlot slot;

        if (pool.Count > 0)
        {
            slot = pool.Dequeue();
        }
        else
        {
            GameObject obj = Instantiate(buffSlotPrefab, buffSlotRoot);
            slot = obj.GetComponent<BuffSlot>();
        }

        slot.gameObject.SetActive(true);
        return slot;
    }

    public void Return(BuffSlot slot)
    {
        slot.gameObject.SetActive(false);
        pool.Enqueue(slot);
    }
}

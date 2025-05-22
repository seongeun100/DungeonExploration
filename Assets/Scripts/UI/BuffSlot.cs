using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI timerText;

    private float duration;
    private BuffSlotPool pool; // 풀 참조

    public void Init(BuffSlotPool poolRef)
    {
        pool = poolRef;
    }

    public void SetBuff(Sprite sprite, float time)
    {
        icon.sprite = sprite;
        duration = time;
        gameObject.SetActive(true);
    }

    void Update()
    {
        duration -= Time.deltaTime;
        timerText.text = $"{duration:0}";

        if (duration <= 0f)
        {
            pool.Return(this);
        }
    }
}

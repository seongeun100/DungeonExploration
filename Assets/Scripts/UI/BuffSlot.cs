using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlot : MonoBehaviour
{
    public Image icon; // 버프 아이콘
    public TextMeshProUGUI timerText; // 버프 남은 시간 텍스트

    private float duration; //현재 남은 시간
    private BuffSlotPool pool; // 풀 참조

// 버프 정보, 풀 참조를 설정하고 활성화
    public void SetBuff(Sprite sprite, float time, BuffSlotPool pool)
    {
        this.pool = pool;
        icon.sprite = sprite;
        duration = time;
        gameObject.SetActive(true);
    }

    // 매 프레임 남은 시간을 감소시키고 UI 업데이트
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

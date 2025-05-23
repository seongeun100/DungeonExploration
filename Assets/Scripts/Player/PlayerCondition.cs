using System.Collections;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    [SerializeField] private BuffSlotPool buffPool; // 버프 UI 슬롯 풀
    public UICondition uiCondition; // UICondition 참조
    private PlayerController controller; // 플레이어 조작 컴포넌트 참조
    private float speedBonus; // 이동속도 버프 수치
    private float jumpBonus; // 점프력 버프 수치
    private float buffDuration = 10f; // 버프 지속 시간

    private Coroutine speedBuffTimer;
    private Coroutine jumpBuffTimer;

    private BuffSlot speedBuffSlot;
    private BuffSlot jumpBuffSlot;

    // UICondition에서 참조된 체력과 스태미나 가져오기
    Condition health => uiCondition.Health;
    Condition stamina => uiCondition.Stamina;

    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        health.Add(stamina.paasiveValue * Time.deltaTime); // 체력 자동 회복

        // 벽타기 중이 아니면 스태미나 자동 회복
        if (!controller.IsWallClinging())
        {
            stamina.Add(stamina.paasiveValue * Time.deltaTime);
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void AddMoveSpeed(float amount, Sprite icon)
    {
        // 기존 버프 제거
        if (speedBonus != 0f)
            controller.moveSpeed -= speedBonus;

        // 새 수치 적용
        speedBonus = amount;
        controller.moveSpeed += speedBonus;

        // 기존 UI 제거
        if (speedBuffSlot != null)
        {
            buffPool.Return(speedBuffSlot);
            speedBuffSlot = null;
        }

        // UI 버프 슬롯 새로 적용
        speedBuffSlot = buffPool.GetBuffSlot();
        speedBuffSlot.SetBuff(icon, buffDuration, buffPool);

        // 코루틴 타이머 재시작
        if (speedBuffTimer != null)
            StopCoroutine(speedBuffTimer);
        speedBuffTimer = StartCoroutine(ResetSpeedBuffAfterDelay(buffDuration));
    }

    public void AddJumpPower(float amount, Sprite icon)
    {
        if (jumpBonus != 0f)
            controller.jumpPower -= jumpBonus;

        jumpBonus = amount;
        controller.jumpPower += jumpBonus;

        if (jumpBuffSlot != null)
        {
            buffPool.Return(jumpBuffSlot);
            jumpBuffSlot = null;
        }

        jumpBuffSlot = buffPool.GetBuffSlot();
        jumpBuffSlot.SetBuff(icon, buffDuration, buffPool);

        if (jumpBuffTimer != null)
            StopCoroutine(jumpBuffTimer);
        jumpBuffTimer = StartCoroutine(ResetJumpBuffAfterDelay(buffDuration));
    }

    // 일정 시간 후 버프 제거하는 코루틴
    IEnumerator ResetSpeedBuffAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        controller.moveSpeed -= speedBonus; // 기존 수치 복구
        speedBonus = 0f; // 버프 수치 초기화
        speedBuffTimer = null; // 실행 중인 버프 타이머 null로 설정

        // 버프 슬롯이 존재하면 반환
        if (speedBuffSlot != null)
        {
            buffPool.Return(speedBuffSlot);
            speedBuffSlot = null;
        }
    }

    IEnumerator ResetJumpBuffAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        controller.jumpPower -= jumpBonus;
        jumpBonus = 0f;
        jumpBuffTimer = null;

        if (jumpBuffSlot != null)
        {
            buffPool.Return(jumpBuffSlot);
            jumpBuffSlot = null;
        }
    }

    // 스태미나 사용
    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0)
        {
            return false;
        }
        stamina.Subtract(amount);
        return true;
    }
}

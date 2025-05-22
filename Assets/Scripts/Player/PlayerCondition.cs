using System.Collections;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uiCondition;
    public BuffSlotPool buffPool;
    private PlayerController controller;
    private float speedBonus;
    private float jumpBonus = 0f;
    private float buffDuration = 10f;

    private Coroutine speedBuffTimer;
    private Coroutine jumpBuffTimer;

    private BuffSlot speedBuffSlot;
    private BuffSlot jumpBuffSlot;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float noHungerHealthDecay;

    void Start()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        hunger.Subtract(hunger.paasiveValue * Time.deltaTime);
        stamina.Add(stamina.paasiveValue * Time.deltaTime);

        if (hunger.curValue == 0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if (health.curValue == 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        health.Add(amount);
    }

    public void AddMoveSpeed(float amount, Sprite icon)
    {
        // 기존 수치 제거
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
        speedBuffSlot.Init(buffPool);
        speedBuffSlot.SetBuff(icon, buffDuration);

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
        jumpBuffSlot.Init(buffPool);
        jumpBuffSlot.SetBuff(icon, buffDuration);

        if (jumpBuffTimer != null)
            StopCoroutine(jumpBuffTimer);
        jumpBuffTimer = StartCoroutine(ResetJumpBuffAfterDelay(buffDuration));
    }

    IEnumerator ResetSpeedBuffAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        controller.moveSpeed -= speedBonus;
        speedBonus = 0f;
        speedBuffTimer = null;

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

    public void Die()
    {
        // Debug.Log("사망");
    }

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

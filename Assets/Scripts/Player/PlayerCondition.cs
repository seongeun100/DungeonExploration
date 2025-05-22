using System.Collections;
using UnityEngine;

public class PlayerCondition : MonoBehaviour
{
    public UICondition uiCondition;
    public BuffSlotPool buffPool;
    private PlayerController controller;
    private float speedBonus = 0f;
    private float jumpBonus = 0f;
    private float buffDuration = 10f;
    private Coroutine buffTimer;

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
        controller.moveSpeed += amount;
        speedBonus += amount;
        BuffSlot slot = buffPool.GetBuffSlot();
        slot.Init(buffPool);
        slot.SetBuff(icon, buffDuration);
        StartBuffTimer();
    }

    public void AddJumpPower(float amount, Sprite icon)
    {
        controller.jumpPower += amount;
        jumpBonus += amount;
        BuffSlot slot = buffPool.GetBuffSlot();
        slot.Init(buffPool);
        slot.SetBuff(icon, buffDuration);
        StartBuffTimer();
    }

    void StartBuffTimer()
    {
        if (buffTimer != null)
        {
            StopCoroutine(buffTimer);
        }
        buffTimer = StartCoroutine(ResetBuffAfterDelay(buffDuration));
    }

    IEnumerator ResetBuffAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        controller.moveSpeed -= speedBonus;
        controller.jumpPower -= jumpBonus;

        speedBonus = 0f;
        jumpBonus = 0f;
        buffTimer = null;
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

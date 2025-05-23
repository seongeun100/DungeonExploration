using UnityEngine;

public class UICondition : MonoBehaviour
{
    [SerializeField] private Condition health;
    [SerializeField] private Condition stamina;

    public Condition Health => health;
    public Condition Stamina => stamina;

    void Start()
    {
        CharacterManager.Instance.Player.condition.uiCondition = this;
    }

}

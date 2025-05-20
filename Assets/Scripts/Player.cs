using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;

    void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
    }
}

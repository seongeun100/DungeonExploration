using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data; // 아이템 데이터

    // 플레이어가 오브젝트에 마우스를 올렸을 때 보여줄 UI 텍스트
    public string GetInteractPrompt()
    {
        string str = $"{data.displayName}\n{data.description}";
        return str;
    }

    // 플레이어가 해당 아이템과 상호작용할 때
    public void OnInteract()
    {
        CharacterManager.Instance.Player.itemData = data; // 플레이어에게 아이템 데이터 전달
        CharacterManager.Instance.Player.addItem?.Invoke(); // 아이템 추가 이벤트 호출
        Destroy(gameObject); // 필드에 존재하던 오브젝트 제거
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quatityText;

    public ItemData item; // 아이템 데이터
    public UIInventory inventory;
    public int index;
    public int quantity;
    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    // 아이템 정보 표시
    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        quatityText.text = quantity > 1 ? quantity.ToString() : string.Empty;
    }

    // 아이템 정보 초기화
    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quatityText.text = string.Empty;
    }

    // 슬롯 클릭시 호출되는 함수
    public void OnClickButton()
    {
        inventory.SelectItem(index);
    }
}
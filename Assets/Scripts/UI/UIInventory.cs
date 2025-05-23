using TMPro;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryWindow;
    [SerializeField] private Transform slotPanel;
    [SerializeField] private Transform dropPosition;

    [Header("Select Item")]
    [SerializeField] private TextMeshProUGUI selectedItemName;
    [SerializeField] private TextMeshProUGUI selectedItemDescription;
    [SerializeField] private TextMeshProUGUI selectedItemStatName;
    [SerializeField] private TextMeshProUGUI selectedItemStatValue;
    [SerializeField] private GameObject useButton;
    [SerializeField] private GameObject dropButton;
    private ItemData selectedItem;
    private int selectedItemIndex;

    private ItemSlot[] slots; // 인벤토리 표시될 슬롯
    private PlayerController controller;
    private PlayerCondition condition;

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        // 인벤토리 열기,닫기와 아이템 추가 이벤트 등록
        controller.inventory += Toggle;
        CharacterManager.Instance.Player.addItem += AddItem;

        inventoryWindow.SetActive(false); // 인벤토리 초기상태

        // 아이템 슬롯만큼 배열에 할당하고 초기화
        slots = new ItemSlot[slotPanel.childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
            slots[i].Clear();
        }

        ClearSelectedItemWindow();
    }

    // 선택 아이템 정보 UI 초기화
    void ClearSelectedItemWindow()
    {
        selectedItem = null;

        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        useButton.SetActive(false);
        dropButton.SetActive(false);
    }

    // 인벤토리 열고 닫기
    public void Toggle()
    {
        if (IsOpen())
        {
            inventoryWindow.SetActive(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
        }
    }

    // 인벤토리 열려있는지 확인
    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    // 아이템 추가 처리
    public void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData;

        // 스택가능한 아이템이면 기존 슬롯에 추가
        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                CharacterManager.Instance.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        // 빈 슬롯에 새로 추가
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null;
            return;
        }

        // 슬롯없으면 땅에 버림
        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null;
    }

    // UI 갱신
    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set();
            }
            else
            {
                slots[i].Clear();
            }
        }
    }

    // 기존 스택에 추가 가능한 슬롯 찾기
    ItemSlot GetItemStack(ItemData data)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    // 빈 슬롯 찾기
    ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }
        return null;
    }

    // 아이템 버리기
    public void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360));
    }

    // 슬롯 클릭 시 선택된 아이템 처리
    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index].item;
        selectedItemIndex = index;

        selectedItemName.text = selectedItem.displayName;
        selectedItemDescription.text = selectedItem.description;

        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        // 소비 아이템 효과 정보
        for (int i = 0; i < selectedItem.consumables.Length; i++)
        {
            selectedItemStatName.text += selectedItem.consumables[i].Type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.consumables[i].Value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.type == ItemType.Consumable);
        dropButton.SetActive(true);
    }

    // 아이템 사용 버튼 클릭 시 호출
    public void OnUseButton()
    {
        if (selectedItem.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.consumables.Length; i++)
            {
                switch (selectedItem.consumables[i].Type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumables[i].Value); break;
                    case ConsumableType.MoveSpeed:
                        condition.AddMoveSpeed(selectedItem.consumables[i].Value, selectedItem.icon); break;
                    case ConsumableType.JumpPower:
                        condition.AddJumpPower(selectedItem.consumables[i].Value, selectedItem.icon); break;
                }
            }
            RemoveSelctedItem();
        }
    }

    // 아이템 버리기 버튼 클릭 시 호출
    public void OnDropButton()
    {
        ThrowItem(selectedItem);
        RemoveSelctedItem();
    }

    // 선택된 아이템 하나 제거
    void RemoveSelctedItem()
    {
        slots[selectedItemIndex].quantity--;

        // 수량이 0 이하가 되면 슬롯 비우기
        if (slots[selectedItemIndex].quantity <= 0)
        {
            slots[selectedItemIndex].Clear();
            selectedItem = null;
            ClearSelectedItemWindow();
        }

        UpdateUI();
    }

}
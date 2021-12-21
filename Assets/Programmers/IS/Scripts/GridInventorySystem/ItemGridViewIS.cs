using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The script controls
/// (1) initiating the grid
/// (2) controlling when to call what grid operation logic
/// The script should be used in pair with ItemGrid scriptable object
/// </summary>

public class ItemGridViewIS : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
{
    [Header("Grid")]
    [SerializeField] private ItemGridIS grid;
    [SerializeField] private GameObject inventoryItem;
    
    [Header("Event")]
    [SerializeField] private GameEvent addItemSuccessful;
    
    [Header("Scriptable Objects")]
    [SerializeField] private ItemDataIS pickedUpItem;
    [SerializeField] private GameObjectIdListValue pickedUpWorldItemIds;

    private InventoryItemIS selectedItem;
    private InventoryItemIS overlapItem;
    private RectTransform rectTrans;

    private bool isCursorInsideGrid;

    private void Awake()
    {
        grid.rectTrans = GetComponent<RectTransform>();
        grid.InitGrid();
        pickedUpItem.ResetItemData();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        ItemIconStickToCursor();

        if (Input.GetMouseButtonDown(1) && !isCursorInsideGrid && selectedItem != null)
        {
            var target = selectedItem.ItemData.WorldItemId;
            Debug.Log("WORLD ID ON SELECTED: " + target);
            var find = pickedUpWorldItemIds.List.FirstOrDefault(x => x.id == target);
            Debug.Log("WORLD ITEM FROM SO: " + find?.worldItem);
            Debug.Log("WORLD ITEM ID FROM SO: " + find?.id);
            
            find?.worldItem.SetActive(true);
            selectedItem.gameObject.SetActive(false);
            selectedItem = null;
        }
    }

    private void ItemIconStickToCursor()
    {
        if (selectedItem != null)
        {
            rectTrans.position = Input.mousePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || !isCursorInsideGrid) return;
        var targetGridCell = grid.GetTileGridPosition(Input.mousePosition);
        
        // Debug.Log("grid cell clicked: " + targetGridCell);
        
        if (pickedUpItem.HasValue) AddItem(targetGridCell);
        else if (selectedItem != null) AddItem(targetGridCell, selectedItem);
        else RemoveItem(targetGridCell);
    }

    private void AddItem(Vector2Int targetGridCell)
    {
        Debug.Log("Add new item is hit");
        var item = Instantiate(inventoryItem);
        item.GetComponent<InventoryItemIS>().Set(pickedUpItem);
        var success = grid.AddItem(item.GetComponent<InventoryItemIS>(), targetGridCell.x, targetGridCell.y);
        if (success)
        {
            pickedUpItem.ResetItemData();
            addItemSuccessful.Raise();
        }
    }
    
    private void AddItem(Vector2Int targetGridCell, InventoryItemIS existingItem)
    {
        Debug.Log("Add back existing item is hit");
        var success = grid.AddItem(existingItem, targetGridCell.x, targetGridCell.y);
        if (success)
        {
            Debug.Log("add back existing item succeeded");
            selectedItem = null;
        }
    }

    private void RemoveItem(Vector2Int targetGridCell)
    {
        Debug.Log("Remove item is hit");
        selectedItem = grid.RemoveItem(targetGridCell.x, targetGridCell.y);
        if (selectedItem != null) rectTrans = selectedItem.GetComponent<RectTransform>();
    }

    public void OnQuickAdd()
    {
        var startSlot = grid.GetFirstAvailableSlot(pickedUpItem.width, pickedUpItem.height);
        // Debug.Log("Available start position: " + startSlot);
        AddItem(startSlot);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isCursorInsideGrid = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isCursorInsideGrid = false;
    }
}

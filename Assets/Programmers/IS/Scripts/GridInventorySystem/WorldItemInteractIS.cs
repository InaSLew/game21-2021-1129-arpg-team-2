using UnityEngine;
using UnityEngine.EventSystems;

public class WorldItemInteractIS : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private float distanceFromCamera;
    [SerializeField] private ItemDataIS pickedUpItem;
    [SerializeField] private ItemGridViewIS grid;
    [SerializeField] private GameEventInt worldItemInstanceId;

    private GridVisibleControllerIS gridVisibleControl;
    private bool isStickToCursor;
    private Camera cam;

    private void Awake()
    {
        gridVisibleControl = FindObjectOfType<GridVisibleControllerIS>();
        cam = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && Input.GetKey(KeyCode.LeftControl))
        {
            Debug.Log("Quick add");
            UpdatePickedUpItemData();
            grid.OnQuickAdd();
        }
        else
        {
            gridVisibleControl.SetGridVisibility(true);
            isStickToCursor = true;
            transform.LookAt(cam.transform);
            UpdatePickedUpItemData();
        }
    }

    private void UpdatePickedUpItemData()
    {
        var currentItem = GetComponent<WorldItemIS>().ItemData;
        pickedUpItem.height = currentItem.height;
        pickedUpItem.width = currentItem.width;
        pickedUpItem.itemIcon = currentItem.itemIcon;
        pickedUpItem.HasValue = true;
    }

    private void Update()
    {
        if (isStickToCursor) ItemStickToCursor();
    }

    private void ItemStickToCursor()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = cam.nearClipPlane + distanceFromCamera;
        var mouseWorldPos = cam.ScreenToWorldPoint(mousePos);
        transform.position = mouseWorldPos;
    }

    public void OnItemAddedSuccess()
    {
        worldItemInstanceId.Raise(gameObject.GetInstanceID());
        gameObject.SetActive(false);
    }
}

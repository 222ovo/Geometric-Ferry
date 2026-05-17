using UnityEngine;

/// <summary>
/// 可拖到 presetGrid 上吸附的 Item：统一 Start / Update、拾取与最近格子吸附逻辑。
/// 子类通过虚方法覆盖表现（缩放、失败回栏、三角形偏移与 Roll 等）。
/// </summary>
public class DraggableGridItem : Item, IDraggable
{
    [Header("调控物体的距离常数")]
    [SerializeField] protected float pickRadius = 0.5f;

    protected virtual void Start()
    {
        if (Camera.main == null)
            Debug.LogWarning("Camera.main is null");

        if (manager == null)
            manager = FindObjectOfType<GameManager>();

        initialPosition = transform.position;
        Find.FindGrid(presetGrid);
    }

    protected virtual void Update()
    {
        if (manager == null || manager.isStart)
            return;

        StartMouseDrag();
        OnAfterStartMouseDragBeforeMove();
        if (isDragging)
            FollowMouse();
        StopMouseDrag();
    }

    /// <summary>在 StartMouseDrag 之后、跟随鼠标之前调用（例如三角形右键旋转）。</summary>
    protected virtual void OnAfterStartMouseDragBeforeMove() { }

    void FollowMouse()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        transform.position = mouseWorldPosition;
    }

    public void StartMouseDrag()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        if (Vector3.Distance(mouseWorldPosition, transform.position) > pickRadius)
            return;

        isDragging = true;
        if (isConnected)
            CancelRegisterGrid();
        isConnected = false;
    }

    public void StopMouseDrag()
    {
        if (!isDragging || manager.isStart || !Input.GetMouseButtonUp(0))
            return;

        int i = FindNearestPlaceableGridIndex(transform.position, pickRadius);
        if (i >= 0)
        {
            GameObject cell = presetGrid[i];
            transform.position = cell.transform.position;
            OnAdjustSnapPosition(i, cell);
            isDragging = false;

            if (!isConnected && cell.CompareTag("grid"))
            {
                isConnected = true;
                transform.SetParent(null);
                OnConnectedToGrid(i, cell);
                RegisterGrid(cell);
            }
            return;
        }

        OnReturnedToInventory();
        isConnected = false;
        isDragging = false;
        transform.position = initialPosition;
    }

    /// <summary>已将 position 对齐格子中心后调用，用于局部偏移等。</summary>
    protected virtual void OnAdjustSnapPosition(int index, GameObject cell) { }

    /// <summary>成功吸附到带 grid 标签的格子后调用（此时 parent 已清空）。</summary>
    protected virtual void OnConnectedToGrid(int index, GameObject cell)
    {
        transform.localScale = Vector3.one;
    }

    /// <summary>未吸附到任何合法格子时：回到底栏/跟随相机等。</summary>
    protected virtual void OnReturnedToInventory()
    {
        transform.SetParent(Camera.main.transform);
    }
}

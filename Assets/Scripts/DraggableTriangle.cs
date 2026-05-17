using UnityEngine;

public class DraggableTriangle : DraggableGridItem
{
    protected override void OnAfterStartMouseDragBeforeMove()
    {
        Roll();
    }

    protected override void OnAdjustSnapPosition(int index, GameObject cell)
    {
        transform.Translate(0, -0.16f, 0);
    }

    protected override void OnConnectedToGrid(int index, GameObject cell)
    {
        transform.localScale = new Vector3(0.9f, 1.12f, 1);
    }

    protected override void OnReturnedToInventory()
    {
        base.OnReturnedToInventory();
        transform.localScale = new Vector3(0.8f, 1f, 0.9f);
    }

    void Roll()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        if (Vector3.Distance(mouseWorldPosition, transform.position) > pickRadius)
            return;

        if (Input.GetMouseButtonDown(1))
            transform.Rotate(new Vector3(0, 0, -90), Space.Self);
    }
}

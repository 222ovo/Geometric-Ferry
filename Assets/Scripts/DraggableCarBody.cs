using UnityEngine;

public class DraggableCarBody : DraggableGridItem
{
    protected override void OnReturnedToInventory()
    {
        base.OnReturnedToInventory();
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }
}

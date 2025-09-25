using UnityEngine;

//This contains all the events (data types) that the game uses

public class CheckToAddInventoryItem : IEvent //Prevent player input to scripts that are subscribed to this event
{
    public InventoryItem InventoryItem;

    public CheckToAddInventoryItem(InventoryItem inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}

public class Interact : IEvent
{

}



/*
public class ChangeToNewCanvas : IEvent //Change current UI canvas to a new one (newCanvas) and if it's a dialogue canvas, determined by (isNewCanvasADialogueCanvas)
{
    public bool IsNewCanvasADialogueCanvas;
    public GameObject NewCanvas;

    public ChangeToNewCanvas(GameObject newCanvas, bool isNewCanvasADialogueCanvas)
    {
        NewCanvas = newCanvas;
        IsNewCanvasADialogueCanvas = isNewCanvasADialogueCanvas;
    }
}
*/

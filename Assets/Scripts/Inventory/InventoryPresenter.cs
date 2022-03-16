using System.Linq;
using UnityEngine;

public class InventoryPresenter : Presenter<Inventory, InventoryView, InventoryPresenter>
{
    public override void OnStart()
    {
        Visible = false;
        SetModel(Object.FindObjectOfType<MinerController>().inventory);
    }

    public override void OnShow()
    {
        OnModelUpdate();
    }

    protected override void OnModelUpdate()
    {
        if (!Visible)
            return;

        int[] ids = model.GetBlocksIDs();
        int[] counts = ids.Select(id => model.GetBlocksCount(id)).ToArray();

        view.Init(ids, counts, model.activeBlockId, id => model.activeBlockId = id);
    }

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Visible = !Visible;
            Time.timeScale = Visible ? 0f : 1f;
        }
    }
}

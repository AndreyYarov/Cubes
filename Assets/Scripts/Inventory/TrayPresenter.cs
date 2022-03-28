using System.Linq;
using UnityEngine;

public class TrayPresenter : Presenter<Inventory, TrayView, TrayPresenter>
{
    public override void OnStart()
    {
        SetModel(Object.FindObjectOfType<MinerController>().inventory);
        OnModelUpdate();
    }

    protected override void OnModelUpdate()
    {
        int[] ids = model.GetActiveBlocksIDs();
        int[] counts = ids.Select(id => model.GetBlocksCount(id)).ToArray();
        string[] keys = Enumerable.Range(1, ids.Length).Select(i => (i % 10).ToString()).ToArray();

        view.Init(ids, counts, keys, model.activeBlockId, id => model.activeBlockId = id);
    }

    public override void OnUpdate()
    {
        for (int i = 0; i < model.slotCount; i++)
        {
            KeyCode key = KeyCode.Alpha0 + (i + 1) % 10;
            if (Input.GetKeyDown(key))
                model.activeSlot = i;
        }
    }
}

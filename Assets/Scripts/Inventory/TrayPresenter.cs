using System.Linq;
using System.Collections;
using System.Collections.Generic;
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

        view.Init(ids, counts, model.activeBlockId, id => model.activeBlockId = id);
    }
}

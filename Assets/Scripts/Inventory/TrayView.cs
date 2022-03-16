using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TrayView : BaseInventoryView<TrayView, TrayPresenter>
{
    public void Init(int[] ids, int[] counts, int selectedId, UnityAction<int> OnSelect) =>
        Init(ids, counts, selectedId, transform, OnSelect);
}

using UnityEngine.Events;

public class TrayView : BaseInventoryView<TrayView, TrayPresenter>
{
    public void Init(int[] ids, int[] counts, string[] keys, int selectedId, UnityAction<int> OnSelect)
    {
        Init(ids, counts, selectedId, transform, OnSelect);
        SetTitles(keys);
    }
}

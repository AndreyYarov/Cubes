using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class BaseInventoryView<V, P> : View<V, P>
    where V : BaseInventoryView<V, P>
    where P : BasePresenter<V, P>, new()
{
    [SerializeField] private InventoryItem m_ItemPrefab;

    private List<InventoryItem> items = new List<InventoryItem>();
    private Stack<InventoryItem> disabledItems = new Stack<InventoryItem>();

    protected void SetTitles(string[] titles)
    {
        if (titles == null || titles.Length != items.Count)
        {
            Debug.LogError("Wrong titles count");
            return;
        }

        for (int i = 0; i < items.Count; i++)
            items[i].SetTitle(titles[i]);
    }

    protected void Init(int[] ids, int[] counts, int selectedId, Transform parent, UnityAction<int> OnSelect)
    {
        if (ids == null || counts == null || ids.Length != counts.Length)
        {
            Debug.LogError("Wrong data on init inventory view");
            return;
        }

        ToggleGroup group = parent.GetComponent<ToggleGroup>();

        if (items.Count < ids.Length)
        {
            for (int i = items.Count; i < ids.Length; i++)
            {
                if (disabledItems.Count == 0)
                    items.Add(Instantiate(m_ItemPrefab, parent));
                else
                {
                    items.Add(disabledItems.Pop());
                    items[i].gameObject.SetActive(true);
                }
                items[i].group = group;
            }
        }
        else if (items.Count > ids.Length)
        {
            for (int i = ids.Length; i < items.Count; i++)
            {
                items[i].gameObject.SetActive(false);
                disabledItems.Push(items[i]);
            }
            items.RemoveRange(ids.Length, items.Count - ids.Length);
        }


        for (int i = 0; i < ids.Length; i++)
        {
            var id = ids[i];
            if (id >= 0)
            {
                BlockInfo blockInfo = BlockDatabase.current[id];
                items[i].Init(blockInfo.sprite, blockInfo.name, counts[i], () => OnSelect(id));
                items[i].enabled = true;
                if (id == selectedId)
                    items[i].Select();
            }
            else
            {
                Debug.Log($"Slot {i + 1} is empty");
                items[i].Init(null, "", 0, null);
                items[i].enabled = false;
            }
        }
    }
}

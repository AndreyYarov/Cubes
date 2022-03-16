using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : IModel
{
    private Dictionary<int, int> blocks = new Dictionary<int, int>();
    private int[] activeSlots;
    private int _activeSlot = 0;

    private UnityEvent OnInventoryUpdate = new UnityEvent();
    public void AddUpdateListener(UnityAction listener) => OnInventoryUpdate.AddListener(listener);
    public void RemoveUpdateListener(UnityAction listener) => OnInventoryUpdate.RemoveListener(listener);

    public int slotCount
    {
        get => activeSlots == null ? 0 : activeSlots.Length;
        set
        {
            if (slotCount == value)
                return;
            activeSlots = new int[value];
            int[] ids = GetBlocksIDs();
            for (int i = 0; i < value && i < ids.Length; i++)
                activeSlots[i] = ids[i];
            for (int i = ids.Length; i < value; i++)
                activeSlots[i] = -1;
            OnInventoryUpdate.Invoke();
        }
    }
    public int activeSlot
    {
        get => _activeSlot;
        set
        {
            if (_activeSlot != value)
            {
                _activeSlot = value;
                OnInventoryUpdate.Invoke();
            }
        }
    }
    public int activeBlockId 
    { 
        get => activeSlot < 0 ? -1 : activeSlots[activeSlot];
        set
        {
            int newSlot = System.Array.IndexOf(activeSlots, value);
            if (newSlot < 0)
            {
                newSlot = System.Array.IndexOf(activeSlots, -1);
                if (newSlot < 0)
                    newSlot = activeSlots.Length - 1;
                activeSlots[newSlot] = value;
            }
            activeSlot = newSlot;
        }
    }

    public int[] GetBlocksIDs() => blocks.Keys.ToArray();
    public int[] GetActiveBlocksIDs() => (int[])activeSlots.Clone();
    public int GetBlocksCount(int id)
    {
        if (blocks.TryGetValue(id, out int count))
            return count;
        return 0;
    }

    public void GetSlotInfo(int index, out BlockInfo blockInfo, out int count)
    {
        int id = activeSlots[index];
        if (id < 0)
        {
            blockInfo = null;
            count = 0;
        }
        else
        {
            blockInfo = BlockDatabase.current[id];
            count = blocks[id];
        }
    }

    public void GetActiveSlotInfo(out BlockInfo blockInfo, out int count) => GetSlotInfo(activeSlot, out blockInfo, out count);

    public bool UseActiveBlock()
    {
        if (activeSlot < 0)
            return false;
        int id = activeSlots[activeSlot];
        if (!blocks.ContainsKey(id) || blocks[id] <= 0)
            return false;
        blocks[id]--;
        if (blocks[id] == 0)
        {
            blocks.Remove(id);
            activeSlots[activeSlot] = -1;
            activeSlot = -1;
        }
        OnInventoryUpdate.Invoke();
        return true;
    }

    public void SetInventory(int slotCount, params (int, int)[] blocks)
    {
        this.blocks.Clear();
        foreach (var (blockId, count) in blocks)
            this.blocks.Add(blockId, count);
        this.slotCount = slotCount;
        activeSlot = 0;
        OnInventoryUpdate.Invoke();
    }

    public void AddBlocks(int blockId, int count)
    {
        if (!blocks.ContainsKey(blockId))
        {
            blocks.Add(blockId, count);
            int freeSlot = System.Array.FindIndex(activeSlots, slot => slot < 0);
            if (freeSlot >= 0)
                activeSlots[freeSlot] = blockId;
        }
        else
            blocks[blockId] += count;
        OnInventoryUpdate.Invoke();
    }
}

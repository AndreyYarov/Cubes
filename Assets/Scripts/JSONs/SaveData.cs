using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int width, height, depth;
    public List<BlockData> blocks = new List<BlockData>();
    public int[] inventoryIDs, inventoryCounts;
}

public static class BlockFactory
{
    public static Block Create(int blockId, Point min, Point max)
    {
        if (min <= max)
            return BlockPool.Create(min, max, blockId);
        return null;
    }

    public static Block Create(string blockName, Point min, Point max)
    {
        if (min <= max)
        {
            int blockId = BlockDatabase.current.GetBlockIndex(blockName);
            return BlockPool.Create(min, max, blockId);
        }
        return null;
    }
}

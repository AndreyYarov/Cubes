using System.Collections.Generic;
using UnityEngine;

public static class BlockPool 
{
    private static Stack<Block> pool = new Stack<Block>();
    private static Transform _activeParent, _inactiveParent;

    private static Transform activeParent
    {
        get
        {
            if (!_activeParent)
            {
                GameObject go = GameObject.Find("Blocks-Active");
                if (!go)
                    go = new GameObject("Blocks-Active");
                _activeParent = go.transform;
            }
            return _activeParent;
        }
    }

    private static Transform inactiveParent
    {
        get
        {
            if (!_inactiveParent)
            {
                GameObject go = GameObject.Find("Blocks-Inactive");
                if (!go)
                    go = new GameObject("Blocks-Inactive");
                go.SetActive(false);
                _inactiveParent = go.transform;
            }
            return _inactiveParent;
        }
    }

    public static void Push(Block block)
    {
        pool.Push(block);
        block.transform.parent = inactiveParent;
    }

    public static Block Create(Point min, Point max, int blockId)
    {
        Block block;
        if (pool.Count == 0)
        {
            GameObject go = new GameObject();
            block = go.AddComponent<Block>();
        }
        else
            block = pool.Pop();
        block.transform.parent = activeParent;
        block.Init(min, max, blockId);
        return block;
    }

    public static void Clear()
    {
        while (pool.Count > 0)
            Object.DestroyImmediate(pool.Pop().gameObject);
    }
}

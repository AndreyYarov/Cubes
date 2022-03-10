using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Block Database")]
public class BlockDatabase : ScriptableObject
{
    private static BlockDatabase _current;
    public static BlockDatabase current
    {
        get
        {
            if (!_current)
                _current = Resources.Load<BlockDatabase>("Block-Database");
            return _current;
        }
    }

    public static string[] GetBlockNames()
    {
        string[] names = new string[current.Count];
        for (int i = 0; i < current.Count; i++)
            names[i] = current[i].name;
        return names;
    }

    [SerializeField] private List<BlockInfo> m_Blocks = new List<BlockInfo>();
    public BlockInfo this[int index] => m_Blocks[index];
    public BlockInfo this[string name] => m_Blocks.Find(blockInfo => blockInfo.name == name);
    public int Count => m_Blocks.Count;
    public int GetBlockIndex(string name) => m_Blocks.FindIndex(blockInfo => blockInfo.name == name);
}

﻿using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class Block : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private BoxCollider _boxCollider;
    [SerializeField] private Point m_Min, m_Max;
    [SerializeField] private int m_BlockId;

    public MeshFilter meshFilter
    {
        get
        {
            if (!_meshFilter)
                _meshFilter = GetComponent<MeshFilter>();
            return _meshFilter;
        }
    }
    public MeshRenderer meshRenderer
    {
        get
        {
            if (!_meshRenderer)
                _meshRenderer = GetComponent<MeshRenderer>();
            return _meshRenderer;
        }
    }
    public BoxCollider boxCollider
    {
        get
        {
            if (!_boxCollider)
                _boxCollider = GetComponent<BoxCollider>();
            return _boxCollider;
        }
    }
    public Point min => m_Min;
    public Point max => m_Max;
    public int blockId => m_BlockId;

    public void Init(Mesh mesh, Point min, Point max, int blockId)
    {
        m_Min = min;
        m_Max = max;
        m_BlockId = blockId;

        BlockInfo blockInfo = BlockDatabase.current[blockId];
        Point size = max - min + new Point(1, 1, 1);

        transform.position = (Vector3)(min + max) * 0.5f;
        name = $"{blockInfo.name} {size.x}x{size.y}x{size.z}";
        meshFilter.mesh = mesh;
        meshRenderer.materials = new Material[] { blockInfo.top, blockInfo.side, blockInfo.bottom };
        boxCollider.size = size;
    }

    public void Cut(Point pos)
    {
        Point min = this.min, max = this.max;
        if (pos >= min && pos <= max)
        {
            BlockPool.Push(this);

            BlockFactory.Create(blockId, min, new Point(pos.x - 1, max.y, max.z));
            BlockFactory.Create(blockId, new Point(pos.x + 1, min.y, min.z), max);
            BlockFactory.Create(blockId, new Point(pos.x, min.y, min.z), new Point(pos.x, pos.y - 1, max.z));
            BlockFactory.Create(blockId, new Point(pos.x, pos.y + 1, min.z), new Point(pos.x, max.y, max.z));
            BlockFactory.Create(blockId, new Point(pos.x, pos.y, min.z), new Point(pos.x, pos.y, pos.z - 1));
            BlockFactory.Create(blockId, new Point(pos.x, pos.y, pos.z + 1), new Point(pos.x, pos.y, max.z));
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Block : MonoBehaviour
{
    private BoxCollider _boxCollider;
    private Point _min, _max;
    private int _blockId;
    public BoxCollider boxCollider
    {
        get
        {
            if (!_boxCollider)
                _boxCollider = GetComponent<BoxCollider>();
            return _boxCollider;
        }
    }
    public Point min => _min;
    public Point max => _max;
    public int blockId => _blockId;

    private int vertsStartId;

    public void Init(Point min, Point max, int blockId)
    {
        _min = min;
        _max = max;
        _blockId = blockId;

        BlockInfo blockInfo = BlockDatabase.current[blockId];
        Point size = max - min + new Point(1, 1, 1);

        transform.position = (Vector3)(min + max) * 0.5f;
        name = $"{blockInfo.name} {size.x}x{size.y}x{size.z}";
        boxCollider.size = size;
        BlockMeshController.GetController(blockId).AddBlock(min, max, out vertsStartId);
    }

    public void Cut(Point pos)
    {
        Point min = this.min, max = this.max;
        if (pos >= min && pos <= max)
        {
            BlockPool.Push(this);
            BlockMeshController.GetController(blockId).RemoveBlock(vertsStartId);

            BlockFactory.Create(blockId, min, new Point(pos.x - 1, max.y, max.z));
            BlockFactory.Create(blockId, new Point(pos.x + 1, min.y, min.z), max);
            BlockFactory.Create(blockId, new Point(pos.x, min.y, min.z), new Point(pos.x, pos.y - 1, max.z));
            BlockFactory.Create(blockId, new Point(pos.x, pos.y + 1, min.z), new Point(pos.x, max.y, max.z));
            BlockFactory.Create(blockId, new Point(pos.x, pos.y, min.z), new Point(pos.x, pos.y, pos.z - 1));
            BlockFactory.Create(blockId, new Point(pos.x, pos.y, pos.z + 1), new Point(pos.x, pos.y, max.z));
        }
    }
}

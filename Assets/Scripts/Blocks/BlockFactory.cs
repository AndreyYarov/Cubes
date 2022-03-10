using System.Collections.Generic;
using UnityEngine;

public static class BlockFactory
{
    private static Dictionary<int, Mesh> meshes = new Dictionary<int, Mesh>();

    private static Mesh GetMesh(Point size)
    {
        int sizeKey = (size.x & 0x7ff) | ((size.y & 0x3ff) << 11) | ((size.z & 0x7ff) << 21); //11 бит - x, 10 бит - y, 11 бит - z

        if (!meshes.TryGetValue(sizeKey, out Mesh mesh))
        {
            Vector3 ext = (Vector3)size * 0.5f;

            Vector3[] verts = new Vector3[]
            {
                    new Vector3(-ext.x, ext.y, -ext.z), new Vector3(-ext.x, ext.y, ext.z), new Vector3(ext.x, ext.y, ext.z), new Vector3(ext.x, ext.y, -ext.z),
                    new Vector3(-ext.x, -ext.y, -ext.z), new Vector3(-ext.x, ext.y, -ext.z), new Vector3(ext.x, ext.y, -ext.z), new Vector3(ext.x, -ext.y, -ext.z),
                    new Vector3(ext.x, -ext.y, -ext.z), new Vector3(ext.x, ext.y, -ext.z), new Vector3(ext.x, ext.y, ext.z), new Vector3(ext.x, -ext.y, ext.z),
                    new Vector3(ext.x, -ext.y, ext.z), new Vector3(ext.x, ext.y, ext.z), new Vector3(-ext.x, ext.y, ext.z), new Vector3(-ext.x, -ext.y, ext.z),
                    new Vector3(-ext.x, -ext.y, ext.z), new Vector3(-ext.x, ext.y, ext.z), new Vector3(-ext.x, ext.y, -ext.z), new Vector3(-ext.x, -ext.y, -ext.z),
                    new Vector3(-ext.x, -ext.y, -ext.z), new Vector3(ext.x, -ext.y, -ext.z), new Vector3(ext.x, -ext.y, ext.z), new Vector3(-ext.x, -ext.y, ext.z)
            };

            Vector2[] uv = new Vector2[]
            {
                    new Vector2(0f, 0f), new Vector2(0f, size.z), new Vector2(size.x, size.z), new Vector2(size.x, 0f),
                    new Vector2(0f, 0f), new Vector2(0f, size.y), new Vector2(size.x, size.y), new Vector2(size.x, 0f),
                    new Vector2(0f, 0f), new Vector2(0f, size.y), new Vector2(size.z, size.y), new Vector2(size.z, 0f),
                    new Vector2(0f, 0f), new Vector2(0f, size.y), new Vector2(size.x, size.y), new Vector2(size.x, 0f),
                    new Vector2(0f, 0f), new Vector2(0f, size.y), new Vector2(size.z, size.y), new Vector2(size.z, 0f),
                    new Vector2(0f, 0f), new Vector2(0f, size.x), new Vector2(size.z, size.x), new Vector2(size.z, 0f)
            };

            Vector3[] normals = new Vector3[]
            {
                    Vector3.down, Vector3.down, Vector3.down, Vector3.down,
                    Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
                    Vector3.left, Vector3.left, Vector3.left, Vector3.left,
                    Vector3.back, Vector3.back, Vector3.back, Vector3.back,
                    Vector3.right, Vector3.right, Vector3.right, Vector3.right,
                    Vector3.up, Vector3.up, Vector3.up, Vector3.up
            };

            int[] up = new int[]
            {
                    0, 1, 2,
                    2, 3, 0
            };

            int[] sides = new int[]
            {
                    4, 5, 6,
                    6, 7, 4,
                    8, 9, 10,
                    10, 11, 8,
                    12, 13, 14,
                    14, 15, 12,
                    16, 17, 18,
                    18, 19, 16
            };

            int[] down = new int[]
            {
                    20, 21, 22,
                    22, 23, 20
            };

            mesh = new Mesh();
            mesh.name = $"Block {size.x}x{size.y}x{size.z}";
            mesh.subMeshCount = 3;
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uv);
            mesh.SetNormals(normals);
            mesh.SetTriangles(up, 0, false);
            mesh.SetTriangles(sides, 1, false);
            mesh.SetTriangles(down, 2, false);
            mesh.RecalculateBounds();

            meshes.Add(sizeKey, mesh);
        }

        return mesh;
    }

    public static Block Create(int blockId, Point min, Point max)
    {
        if (min <= max)
        {
            Point size = max - min + new Point(1, 1, 1);
            Mesh mesh = GetMesh(size);
            return BlockPool.Create(min, max, mesh, blockId);
        }
        return null;
    }

    public static Block Create(string blockName, Point min, Point max)
    {
        if (min <= max)
        {
            Point size = max - min + new Point(1, 1, 1);
            Mesh mesh = GetMesh(size);
            int blockId = BlockDatabase.current.GetBlockIndex(blockName);
            return BlockPool.Create(min, max, mesh, blockId);
        }
        return null;
    }
}

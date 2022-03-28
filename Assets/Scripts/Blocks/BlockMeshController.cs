using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BlockMeshController : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Stack<int> freeVerts = new Stack<int>();
    private List<Vector3> verts = new List<Vector3>(), normals = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> upSubmesh = new List<int>(), sideSubmesh = new List<int>(), downSubmesh = new List<int>();
    private bool vertsChanged = false, normalsChanged = false, trisChanged = false;

    private static Dictionary<int, BlockMeshController> controllers = new Dictionary<int, BlockMeshController>();
    private static Transform parent;

    public static void Clear() => controllers.Clear();

    public static BlockMeshController GetController(string blockName)
    {
        int blockId = BlockDatabase.current.GetBlockIndex(blockName);
        return GetController(blockId);
    }

    public static BlockMeshController GetController(int blockId)
    {
        if (controllers.TryGetValue(blockId, out var controller))
            return controller;

        if (!parent)
        {
            GameObject go = GameObject.Find("Block-Renderers");
            if (!go)
                go = new GameObject("Block-Renderers");
            parent = go.transform;
        }

        controller = new GameObject().AddComponent<BlockMeshController>();
        controller.transform.parent = parent;
        controller.Init(blockId);
        controllers.Add(blockId, controller);
        return controller;
    }

    private void Init(int blockId)
    {
        BlockInfo blockInfo = BlockDatabase.current[blockId];
        name = blockInfo.name;

        mesh = new Mesh();
        mesh.name = blockInfo.name + "_mesh";
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.subMeshCount = 3;

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = new Material[] { blockInfo.top, blockInfo.side, blockInfo.bottom };
    }

    public void AddBlock(Point min, Point max, out int vertsStartId)
    {
        Point size = max - min + new Point(1, 1, 1);
        Vector3 p1 = min - Vector3.one * 0.5f;
        Vector3 p2 = max + Vector3.one * 0.5f;

        bool add = freeVerts.Count == 0;
        vertsStartId = add ? verts.Count : freeVerts.Pop();

        Vector3[] newVerts = new Vector3[]
        {
            new Vector3(p1.x, p2.y, p1.z), new Vector3(p1.x, p2.y, p2.z), new Vector3(p2.x, p2.y, p2.z), new Vector3(p2.x, p2.y, p1.z),
            new Vector3(p1.x, p1.y, p1.z), new Vector3(p1.x, p2.y, p1.z), new Vector3(p2.x, p2.y, p1.z), new Vector3(p2.x, p1.y, p1.z),
            new Vector3(p2.x, p1.y, p1.z), new Vector3(p2.x, p2.y, p1.z), new Vector3(p2.x, p2.y, p2.z), new Vector3(p2.x, p1.y, p2.z),
            new Vector3(p2.x, p1.y, p2.z), new Vector3(p2.x, p2.y, p2.z), new Vector3(p1.x, p2.y, p2.z), new Vector3(p1.x, p1.y, p2.z),
            new Vector3(p1.x, p1.y, p2.z), new Vector3(p1.x, p2.y, p2.z), new Vector3(p1.x, p2.y, p1.z), new Vector3(p1.x, p1.y, p1.z),
            new Vector3(p1.x, p1.y, p1.z), new Vector3(p2.x, p1.y, p1.z), new Vector3(p2.x, p1.y, p2.z), new Vector3(p1.x, p1.y, p2.z)
        };
        Vector2[] newUVs = new Vector2[]
        {
            new Vector2(0f, 0f), new Vector2(0f, size.z), new Vector2(size.x, size.z), new Vector2(size.x, 0f),
            new Vector2(0f, 0f), new Vector2(0f, size.y), new Vector2(size.x, size.y), new Vector2(size.x, 0f),
            new Vector2(0f, 0f), new Vector2(0f, size.y), new Vector2(size.z, size.y), new Vector2(size.z, 0f),
            new Vector2(0f, 0f), new Vector2(0f, size.y), new Vector2(size.x, size.y), new Vector2(size.x, 0f),
            new Vector2(0f, 0f), new Vector2(0f, size.y), new Vector2(size.z, size.y), new Vector2(size.z, 0f),
            new Vector2(0f, 0f), new Vector2(0f, size.x), new Vector2(size.z, size.x), new Vector2(size.z, 0f)
        };

        if (add)
        {
            verts.AddRange(newVerts);
            uvs.AddRange(newUVs);
            Vector3[] newNormals = new Vector3[]
            {
                Vector3.up, Vector3.up, Vector3.up, Vector3.up,
                Vector3.back, Vector3.back, Vector3.back, Vector3.back,
                Vector3.right, Vector3.right, Vector3.right, Vector3.right,
                Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
                Vector3.left, Vector3.left, Vector3.left, Vector3.left,
                Vector3.down, Vector3.down, Vector3.down, Vector3.down,
            };
            normals.AddRange(newNormals);

            normalsChanged = true;
        }
        else
        {
            verts.Replace(vertsStartId, newVerts);
            uvs.Replace(vertsStartId, newUVs);
        }

        int[] newUp = new int[]
        {
            vertsStartId, vertsStartId + 1, vertsStartId + 2,
            vertsStartId + 2, vertsStartId + 3, vertsStartId + 0
        };

        int[] newSides = new int[]
        {
            vertsStartId + 4, vertsStartId + 5, vertsStartId + 6,
            vertsStartId + 6, vertsStartId + 7, vertsStartId + 4,
            vertsStartId + 8, vertsStartId + 9, vertsStartId + 10,
            vertsStartId + 10, vertsStartId + 11, vertsStartId + 8,
            vertsStartId + 12, vertsStartId + 13, vertsStartId + 14,
            vertsStartId + 14, vertsStartId + 15, vertsStartId + 12,
            vertsStartId + 16, vertsStartId + 17, vertsStartId + 18,
            vertsStartId + 18, vertsStartId + 19, vertsStartId + 16
        };

        int[] newDown = new int[]
        {
            vertsStartId + 20, vertsStartId + 21, vertsStartId + 22,
            vertsStartId + 22, vertsStartId + 23, vertsStartId + 20
        };

        upSubmesh.AddRange(newUp);
        sideSubmesh.AddRange(newSides);
        downSubmesh.AddRange(newDown);

        vertsChanged = trisChanged = true;
    }

    public void RemoveBlock(int vertsStartId)
    {
        freeVerts.Push(vertsStartId);
        int id = upSubmesh.IndexOf(vertsStartId);

        upSubmesh.RemoveRange(id, 6);
        sideSubmesh.RemoveRange(id * 4, 24);
        downSubmesh.RemoveRange(id, 6);

        trisChanged = true;
    }

    private void LateUpdate()
    {
        if (vertsChanged)
        {
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
        }

        if (normalsChanged)
            mesh.SetNormals(normals);

        if (trisChanged)
        {
            mesh.SetTriangles(upSubmesh, 0);
            mesh.SetTriangles(sideSubmesh, 1);
            mesh.SetTriangles(downSubmesh, 2);
            mesh.RecalculateBounds();
        }

        if (vertsChanged || normalsChanged || trisChanged)
            meshFilter.mesh = mesh;

        vertsChanged = normalsChanged = trisChanged = false;
    }
}

using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;

using DateTime = System.DateTime;

public class World : MonoBehaviour
{
    [SerializeField, Range(1, 2048)] private int width = 256;
    [SerializeField, Range(1, 1024)] private int height = 128;
    [SerializeField, Range(1, 2048)] private int depth = 256;
    [SerializeField, Range(0f, 1f)] private float relief = 0.33f;
    [SerializeField, Range(4, 128)] private int reliefStep = 16;
    [SerializeField, Range(15, 50)] private int maxLoadFrameDuration = 33;

    private bool ready = false;
    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        player.gameObject.SetActive(false);
    }

    private void DestroyWorld()
    {
        foreach (var block in FindObjectsOfType<Block>())
            DestroyImmediate(block.gameObject);
        BlockPool.Clear();
        foreach (var blockMeshController in FindObjectsOfType<BlockMeshController>())
            DestroyImmediate(blockMeshController.gameObject);
        BlockMeshController.Clear();
    }

    public void GenerateWorld() => StartCoroutine(GenerateWorldAsync());

    private IEnumerator GenerateWorldAsync()
    {
        Menu.Current.Hide();
        Tutorial.Show();
        player.gameObject.SetActive(false);
        Time.timeScale = 0f;

        if (ready)
        {
            yield return null;
            ready = false;
            DestroyWorld();
        }
        yield return null;

        transform.position = new Vector3((width - 1) * 0.5f, (height - 1) * 0.5f, (depth - 1) * 0.5f);
        transform.localScale = new Vector3(width, height, depth);
        int stoneHeight = height / 5;
        float reliefHeight = height * relief * 0.25f;
        float reliefCenter = height * 0.5f;

        BlockFactory.Create("Lava", new Point(0, 0, 0), new Point(width - 1, 0, depth - 1));
        BlockFactory.Create("Stone", new Point(0, 1, 0), new Point(width - 1, stoneHeight, depth - 1));

        int[,] y = new int[width, depth];

        int hx = width / reliefStep + 2;
        int hz = depth / reliefStep + 2;
        float[,] hh = new float[hx, hz];
        for (int i = 0; i < hx; i++)
            for (int j = 0; j < hz; j++)
                hh[i, j] = Random.Range(-reliefHeight, reliefHeight);

        yield return null;
        DateTime last = DateTime.Now;

        for (int x = 0; x < width; x++)
        {
            var now = DateTime.Now;
            if ((now - last).Milliseconds >= maxLoadFrameDuration)
            {
                last = now;
                yield return null;
            }

            int tx1 = x / reliefStep, tx2 = tx1 + 1;
            float xb1 = (float)x / reliefStep - tx1, xb2 = 1f - xb1;
            for (int z = 0; z < depth; z++)
            {
                int tz1 = z / reliefStep, tz2 = tz1 + 1;
                float zb1 = (float)z / reliefStep - tz1, zb2 = 1f - zb1;

                y[x, z] = Mathf.RoundToInt(
                    Mathf.Clamp01(1f - Mathf.Sqrt(xb1 * xb1 + zb1 * zb1)) * hh[tx1, tz1] +
                    Mathf.Clamp01(1f - Mathf.Sqrt(xb1 * xb1 + zb2 * zb2)) * hh[tx1, tz2] +
                    Mathf.Clamp01(1f - Mathf.Sqrt(xb2 * xb2 + zb1 * zb1)) * hh[tx2, tz1] +
                    Mathf.Clamp01(1f - Mathf.Sqrt(xb2 * xb2 + zb2 * zb2)) * hh[tx2, tz2] +
                    reliefCenter
                );
            }
        }

        for (int x = 0; x < width; x++)
        {
            var now = DateTime.Now;
            if ((now - last).Milliseconds >= maxLoadFrameDuration)
            {
                last = now;
                yield return null;
            }

            for (int z = 0; z < depth; z++)
            {
                if (y[x, z] < 0)
                    continue;

                int h = y[x, z];

                int xx = x + 1;
                while (xx < width && y[xx, z] == h)
                    xx++;
                xx--;

                int zz = z;
                bool f = true;
                while (f)
                {
                    zz++;
                    if (zz == depth)
                        f = false;
                    else
                        for (int dx = x; dx <= xx; dx++)
                            if (y[dx, zz] != h)
                            {
                                f = false;
                                break;
                            }
                }
                zz--;

                for (int dx = x; dx <= xx; dx++)
                    for (int dz = z; dz <= zz; dz++)
                        y[dx, dz] = -1;

                BlockFactory.Create("Ground", new Point(x, stoneHeight + 1, z), new Point(xx, h - 1, zz));
                BlockFactory.Create("Grass", new Point(x, h, z), new Point(xx, h, zz));
            }
        }

        player.Init(GetSpawnPoint(width, height, depth));
        ready = true;
        Time.timeScale = 1f;
        Tutorial.Hide();
        GameplayUI.Show();
    }

    public void Load(string fileName) => StartCoroutine(LoadAsync(fileName));

    private IEnumerator LoadAsync(string fileName)
    {
        Menu.Current.Hide();
        Tutorial.Show();
        player.gameObject.SetActive(false);
        Time.timeScale = 0f;

        if (ready)
        {
            yield return null;
            ready = false;
            DestroyWorld();
        }
        yield return null;

        string path = Application.persistentDataPath + fileName;
        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (var blockInfo in data.blocks)
            BlockFactory.Create(blockInfo.blockId, blockInfo.min, blockInfo.max);

        Inventory inventory = player.GetComponent<MinerController>().inventory;
        (int, int)[] invBlocks = new (int, int)[data.inventoryIDs.Length];
        for (int i = 0; i < invBlocks.Length; i++)
            invBlocks[i] = (data.inventoryIDs[i], data.inventoryCounts[i]);
        inventory.SetInventory(inventory.slotCount, invBlocks);

        player.Init(GetSpawnPoint(data.width, data.height, data.depth));
        ready = true;
        Time.timeScale = 1f;
        Tutorial.Hide();
        GameplayUI.Show();
    }

    public void Save(string fileName)
    {
        Block[] allBlocks = FindObjectsOfType<Block>();
        SaveData data = new SaveData { width = this.width, height = this.height, depth = this.depth };

        foreach (Block block in allBlocks)
            data.blocks.Add(new BlockData { min = block.min, max = block.max, blockId = block.blockId });

        Inventory inventory = player.GetComponent<MinerController>().inventory;
        data.inventoryIDs = inventory.GetBlocksIDs();
        data.inventoryCounts = data.inventoryIDs.Select(id => inventory.GetBlocksCount(id)).ToArray();

        string path = Application.persistentDataPath + fileName;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);

        Menu.Current.Hide();
    }

    public Vector3 GetSpawnPoint(int width, int height, int depth) =>
        new Vector3((width - 1) * 0.5f, height + 1f, (depth - 1) * 0.5f);

    private static World _current;
    public static World Current
    {
        get
        {
            if (!_current)
                _current = FindObjectOfType<World>();
            return _current;
        }
    }
    public static bool Ready => Current.ready;
}

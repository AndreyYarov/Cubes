using System.Collections;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField, Range(1, 2048)] private int width = 256;
    [SerializeField, Range(1, 1024)] private int height = 128;
    [SerializeField, Range(1, 2048)] private int depth = 256;
    [SerializeField, Range(0f, 1f)] private float relief = 0.33f;
    [SerializeField, Range(4, 128)] private int reliefStep = 16;

    private bool ready = false;

    private void Start()
    {
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

        for (int x = 0; x < width; x++)
        {
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
        ready = true;
    }

    public IEnumerator WaitForReady()
    {
        while (!ready)
            yield return null;
    }

    public Vector3 GetSpawnPoint() =>
        new Vector3((width - 1) * 0.5f, height + 1f, (depth - 1) * 0.5f);
}

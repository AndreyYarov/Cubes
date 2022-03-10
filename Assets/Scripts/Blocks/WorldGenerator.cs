using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField, Range(1, 2048)] private int width = 256;
    [SerializeField, Range(1, 1024)] private int height = 128;
    [SerializeField, Range(1, 2048)] private int depth = 256;
    [SerializeField, Range(0f, 1f)] private float relief = 0.33f;

    private void Start()
    {
        float reliefHeight = height * relief * 0.25f;
        int reliefCenter = height / 2;
        int stoneHeight = height / 5;
        float kx = Random.Range(0.01f, 0.1f);
        float kz = Random.Range(0.01f, 0.1f);
        int px = Random.Range(0, width);
        int pz = Random.Range(0, depth);

        BlockFactory.Create("Lava", new Point(0, 0, 0), new Point(width - 1, 0, depth - 1));
        BlockFactory.Create("Stone", new Point(0, 1, 0), new Point(width - 1, stoneHeight, depth - 1));

        int[,] y = new int[width, depth];

        for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
            {
                float dx = (width * 0.5f - x) / width * 1.4f;
                float dz = (depth * 0.5f - z) / depth * 1.4f;
                float bh = reliefHeight * (0.5f + Mathf.Sqrt(dx * dx + dz * dz));
                y[x, z] = Mathf.RoundToInt(Mathf.Sin(kx * (x + px)) * Mathf.Sin(kz * (z + pz)) * bh) + reliefCenter;
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
    }
}

using UnityEngine;

public static class CrackController
{
    private static Point lastHit = new Point(-1, -1, -1);
    private static int lastStrength;
    private static GameObject crackLittle, crackBig;

    public static void Init()
    {
        crackLittle = Object.Instantiate(Resources.Load<GameObject>("Crack-Little"));
        crackLittle.SetActive(false);
        crackBig = Object.Instantiate(Resources.Load<GameObject>("Crack-Big"));
        crackBig.SetActive(false);
    }

    private static void Destroy(Point point, Block block)
    {
        block.Cut(point);
        lastHit = new Point(-1, -1, -1);
        crackLittle.SetActive(false);
        crackBig.SetActive(false);
    }

    public static void DoHit(Point point, Block block)
    {
        if (point == lastHit)
        {
            lastStrength--;
            if (lastStrength <= 0)
                Destroy(point, block);
            else if (lastStrength < (BlockDatabase.current[block.blockId].strenght + 1) / 2)
                crackBig.SetActive(true);
        }
        else
        {
            lastStrength = BlockDatabase.current[block.blockId].strenght - 1;
            if (lastStrength <= 0)
                Destroy(point, block);
            else
            {
                lastHit = point;
                crackLittle.transform.position = crackBig.transform.position = point;
                crackLittle.SetActive(true);
                crackBig.SetActive(false);
            }
        }
    }
}

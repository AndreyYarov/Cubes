using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerController))]
public class MinerController : MonoBehaviour
{
    [SerializeField] private Transform m_Axe;
    [SerializeField] private Vector3 m_AxeTargetRotation;
    [SerializeField] private float m_HitAnimationDuration = 0.5f;
    [SerializeField] private float m_PlaceAnimationDuration = 0.3f;
    [SerializeField] private float m_MineDistance = 5f;

    private Inventory _inventory;
    private GameObject targetInstance;

    public Inventory inventory
    {
        get
        {
            if (_inventory == null)
            {
                _inventory = new Inventory();
                _inventory.SetInventory(5, Enumerable.Range(0, 16).Select(i => (i, 15)).ToArray());
            }
            return _inventory;
        }
    }

    private IEnumerator Start()
    {
        targetInstance = Instantiate(Resources.Load<GameObject>("Target-Cube"));
        targetInstance.SetActive(false);
        CrackController.Init();

        yield return null;

        while (true)
        {
            if (!MouseOverUI())
            {
                if (Input.GetMouseButton(0))
                    yield return Hit();
                else if (Input.GetMouseButton(1))
                    yield return Place();
            }
            yield return null;
        }
    }

    private void Update()
    {
        if (GetHitPoint(out Point point))
        {
            targetInstance.SetActive(true);
            targetInstance.transform.position = point;
        }
        else
            targetInstance.SetActive(false);
    }

    private bool GetHitPoint(out Point point, out Vector3 hitPosition, out Block block)
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hitInfo, m_MineDistance))
        {
            block = hitInfo.collider.GetComponent<Block>();
            if (block)
            {
                hitPosition = hitInfo.point;
                point = Point.Clamp((Point)hitInfo.point, block.min, block.max);
                return true;
            }
        }
        hitPosition = Vector3.zero;
        point = new Point();
        block = null;
        return false;
    }
    private bool GetHitPoint(out Point point) => GetHitPoint(out point, out _, out _);
    private bool GetHitPoint(out Point point, out Vector3 hitPosition) => GetHitPoint(out point, out hitPosition, out _);
    private bool GetHitPoint(out Point point, out Block block) => GetHitPoint(out point, out _, out block);
    private int Sign(float value) => value > 0f ? 1 : value < 0f ? -1 : 0;

    private bool MouseOverUI()
    {
        if (!EventSystem.current)
            return false;
        var pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        foreach (RaycastResult result in results)
            if (result.gameObject.layer == 5)
                return true;
        return false;
    }
    private IEnumerator Hit()
    {
        m_Axe.gameObject.SetActive(true);
        Quaternion start = m_Axe.localRotation;
        Quaternion target = Quaternion.Euler(m_AxeTargetRotation);

        float delay = m_HitAnimationDuration * 0.5f;
        float t = 0f;

        while (t < delay)
        {
            yield return null;
            t = Mathf.Min(t + Time.deltaTime, delay);
            m_Axe.localRotation = Quaternion.Slerp(start, target, t / delay);
        }

        if (GetHitPoint(out Point point, out Block block))
            CrackController.DoHit(point, block);

        while (t > 0f)
        {
            yield return null;
            t = Mathf.Max(t - Time.deltaTime, 0f);
            m_Axe.localRotation = Quaternion.Slerp(start, target, t / delay);
        }
    }
    private IEnumerator Place()
    {
        if (GetHitPoint(out Point point, out Vector3 hitPosition))
        {
            if (inventory.activeBlockId < 0)
                yield break;

            Vector3 delta = hitPosition - point;
            Vector3 absDelta = new Vector3(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));
            if (absDelta.x > absDelta.y && absDelta.x > absDelta.z)
                point.x += Sign(delta.x);
            else if (absDelta.y > absDelta.z)
                point.y += Sign(delta.y);
            else
                point.z += Sign(delta.z);

            m_Axe.gameObject.SetActive(false);

            int blockId = inventory.activeBlockId;
            if (inventory.UseActiveBlock())
            {
                Block block = BlockFactory.Create(blockId, point, point);

                Vector3 start = hitPosition;
                Vector3 end = point;
                block.transform.position = start;
                block.transform.localScale = Vector3.zero;

                float t = 0f;
                while (t < 1f)
                {
                    yield return null;
                    t = Mathf.Min(t + Time.deltaTime / m_PlaceAnimationDuration, 1f);
                    block.transform.position = Vector3.Lerp(start, end, t);
                    block.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                }
            }
        }
    }
}

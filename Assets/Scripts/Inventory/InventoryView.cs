using UnityEngine;
using UnityEngine.Events;

public class InventoryView : BaseInventoryView<InventoryView, InventoryPresenter>
{
    [SerializeField] private RectTransform m_InventoryParent;
    [SerializeField] private GameObject m_Container;

    public override bool Visible 
    {
        get => m_Container ? m_Container.activeSelf : base.Visible;
        set
        {
            if (value != Visible)
            {
                m_Container?.SetActive(value);
                base.Visible = value;
            }
        }
    }

    public void Init(int[] ids, int[] counts, int selectedId, UnityAction<int> OnSelect) => 
        Init(ids, counts, selectedId, m_InventoryParent, OnSelect);
}

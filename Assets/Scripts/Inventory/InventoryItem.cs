using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : Toggle
{
    [SerializeField] private Image m_Icon;
    [SerializeField] private TMP_Text m_Title;
    [SerializeField] private TMP_Text m_Counter;

    public void Init(Sprite sprite, string title, int count, UnityAction OnSelect)
    {      
        if (m_Icon) m_Icon.sprite = sprite;
        if (m_Title) m_Title.text = title;
        if (m_Counter) m_Counter.text = count > 0 ? "x" + count : "";

        onValueChanged.RemoveAllListeners();
        onValueChanged.AddListener(selected => { if (selected) OnSelect(); });
    }

    public void SetTitle(string title)
    {
        if (m_Title) m_Title.text = title;
    }
}

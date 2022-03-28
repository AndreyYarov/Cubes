using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject m_Container;
    [SerializeField] private Button m_Save1, m_Save2, m_Save3, m_Load1, m_Load2, m_Load3;

    private static Menu _current;
    public static Menu Current
    {
        get
        {
            if (!_current)
                _current = FindObjectOfType<Menu>();
            return _current;
        }
    }

    public void Show()
    {
        m_Container.SetActive(true);
        Awake();
    }
    public void Hide() => m_Container.SetActive(false);

    private void Awake()
    {
        m_Save1.enabled = m_Save2.enabled = m_Save3.enabled = World.Ready;
        m_Load1.enabled = File.Exists(Application.persistentDataPath + "save_1");
        m_Load2.enabled = File.Exists(Application.persistentDataPath + "save_2");
        m_Load3.enabled = File.Exists(Application.persistentDataPath + "save_3");
    }

    private void Update()
    {
        if (World.Ready && Time.timeScale == 1f && Input.GetKeyDown(KeyCode.Escape))
            if (m_Container.activeSelf)
                Hide();
            else
                Show();
    }
}

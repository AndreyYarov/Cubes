using System;
using UnityEngine;

[Serializable]
public class BlockInfo
{
    [SerializeField] private string m_Name;
    [SerializeField] private Material m_Top, m_Side, m_Bottom;
    [SerializeField] private Sprite m_Sprite;
    [SerializeField, Min(1)] private int m_Strenght;

    public string name => m_Name;
    public Material top => m_Top;
    public Material side => m_Side;
    public Material bottom => m_Bottom;
    public Sprite sprite => m_Sprite;
    public int strenght => m_Strenght;
}

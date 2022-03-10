using UnityEngine;
using UnityEditor;

public class BlockFactoryWindow : EditorWindow
{
    [MenuItem("Custom/Block factory")]
    public static void Init()
    {
        var window = GetWindow<BlockFactoryWindow>();
        window.titleContent = new GUIContent("Block factory");
        window.Show();
    }

    [SerializeField] private Point min = new Point(0, 0, 0), max = new Point(1, 1, 1), hole;
    private int blockId = 0;
    private Block block;

    private void OnGUI()
    {
        Selection.selectionChanged += Repaint;

        SerializedObject so = new SerializedObject(this);

        var MinProp = so.FindProperty("min");
        EditorGUILayout.PropertyField(MinProp);
        min = Point.Clamp(min, new Point(0, 0, 0), new Point(2047, 1023, 2047));

        var MaxProp = so.FindProperty("max");
        EditorGUILayout.PropertyField(MaxProp);
        max = Point.Clamp(max, min, new Point(2047, 1023, 2047));

        blockId = EditorGUILayout.Popup("Block", blockId, BlockDatabase.GetBlockNames());

        if (GUILayout.Button("Create"))
        {
            block = BlockFactory.Create(blockId, min, max);
            Selection.activeObject = block.gameObject;
        }

        EditorGUILayout.Space();
        block =
            Selection.gameObjects.Length != 1 || !Selection.activeGameObject.activeInHierarchy ? null :
            block && block.gameObject == Selection.activeGameObject ? block : 
            Selection.activeGameObject.GetComponent<Block>();
        if (block)
        {
            var HoleProp = so.FindProperty("hole");
            EditorGUILayout.PropertyField(HoleProp);
            hole = Point.Clamp(hole, block.min, block.max);

            if (GUILayout.Button("Cut"))
            {
                block.Cut(hole);
                Selection.activeGameObject = null;
            }
        }

        so.ApplyModifiedProperties();
    }
}

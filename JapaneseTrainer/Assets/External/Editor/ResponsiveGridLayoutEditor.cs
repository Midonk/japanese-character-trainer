using UnityEditor;
using GridLayout = ResponsiveGridLayout.GridLayout;

[CustomEditor(typeof(ResponsiveGridLayout)), CanEditMultipleObjects]
public class ResponsiveGridLayoutEditor : Editor 
{
    #region Properties

    private bool IsAutoLayout => _fitTypeValue == GridLayout.Uniform || _fitTypeValue == GridLayout.MatchWidth || _fitTypeValue == GridLayout.MatchHeight;
    private bool CanEditRows => _fitTypeValue == GridLayout.FixedRows || _fitTypeValue == GridLayout.FixedRowsAndColumns;
    private bool CanEditColumns => _fitTypeValue == GridLayout.FixedColumns || _fitTypeValue == GridLayout.FixedRowsAndColumns;
    private bool CanEditCellSize => !_autoCellWidth.boolValue || !_autoCellHeight.boolValue;

    #endregion

    
    #region Unity API

    private void OnEnable() 
    {
        _fitType = serializedObject.FindProperty("_fitType");
        _padding = serializedObject.FindProperty("m_Padding");
        _rows = serializedObject.FindProperty("_rows");
        _columns = serializedObject.FindProperty("_columns");
        _cellSize = serializedObject.FindProperty("_cellSize");
        _gap = serializedObject.FindProperty("_gap");
        _autoCellHeight = serializedObject.FindProperty("_autoCellHeight");
        _autoCellWidth = serializedObject.FindProperty("_autoCellWidth");
    }   

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        _fitTypeValue = (GridLayout)_fitType.enumValueIndex;
        EditorGUILayout.PropertyField(_padding);
        EditorGUILayout.PropertyField(_gap);
        EditorGUILayout.PropertyField(_fitType);
        using (new EditorGUI.DisabledScope(IsAutoLayout))
        {
            using (new EditorGUI.DisabledScope(!CanEditRows))
            {
                EditorGUILayout.PropertyField(_rows);
            }
            
            using (new EditorGUI.DisabledScope(!CanEditColumns))
            {
                EditorGUILayout.PropertyField(_columns);
            }
            
            using (new EditorGUI.DisabledScope(!CanEditCellSize))
            {
                EditorGUILayout.PropertyField(_cellSize);
            }
        }
        
        EditorGUILayout.PropertyField(_autoCellWidth);
        EditorGUILayout.PropertyField(_autoCellHeight);

        serializedObject.ApplyModifiedProperties();
    }
        
    #endregion

    
    #region Private Fields

    private SerializedProperty _fitType;
    private SerializedProperty _padding;
    private GridLayout _fitTypeValue;
    private SerializedProperty _rows;
    private SerializedProperty _columns;
    private SerializedProperty _cellSize;
    private SerializedProperty _gap;
    private SerializedProperty _autoCellHeight;
    private SerializedProperty _autoCellWidth;

    #endregion
}
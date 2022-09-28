using UnityEngine;
using UnityEngine.UI;

//Add a minmax to column and or row

public class ResponsiveGridLayout : LayoutGroup
{
    #region Exposed

    [SerializeField] private GridLayout _fitType;
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private Vector2 _cellSize;
    [SerializeField] private Vector2 _gap;
    [SerializeField] private bool _autoCellWidth = true;
    [SerializeField] private bool _autoCellHeight = true;
        
    #endregion


    #region Properties

    public int Rows 
    {
        get => _rows;
        set => _rows = (int)Mathf.Max(1, value);
    }
    
    public int Columns 
    {
        get => _columns;
        set => _columns = (int)Mathf.Max(1, value);
    }

    #endregion


    #region Unity API

    protected override void OnValidate() 
    {
        base.OnValidate();

        Rows = _rows;
        Columns = _columns;
    }
        
    #endregion


    #region Main

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        
        InitializeCellCount();

        Columns = DetermineColumnCount();
        if(!_autoCellWidth) return;

        _cellSize.x = AutoDetermineCellWidth();
    }

    public override void CalculateLayoutInputVertical()
    {
        Rows = DetermineRowCount();
        if(!_autoCellHeight) return;

        _cellSize.y = AutoDermineCellHeight();
    }

    public override void SetLayoutHorizontal()
    {
        var colIndex = 0;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            colIndex = i % Columns;

            var xPos = _cellSize.x * colIndex;
            xPos += _gap.x * colIndex;
            xPos += padding.left;

            SetChildAlongAxis(rectChildren[i], 0, xPos, _cellSize.x);
        }
    }

    public override void SetLayoutVertical()
    {
        var rowIndex = 0;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            rowIndex = i / Columns;

            var yPos = _cellSize.y * rowIndex;
            yPos += _gap.y * rowIndex;
            yPos += padding.top;

            SetChildAlongAxis(rectChildren[i], 1, yPos, _cellSize.y);
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        CalculateLayoutInputHorizontal();
        CalculateLayoutInputVertical();
        SetLayoutHorizontal();
        SetLayoutVertical();
    }

    #endregion

    
    #region Plumbery

    private void InitializeCellCount()
    {
        if(_fitType == GridLayout.Uniform || _fitType == GridLayout.MatchWidth || _fitType == GridLayout.MatchHeight)
        {
            var childRatio = Mathf.Sqrt(rectChildren.Count);
            Columns = Mathf.CeilToInt(childRatio);
            Rows = Columns;
        }
    }

    private float AutoDetermineCellWidth()
    {
        var cellWidth = rectTransform.rect.width;       // Set initial available width
        cellWidth -= _gap.x * (Columns - 1);            // Substract total horizontal gap
        cellWidth -= padding.left + padding.right;      // Substract total horizontal padding
        cellWidth /= Columns;                           // Convert remaining available size to horizontal cell size
        return cellWidth;
    }
    
    private float AutoDermineCellHeight()
    {
        var cellHeight = rectTransform.rect.height;     // Set initial available height
        cellHeight -= _gap.y * (Rows - 1);              // Substract total vertical gap
        cellHeight -= padding.top + padding.bottom;     // Substract total vertical padding
        cellHeight /= Rows;                             // Convert remaining available size to vertical cell size
        return cellHeight;
    }

    private int DetermineColumnCount()
    {
        switch(_fitType)
        {
            case GridLayout.MatchHeight         :
            case GridLayout.FixedRows           : return Mathf.CeilToInt(rectChildren.Count / (float)Rows);
            case GridLayout.FixedColumns        : 
            case GridLayout.FixedRowsAndColumns : 
            default                             : return Columns;
        }
    }
    
    private int DetermineRowCount()
    {
        switch(_fitType)
        {
            case GridLayout.MatchWidth          :
            case GridLayout.FixedColumns        : return Mathf.CeilToInt(rectChildren.Count / (float)Columns);
            case GridLayout.FixedRows           : 
            case GridLayout.FixedRowsAndColumns : 
            default                             : return Rows;
        }   
    }

    #endregion


    public enum GridLayout
    {
        Uniform,                // Create a uniform grid with the same number of row and columns with an automatic cell size that cover all the available space. This may lead to some blank space in he layout
        MatchWidth,             // Create a grid that have a priority to fill all the available space with more columns than rows if applicable
        MatchHeight,            // Create a grid that have a priority to fill all the available space with more rows than columns if applicable
        FixedRows,              // Create a grid with given row number and applying auto layout to everything else to fill the available space
        FixedColumns,           // Create a grid with given column number and applying auto layout to everything else to fill the available space
        FixedRowsAndColumns     // Create a grid with given column and row number and applying auto layout to everything else to fill the available space
    }
}
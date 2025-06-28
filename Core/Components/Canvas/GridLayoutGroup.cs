using System;
using System.Collections.Generic;
using System.Linq;

public class GridLayoutGroup : Component, IStart, ICanvasComponent
{
    public IReadOnlyList<GameObject> Items => _items.Select(x => x.Gameobject).ToList();
    public int Columns
    {
        get => _columns / 2;
        set
        {
            _columns = value <= 0 ? 1 : value;
            UpdateItemsPositions();
        }
    }
    private int _columns;
    private readonly int _marginLeft;
    private readonly int _marginRight;
    private readonly int _marginTop;
    private readonly int _marginDown;
    private int _itemsCount;
    private IReadOnlyList<Transform2D> _items = [];
    
    public GridLayoutGroup() : this(3){}
    public GridLayoutGroup(int columns, int margin = 0) : this(columns, margin, margin, margin, margin){}
    public GridLayoutGroup(int columns, int marginLeft, int marginRight, int marginTop, int marginDown)
    {
        _columns = columns;
        _marginLeft = marginLeft;
        _marginRight = marginRight;
        _marginTop = marginTop;
        _marginDown = marginDown;
    }

    public override void Start()
    {
        gameObject.Canvas.UIObjectsCountHasChanged += OnUIObjectsCountHasChanged;
        _itemsCount = gameObject.ChildrensCount;
        _items = gameObject.Transform.Childrens;
    }

    private void OnUIObjectsCountHasChanged()
    {
        _itemsCount = gameObject.ChildrensCount;
        UpdateItemsPositions();
    }

    private void UpdateItemsPositions()
    {
        if(false == _items[0].Gameobject.TryGetComponent(out CanvasHandler canvasHandler)) return;

        int rows = (int)Math.Ceiling(Convert.ToDouble(_itemsCount) / Convert.ToDouble(_columns));
        int spriteWidth = _itemsCount > 0 ? canvasHandler.Width : 0;
        int spriteHeight = _itemsCount > 0 ? canvasHandler.Height : 0;
        int gridWidth = _columns * spriteWidth;
        int gridHeight = rows * spriteHeight;
        float absolutePositionX = gameObject.Transform.Parent.AbsolutePosition.X;
        float absolutePositionY = gameObject.Transform.Parent.AbsolutePosition.Y;
        float firstPosX = absolutePositionX - gridWidth / 2 + spriteWidth / 2 - absolutePositionX;
        float firstPosY = absolutePositionY - gridHeight / 2 + spriteHeight / 2 - absolutePositionY;

        //Console.WriteLine($"Pos x {firstPosX} pos y {firstPosY} pos x uiobject {_items.ElementAt(0).UIObject.DrawPosition.X} pos y uiobject {_items.ElementAt(0).UIObject.DrawPosition.Y}");

        for (int i = 0; i < _itemsCount; i++)
        {
            float posX = firstPosX + i % _columns * spriteWidth;
            float posY = firstPosY + i / _columns * spriteHeight;
            _items[i].Position = new(posX, posY);
        }

    }

    public override void Destroy()
    {
        gameObject.Canvas.UIObjectsCountHasChanged -= OnUIObjectsCountHasChanged;
    }
}

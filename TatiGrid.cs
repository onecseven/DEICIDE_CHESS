using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class TatiGrid : Node2D
{
    //[Export]
    //GameWrapper gameWrapper = null;
    [Export]
    public Color ColorBase { get; set; } = Colors.Red;

    [Export]
    public bool DrawGrid = true;

    [Export]
    public bool Reversed = false;
    
    [Export]
    public int SideLength
    {
        get => _sideSize; set
        {
            _sideSize = value;
            QueueRedraw();
        }
    }
    public int sideSize;

    [Export]
    public int BoardLength
    {
        get => _boardlength; set
        {
            _boardlength = value;
            QueueRedraw();
        }
    }
    public int _sideSize;
    public int _boardlength;

    public override void _Ready()
	{
	}

    public override void _Draw()
    {
        if (!DrawGrid) return;
        bool color = Reversed;
        for (int i = 0; i < _boardlength; i++)
        {
            for (int j = 0; j < _boardlength; j++)
            {
                var red = new Rect2(new Vector2(j* _sideSize, i* _sideSize), new Vector2(_sideSize, _sideSize));
                DrawRect(red, Colors.Red, color, 1);
                DrawString(ThemeDB.FallbackFont, new Vector2((j * _sideSize) + _sideSize / 3, (i * _sideSize) + _sideSize / 2), $"{j},{i}",HorizontalAlignment.Left,-1, 8);
                color = !color;
            }
            color = !color;
        }        
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}

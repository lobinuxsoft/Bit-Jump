using Godot;
using System;
using System.Collections.Generic;

public class ProgressionTester : Node2D
{
    [Export] private PackedScene circle;
    [Export] private Font font;
    private int level;
    private Vector2 lastCirclePosition;
    private List<Vector2> levelMarkers = new List<Vector2>();

    private Camera2D _camera2D;

    public override void _Ready()
    {
        _camera2D = GetNode<Camera2D>("Camera2D");
        GD.Randomize();
        lastCirclePosition = _camera2D.Position;
        level = 0;

        for (int i = 0; i < 500; i++)
        {
            if (i % 5 == 0)
            {
                level++;
                levelMarkers.Add(lastCirclePosition);
            }

            SpawnCircle();
        }
        Update();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("ui_up"))
            _camera2D.Position += new Vector2(0,-20);
        if (Input.IsActionPressed("ui_down"))
            _camera2D.Position += new Vector2(0,20);
        if (Input.IsActionPressed("ui_left"))
            _camera2D.Position += new Vector2(-20,0);
        if (Input.IsActionPressed("ui_right"))
            _camera2D.Position += new Vector2(20,0);
    }
    
    private void SpawnCircle()
    {
        Vector2 position = Vector2.Zero;
        Circle c = (Circle)circle.Instance();
        var x = (float)GD.RandRange(-150, 150);
        var y = (float)GD.RandRange(-500, -400);
        position = lastCirclePosition + new Vector2(x, y);
        AddChild(c);
        c.Init(position, level);
        lastCirclePosition = position;
    }

    public override void _Draw()
    {
        var l = 1;
        foreach (var pos in levelMarkers)
        {
            var s = new Vector2(pos.x - 480, pos.y - 200);
            var e = new Vector2(pos.x - 80, pos.y - 200);
            DrawLine(s, e, Colors.White, 15);
            DrawString(font, s - new Vector2(0,50), $"Level {l}", Colors.White);
            l++;
        }
    }
}

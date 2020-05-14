using Godot;
using System;

public class Jumper : Area2D
{
    [Export] private Vector2 velocity = new Vector2(100, 0);
    [Export(PropertyHint.Range, "100, 3000")] private float jumpSpeed = 1000;
    [Export] private int _trailLenght = 25;

    private Line2D _trail;
    
    private Circle _target = null;
    
    public Circle Target => _target;

    [Signal] public delegate void OnCapture(Circle circle);

    public override void _Ready()
    {
        Connect("area_entered", this, nameof(OnAreaEnter));
        _trail = GetNode<Line2D>("Trail/Points");
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_trail.Points.Length > _trailLenght)
        {
            _trail.RemovePoint(0);
        }
        _trail.AddPoint(Position);
        
        if (_target != null)
        {
            Transform = _target.OrbitPosition.GlobalTransform;
        }
        else
        {
            Position += velocity * delta;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_target != null && @event is InputEventScreenTouch screenTouch && screenTouch.Pressed)
        {
            Jump();
        }
    }

    private void Jump()
    {
        _target.Implode();
        _target = null;
        velocity = Transform.x * jumpSpeed;
    }

    public void OnAreaEnter(Area2D area)
    {
        _target = (Circle)area;
        _target.GetNode<Node2D>("Pivot").Rotation = (Position - _target.Position).Angle();
        velocity = Vector2.Zero;
        EmitSignal(nameof(OnCapture), _target);
    }
}

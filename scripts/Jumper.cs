using Godot;
using System;

public class Jumper : Area2D
{
    [Export] private Vector2 velocity = new Vector2(100, 0);
    [Export(PropertyHint.Range, "100, 3000")] private float jumpSpeed = 1000;

    private Circle target = null;

    public override void _Ready()
    {
        Connect("area_entered", this, nameof(OnAreaEnter));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (target != null)
        {
            Transform = target.OrbitPosition.GlobalTransform;
        }
        else
        {
            Position += velocity * delta;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (target != null && @event is InputEventScreenTouch screenTouch && screenTouch.Pressed)
        {
            Jump();
        }
    }

    private void Jump()
    {
        target = null;
        velocity = Transform.x * jumpSpeed;
    }

    public void OnAreaEnter(Area2D area)
    {
        target = (Circle)area;
        velocity = Vector2.Zero;
    }
}

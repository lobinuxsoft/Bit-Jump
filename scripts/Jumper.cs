using Godot;
using System;

public class Jumper : Area2D
{
    [Export] private Vector2 velocity = new Vector2(100, 0);
    [Export(PropertyHint.Range, "100, 3000")] private float jumpSpeed = 1000;
    [Export] private int _trailLenght = 25;

    private Line2D _trail;
    
    private Circle _target = null;
    
    private AudioStreamPlayer _jumpSound;
    private AudioStreamPlayer _captureSound;

    private GameSkin _gameSkin;
    
    public Circle Target => _target;

    [Signal] public delegate void OnCapture(Circle circle);
    [Signal] public delegate void OnDie();

    public override void _Ready()
    {
        Connect("area_entered", this, nameof(OnAreaEnter));
        _trail = GetNode<Line2D>("Trail/Points");
        
        _jumpSound = GetNode<AudioStreamPlayer>("Jump");
        _captureSound = GetNode<AudioStreamPlayer>("Capture");

        _gameSkin = SkinManager.instance.GameSkins[SkinManager.instance.skinSelected];
        GetNode<Sprite>(nameof(Sprite)).Modulate = _gameSkin.jumperColor;
        _trail.DefaultColor = _gameSkin.jumperColor;
        _trail.Gradient = _gameSkin.trailGradient;
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

        if (Settings.instance.enableSound)
        {
            _jumpSound.Play();
        }
    }

    public void Die()
    {
        _target = null;
        QueueFree();
    }

    public void OnAreaEnter(Area2D area)
    {
        _target = (Circle)area;
        // _target.GetNode<Node2D>("Pivot").Rotation = (Position - _target.Position).Angle();
        velocity = Vector2.Zero;
        EmitSignal(nameof(OnCapture), _target);

        if (Settings.instance.enableSound)
        {
            _captureSound.Play();
        }
    }

    public void ScreenExit()
    {
        if (_target == null)
        {
            EmitSignal(nameof(OnDie));
            Die();
        }
    }
}

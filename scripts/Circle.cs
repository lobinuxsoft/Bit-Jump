using Godot;
using System;

public class Circle : Area2D
{
    private float _radius = 100;
    private float _rotationSpeed = Mathf.Pi;
    private Position2D _orbitPosition;
    private Node2D _pivot;
    private CollisionShape2D _collisionShape2D;
    private CircleShape2D _circleShape2D;
    private Sprite _sprite;
    private AnimationPlayer _animationPlayer;
    private float imgSize;

    public Position2D OrbitPosition => _orbitPosition;

    public void Init(Vector2 position, float radius = 100)
    {
        Position = position;
        _radius = radius;
        
        _pivot = GetNode<Node2D>("Pivot");
        _orbitPosition = GetNode<Position2D>("Pivot/OrbitPosition");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _sprite = GetNode<Sprite>("Sprite");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _circleShape2D = (CircleShape2D)_collisionShape2D.Shape;
        _circleShape2D.Radius = _radius;
        imgSize = _sprite.Texture.GetSize().x / 2;
        
        _sprite.Scale = Vector2.One * _radius / imgSize;

        _orbitPosition.Position = new Vector2(_radius + 25, 0);

        _rotationSpeed *= Mathf.Pow(-1, GD.Randi() % 2);
    }

    public async void Implode()
    {
        _animationPlayer.Play("implode");
        await ToSignal(_animationPlayer, "animation_finished");
        QueueFree();
    }

    public void Capture()
    {
        _animationPlayer.Play("capture");
    }

    public override void _Process(float delta) 
    { 
        _pivot.Rotation += _rotationSpeed * delta; 
    }
}



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
    private float imgSize;

    public Position2D OrbitPosition => _orbitPosition;

    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            if(_circleShape2D != null)
                _circleShape2D.Radius = _radius;
            
            if(_sprite != null)
                _sprite.Scale = Vector2.One * _radius / imgSize;

            if (_orbitPosition != null)
                _orbitPosition.Position = new Vector2(_radius + 25, 0);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _pivot = GetNode<Node2D>("Pivot");
        _orbitPosition = GetNode<Position2D>("Pivot/OrbitPosition");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _sprite = GetNode<Sprite>("Sprite");

        _circleShape2D = (CircleShape2D)_collisionShape2D.Shape;
        _circleShape2D.Radius = _radius;
        imgSize = _sprite.Texture.GetSize().x / 2;
        
        _sprite.Scale = Vector2.One * _radius / imgSize;

        _orbitPosition.Position = new Vector2(_radius + 25, 0);
    }
    
    public override void _Process(float delta) 
    { 
        _pivot.Rotation += _rotationSpeed * delta; 
    }
}



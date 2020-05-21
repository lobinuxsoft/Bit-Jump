using Godot;
using System;

public class Circle : Area2D
{
    public enum MODES
    {
        STATIC,
        LIMITED
    }

    [Export] private Gradient _gradient;
    private MODES _mode = MODES.STATIC;
    private int _maxCycle = 3;
    private int _curCycle = 0;
    private float _orbitStart;
    private float _radius = 100;
    private float _rotationSpeed = Mathf.Pi;
    private Position2D _orbitPosition;
    private Node2D _pivot;
    private CollisionShape2D _collisionShape2D;
    private CircleShape2D _circleShape2D;
    private Sprite _sprite;
    private Sprite _spriteEffect;
    private AnimationPlayer _animationPlayer;
    private float imgSize;
    private Label _label;
    private Jumper _jumper = null;

    private Settings _settings;
    private GameSkin _gameSkin;
    private AudioStreamPlayer _audioStreamPlayer;

    public Position2D OrbitPosition => _orbitPosition;

    public void Init(Vector2 position, float radius = 100, MODES modes = MODES.LIMITED)
    {
        Position = position;
        _radius = radius;
        
        _pivot = GetNode<Node2D>("Pivot");
        _orbitPosition = GetNode<Position2D>("Pivot/OrbitPosition");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _sprite = GetNode<Sprite>("Sprite");
        _spriteEffect = GetNode<Sprite>("SpriteEffect");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _label = GetNode<Label>("Label");

        _settings = GetTree().Root.GetNode<Settings>(nameof(Settings));
        _gameSkin = SkinManager.instance.GameSkins[SkinManager.instance.skinSelected];
        _gradient = _gameSkin.circleGradient;
        
        _audioStreamPlayer = GetNode<AudioStreamPlayer>("Beep");
        
        SetMode(modes);

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

    public void Capture(Jumper jumper)
    {
        _jumper = jumper;
        _animationPlayer.Play("capture");
        _pivot.Rotation = (_jumper.Position - Position).Angle();
        _orbitStart = _pivot.Rotation;
    }

    public override void _Process(float delta) 
    { 
        _pivot.Rotation += _rotationSpeed * delta;
        if (_mode == MODES.LIMITED && _jumper != null)
        {
            CheckOrbit();
        }
    }

    private void CheckOrbit()
    {
        if (Mathf.Abs(_pivot.Rotation - _orbitStart) > 2 * Mathf.Pi)
        {
            _curCycle--;
            
            if (_settings.enableSound)
            {
                _audioStreamPlayer.Play();
                
            }
            
            _sprite.Modulate = _gradient.Interpolate((float) _curCycle / _maxCycle);
            _label.Text = $"{_curCycle}";
            if (_curCycle <= 0)
            {
                _jumper.Die();
                _jumper = null;
                Implode();
            }
            
            _orbitStart = _pivot.Rotation;
        }
    }


    private void SetMode(MODES mode)
    {
        _mode = mode;
        _curCycle = _maxCycle;
        
        _sprite.Modulate = _gradient.Interpolate((float) _curCycle / _maxCycle);
        _spriteEffect.Modulate = _gradient.Interpolate((float) _curCycle / _maxCycle);
        

        switch (mode)
        {
            case MODES.STATIC:
                _label.Hide();
                break;
            case MODES.LIMITED:
                _label.Text = $"{_curCycle}";
                _label.Show();
                break;
        }
    }
}



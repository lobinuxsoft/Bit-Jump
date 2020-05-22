using Godot;
using System;
using Object = Godot.Object;

public class Circle : Area2D
{
    public enum MODES
    {
        STATIC = 0,
        LIMITED = 1
    }

    [Export] private Gradient _gradient;
    private MODES _mode = MODES.STATIC;
    private float _moveRange = 0;
    private float _moveSpeed = 1;
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
    
    private GameSkin _gameSkin;
    private AudioStreamPlayer _audioStreamPlayer;
    private Tween _moveTween;

    public Position2D OrbitPosition => _orbitPosition;

    public void Init(Vector2 position, int level = 1)
    {
        int mode = Settings.instance.RandWeighted(new[] {10, level - 1});
        Position = position;
        
        float moveChance = Mathf.Clamp((float)level - 10f, 0f, 9f) / 10f;
        
        if (GD.Randf() < moveChance)
        {
            _moveRange = Mathf.Max(25, 100 * (float)GD.RandRange(.75f, 1.25f) * moveChance) * Mathf.Pow(-1, GD.Randi() % 2);
            _moveSpeed = Mathf.Max(2.5f - Mathf.Ceil((float)level / 5) * .25f, .75f);
        }

        float smallChance = Mathf.Min(.9f, Mathf.Max(0f, ((float)level - 10f) / 20f));
        
        if (GD.Randf() < smallChance)
        {
            _radius = Mathf.Max(50f, _radius - (float)level * (float) GD.RandRange(.75f, 1.25f));
        }

        _pivot = GetNode<Node2D>("Pivot");
        _orbitPosition = GetNode<Position2D>("Pivot/OrbitPosition");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        _sprite = GetNode<Sprite>("Sprite");
        _spriteEffect = GetNode<Sprite>("SpriteEffect");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _label = GetNode<Label>("Label");
        
        _gameSkin = SkinManager.instance.GameSkins[SkinManager.instance.skinSelected];

        _audioStreamPlayer = GetNode<AudioStreamPlayer>("Beep");
        _moveTween = GetNode<Tween>("MoveTween");
        
        SetMode((MODES)mode);

        _circleShape2D = (CircleShape2D)_collisionShape2D.Shape;
        _circleShape2D.Radius = _radius;
        imgSize = _sprite.Texture.GetSize().x / 2;
        
        _sprite.Scale = Vector2.One * _radius / imgSize;

        _orbitPosition.Position = new Vector2(_radius + 25, 0);

        _rotationSpeed *= Mathf.Pow(-1, GD.Randi() % 2);
        
        SetTween();
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
            
            if (Settings.instance.enableSound)
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
        

        switch (mode)
        {
            case MODES.STATIC:
                _gradient = _gameSkin.circleGradient;
                _label.Hide();
                break;
            case MODES.LIMITED:
                _label.Text = $"{_curCycle}";
                _gradient = _gameSkin.timerCircleGradient;
                _label.Show();
                break;
        }
        
        _sprite.Modulate = _gradient.Interpolate((float) _curCycle / _maxCycle);
        _spriteEffect.Modulate = _gradient.Interpolate((float) _curCycle / _maxCycle);
    }

    public void SetTween(Object obj = null, NodePath key = null)
    {
        if(_moveRange == 0) return;

        _moveRange *= -1;

        _moveTween.InterpolateProperty(this, "position:x", Position.x, Position.x + _moveRange, _moveSpeed,
            Tween.TransitionType.Quad, Tween.EaseType.InOut);

        _moveTween.Start();
    }
}



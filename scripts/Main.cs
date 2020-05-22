using Godot;

public class Main : Node
{
    [Export] private readonly NodePath _cameraNode = "";
    [Export] private readonly NodePath _startPositionNode = "";
    [Export] private readonly NodePath _screenManagerNode = "";
    [Export] private readonly NodePath _hudNode = "";
    [Export] private readonly NodePath _musicNode = "";
    
    private PackedScene _circle = ResourceLoader.Load<PackedScene>("res://objects/Circle.tscn");
    private PackedScene _jumper = ResourceLoader.Load<PackedScene>("res://objects/Jumper.tscn");

    private Jumper _player = null;
    private int _score = 0;
    private int level = 0;

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            _hud.UpdateScore($"{_score}");
            if (_score > 0 && _score % Settings.instance.circlesPerLevel == 0)
            {
                level++;
                _hud.ShowMessage($"Level {level}");
            }
        }
    }

    private Camera2D _camera;
    private Position2D _startPos;

    private ScreensManager _screensManager;
    private HUD _hud;
    
    private AudioStreamPlayer _audioStreamPlayer;

    public override void _Ready()
    {
        _audioStreamPlayer = GetNode<AudioStreamPlayer>(_musicNode);
        
        _camera = GetNode<Camera2D>(_cameraNode);
        _startPos = GetNode<Position2D>(_startPositionNode);
        _screensManager = GetNode<ScreensManager>(_screenManagerNode);
        _hud = GetNode<HUD>(_hudNode);
        
        _hud.Hide();
        
        GD.Randomize();
    }

    private void NewGame()
    {
        Score = 0;
        level = 1;
        
        _camera.Position = _startPos.Position;
        _player = (Jumper) _jumper.Instance();
        _player.Position = _startPos.Position;
        AddChild(_player);
        _player.Connect("OnCapture", this, nameof(OnJumperCapture));
        _player.Connect("OnDie", this, nameof(OnJumperDie));
        SpawCircle(_startPos.Position);
        _hud.Show();
        _hud.ShowMessage($"GO!");

        if (Settings.instance.enableMusic)
        {
            _audioStreamPlayer.Play();
        }
    }

    private void SpawCircle(Vector2 startPosPosition, bool randomize = false)
    {
        var c = (Circle) _circle.Instance();

        if (randomize)
        {
            var x = (float)GD.RandRange(-150, 150);
            var y = (float)GD.RandRange(-500, -400);

            startPosPosition = _player.Target.Position + new Vector2(x, y);
        }
        
        AddChild(c);
        c.Init(startPosPosition);
    }

    public void OnJumperCapture(Circle circle)
    {
        _camera.Position = circle.Position;
        circle.Capture(_player);
        CallDeferred(nameof(SpawCircle), circle.Position, true);
        Score += 1;
    }

    public void OnJumperDie()
    {
        GetTree().CallGroup("circles", "Implode");
        _screensManager.GameOver();
        _hud.Hide();
        
        if (Settings.instance.enableMusic)
        {
            _audioStreamPlayer.Stop();
        }
    }
}
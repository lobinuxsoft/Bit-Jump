using Godot;

public class Main : Node
{
    public static Main Instace;
    
    [Export] private readonly NodePath _cameraNode = "";
    [Export] private readonly NodePath _startPositionNode = "";
    [Export] private readonly NodePath _screenManagerNode = "";
    [Export] private readonly NodePath _hudNode = "";
    [Export] private readonly NodePath _musicNode = "";
    
    private PackedScene _circle = ResourceLoader.Load<PackedScene>("res://objects/Circle.tscn");
    private PackedScene _jumper = ResourceLoader.Load<PackedScene>("res://objects/Jumper.tscn");

    private Jumper _player = null;
    private int _score = 0;
    private int _highScore = 0;
    private int level = 0;

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            _highScore = (_score > _highScore) ? _score : _highScore;
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
        if (Instace == null)
        {
            Instace = this;
        }
        else
        {
            if (Instace != this)
            {
                QueueFree();
            }
        }
        
        LoadScore();
        _audioStreamPlayer = GetNode<AudioStreamPlayer>(_musicNode);
        
        _camera = GetNode<Camera2D>(_cameraNode);
        _startPos = GetNode<Position2D>(_startPositionNode);
        _screensManager = GetNode<ScreensManager>(_screenManagerNode);
        _hud = GetNode<HUD>(_hudNode);
        
        _hud.Hide();
        
        GD.Randomize();

        Settings.instance.OnGameStart += NewGame;
    }

    private void NewGame()
    {
        Score = 0;
        level = 1;
        
        _camera.Position = _startPos.Position;
        
        _player = (Jumper) _jumper.Instance();
        _player.Position = _startPos.Position;
        
        AddChild(_player);
        
        _player.OnCapture += OnJumperCapture;
        _player.OnDie += OnJumperDie;
        
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
        c.Init(startPosPosition, level);
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
        _screensManager.GameOver(_score, _highScore, level);
        _hud.Hide();
        
        if (Settings.instance.enableMusic)
        {
            _audioStreamPlayer.Stop();
        }

        SaveScore();
        
        Settings.instance.GameOver();
    }

    private void SaveScore()
    {
        var file = new File();
        file.Open(Settings.instance.scoreFile, File.ModeFlags.Write);
        file.StoreVar(_highScore);
        file.Close();
    }
    
    private void LoadScore()
    {
        var file = new File();
        if (file.FileExists(Settings.instance.scoreFile))
        {
            file.Open(Settings.instance.scoreFile, File.ModeFlags.Read);
            _highScore = (int)file.GetVar();
            file.Close();
        }
    }

    public override void _ExitTree()
    {
        Settings.instance.OnGameStart -= NewGame;
    }
}
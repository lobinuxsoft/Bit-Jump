using Godot;

public class Main : Node
{
    [Export] private readonly NodePath _cameraNode = "";
    [Export] private readonly NodePath _startPositionNode = "";
    
    private PackedScene _circle = ResourceLoader.Load<PackedScene>("res://objects/Circle.tscn");
    private PackedScene _jumper = ResourceLoader.Load<PackedScene>("res://objects/Jumper.tscn");

    private Jumper _player = null;

    private Camera2D _camera;
    private Position2D _startPos;

    public override void _Ready()
    {
        _camera = GetNode<Camera2D>(_cameraNode);
        _startPos = GetNode<Position2D>(_startPositionNode);
        
        GD.Randomize();
        NewGame();
    }

    private void NewGame()
    {
        _camera.Position = _startPos.Position;
        _player = (Jumper) _jumper.Instance();
        _player.Position = _startPos.Position;
        AddChild(_player);
        _player.Connect("OnCapture", this, nameof(OnJumperCapture));
        SpawCircle(_startPos.Position);
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
    }
}
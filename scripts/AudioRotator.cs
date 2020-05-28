using Godot;
using System;

public class AudioRotator : Node
{
    [Export] private bool useBusChannel = true;
    [Export] private NodePath _nodeToRotate;
    [Export] Vector3 _axisToRotate = Vector3.Zero;
    [Export] private float _minRotSpeed = 15;
    [Export] private float _maxRotSpeed = 45;

    private Spatial _spatial;
    private int _busChannel = 0;

    private bool _processEffect = false;
    
    public override void _Ready()
    {
        Settings.instance.OnGameStart += ActivateProcess;
        Settings.instance.OnGameOver += DeactivateProcess;
        
        _spatial = GetNode<Spatial>(_nodeToRotate);
        
        GD.Randomize();
        _busChannel = (int) GD.RandRange(0, AudioAnalyzer.Instance.BusChannels.Count);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
      if(!_processEffect) return;
      
      if (_spatial != null)
      {
          if(useBusChannel)
            _spatial.Rotate(_axisToRotate.Normalized(), Mathf.Lerp(_minRotSpeed, _maxRotSpeed, AudioAnalyzer.Instance.BusChannels[_busChannel]) * delta);
          else
            _spatial.Rotate(_axisToRotate.Normalized(), Mathf.Lerp(_minRotSpeed, _maxRotSpeed, AudioAnalyzer.Instance.Amplitud) * delta);
      }
  }

  public override void _ExitTree()
  {
      Settings.instance.OnGameStart += ActivateProcess;
      Settings.instance.OnGameOver += DeactivateProcess;
  }

  private void ActivateProcess()
  {
      _processEffect = true;
  }

  private void DeactivateProcess()
  {
      _processEffect = false;
  }
}

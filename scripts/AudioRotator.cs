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
    
    public override void _Ready()
    {
        _spatial = GetNode<Spatial>(_nodeToRotate);
        
        GD.Randomize();
        _busChannel = (int) GD.RandRange(0, AudioAnalyzer.Instance.BusChannels.Count);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(float delta)
  {
      if (_spatial != null)
      {
          if(useBusChannel)
            _spatial.Rotate(_axisToRotate.Normalized(), Mathf.Lerp(_minRotSpeed, _maxRotSpeed, AudioAnalyzer.Instance.BusChannels[_busChannel]) * delta);
          else
            _spatial.Rotate(_axisToRotate.Normalized(), Mathf.Lerp(_minRotSpeed, _maxRotSpeed, AudioAnalyzer.Instance.Amplitud) * delta);
      }
  }
}

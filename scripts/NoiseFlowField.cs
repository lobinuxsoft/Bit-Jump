using Godot;
using System;
using System.Collections.Generic;

public class NoiseFlowField : MultiMeshInstance
{
    [Export] private Gradient _gradient;
    [Export] private Vector3 _gridSize = Vector3.One;
    [Export] private float _increment;
    [Export] private Vector3 _offset, _offsetSpeed;

    [Export] private OpenSimplexNoise _simplexNoise;

    List<FlowData> _flowDatas = new List<FlowData>();

    public override void _Ready()
    {
        Multimesh.InstanceCount = AudioAnalyzer.Instance.BusChannels.Count;
        InitInstances();
    }

    void InitInstances()
    {
        for (int i = 0; i < Multimesh.InstanceCount; i++)
        {
            var position = new Transform(Quat.Identity, new Vector3(GD.Randf() * _gridSize.x - _gridSize.x / 2, GD.Randf() * _gridSize.y - _gridSize.y / 2, GD.Randf() * _gridSize.z - _gridSize.z / 2));
            Multimesh.SetInstanceTransform(i, position);
            Multimesh.SetInstanceColor(i, _gradient.Interpolate((float) i / Multimesh.InstanceCount));
        }
    }
    
    public override void _Process(float delta)
    {
        for (int i = 0; i < Multimesh.InstanceCount; i++)
        {
            var t = Multimesh.GetInstanceTransform(i);
            t.Scaled(Vector3.Zero.LinearInterpolate(Vector3.One * 100, AudioAnalyzer.Instance.BusChannels[i]));
            
            Multimesh.SetInstanceTransform(i, t);
            
            Multimesh.SetInstanceColor(i, Colors.Black.LinearInterpolate(_gradient.Interpolate((float) i / Multimesh.InstanceCount), AudioAnalyzer.Instance.BusChannels[i]));
        }
    }

    void CalCulateFlowFieldDirections()
    {
        float xOff = 0f;
        for (int x = 0; x < _gridSize.x; x++)
        {
            float yOff = 0f;
            for (int y = 0; y < _gridSize.y; y++)
            {
                float zOff = 0f;
                for (int z = 0; z < _gridSize.z; z++)
                {
                    float noise = _simplexNoise.GetNoise3d(xOff + _offset.x, yOff + _offset.y, zOff + _offset.z) + 1;
                    Vector3 noiseDir = new Vector3(Mathf.Cos(noise * Mathf.Pi), Mathf.Sin(noise * Mathf.Pi), Mathf.Cos(noise * Mathf.Pi));
                    
                    _flowDatas.Add(new FlowData{direction = noiseDir.Normalized()});
                    
                    Vector3 pos = new Vector3(x, y, z);
                    Vector3 endPos = pos + noiseDir.Normalized();
                    
                    zOff += _increment;
                }
                yOff += _increment;
            }
            xOff += _increment;
        }
    }
}

public struct FlowData
{
    public Vector3 direction;
}

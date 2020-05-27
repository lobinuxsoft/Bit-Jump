using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public class NoiseFlowField : MultiMeshInstance
{
    [Export] private Gradient _gradient;
    [Export] private Vector3 _gridSize = Vector3.One;

    private bool processThread = false;

    public override void _Ready()
    {
        Settings.instance.OnGameStart += ActivateEffect;
        Settings.instance.OnGameOver += DeactivateEffect;
        
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
        if (!processThread) return;
        
        var data = new MultimeshData {multiMesh = Multimesh};
        ThreadPool.QueueUserWorkItem(RhythmCalculation, data);
    }

    void RhythmCalculation(object multiMesh)
    {
        var m = (MultimeshData) multiMesh;
        for (int i = 0; i < m.multiMesh.InstanceCount; i++)
        {
            var t = m.multiMesh.GetInstanceTransform(i);
            t.basis = new Basis(Vector3.Left * AudioAnalyzer.Instance.BusChannels[i] * 5, Vector3.Up * AudioAnalyzer.Instance.BusChannels[i] * 5, Vector3.Forward * AudioAnalyzer.Instance.BusChannels[i] * 5);
            
            m.multiMesh.SetInstanceTransform(i, t);
            
            m.multiMesh.SetInstanceColor(i, Colors.Black.LinearInterpolate(_gradient.Interpolate((float) i / m.multiMesh.InstanceCount), AudioAnalyzer.Instance.BusChannels[i]));
        }
    }

    public override void _ExitTree()
    {
        Settings.instance.OnGameStart -= ActivateEffect;
        Settings.instance.OnGameOver -= DeactivateEffect;
    }

    private void ActivateEffect()
    {
        Visible = true;
        processThread = true;
    }

    private void DeactivateEffect()
    {
        processThread = false;
        Visible = false;
    }
}

public struct MultimeshData
{
    public MultiMesh multiMesh;
}

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
        
        var data = new MultimeshDataStruct {MultiMesh = Multimesh};
        ThreadPool.QueueUserWorkItem(ThreadProcess, data);
    }

    void ThreadProcess(object multiMesh)
    {
        if(!processThread) return;
        
        var m = (MultimeshDataStruct) multiMesh;
        for (int i = 0; i < m.MultiMesh.InstanceCount; i++)
        {
            m.MultiMesh.SetInstanceCustomData(i, new Color(AudioAnalyzer.Instance.BusChannels[i] * 5f, AudioAnalyzer.Instance.BusChannels[i] * 5f, AudioAnalyzer.Instance.BusChannels[i] * 5f));
            
            m.MultiMesh.SetInstanceColor(i, Colors.Black.LinearInterpolate(_gradient.Interpolate((float) i / m.MultiMesh.InstanceCount), AudioAnalyzer.Instance.BusChannels[i]));
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
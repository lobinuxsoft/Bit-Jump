using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public class RingVisualizer : MultiMeshInstance
{
    [Export] private Gradient _gradient;
    [Export(PropertyHint.Range, "0.0, 200.0")] private float radius = 1;
    [Export] private Vector3 minScale = new Vector3(1, 1, 1);
    [Export] private Vector3 maxScale = new Vector3(1, 10, 1);

    private bool process = false;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Settings.instance.OnGameStart += ActivateEffect;
        Settings.instance.OnGameOver += DeactivateEffect;
        
        Multimesh.InstanceCount = AudioAnalyzer.Instance.BusChannels.Count;
    }

    public override void _Process(float delta)
    {
        if(!process) return;
        
        var data = new MultimeshDataStruct {MultiMesh = Multimesh};
        ThreadPool.QueueUserWorkItem(ThreadProcess, data);
    }

    private void ThreadProcess(object data)
    {
        var m = (MultimeshDataStruct) data;
        
        for (int i = 0; i < m.MultiMesh.InstanceCount; i++)
        { 
            float angle = i * 2 * Mathf.Pi / m.MultiMesh.InstanceCount;
            
            var vertical = Mathf.Sin(angle);
            var horizontal = Mathf.Cos(angle);
           
            var spawnDir = new Vector3(horizontal, 0, vertical);
            
            var pos = spawnDir * radius;
            
            var t = new Transform(Quat.Identity, pos);
            
            Vector3 scale = minScale.LinearInterpolate(maxScale, AudioAnalyzer.Instance.BusChannels[i]);

            Color color = _gradient.Interpolate((float) i / m.MultiMesh.InstanceCount);
            
            m.MultiMesh.SetInstanceCustomData(i, new Color(scale.x, scale.y, scale.z));

            m.MultiMesh.SetInstanceColor(i, Colors.Black.LinearInterpolate(color, AudioAnalyzer.Instance.BusChannels[i]));
            
            m.MultiMesh.SetInstanceTransform(i, t);
        }
    }

    private void ActivateEffect()
    {
        process = true;
        Visible = true;
    }

    private void DeactivateEffect()
    {
        process = false;
        Visible = false;
    }

    public override void _ExitTree()
    {
        Settings.instance.OnGameStart -= ActivateEffect;
        Settings.instance.OnGameOver -= DeactivateEffect;
    }
}

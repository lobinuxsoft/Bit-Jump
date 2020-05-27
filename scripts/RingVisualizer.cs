using Godot;
using System;
using System.Collections.Generic;

public class RingVisualizer : MultiMeshInstance
{
    [Export] private Gradient _gradient;
    [Export(PropertyHint.Range, "0.0, 200.0")] private float radius = 1;
    [Export] private Vector3 minScale = new Vector3(1, 1, 1);
    [Export] private Vector3 maxScale = new Vector3(1, 10, 1);
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Multimesh.InstanceCount = AudioAnalyzer.Instance.BusChannels.Count;
    }

    public override void _Process(float delta)
    {
        for (int i = 0; i < Multimesh.InstanceCount; i++)
        { 
            float angle = i * 2 * Mathf.Pi / Multimesh.InstanceCount;
            
            var vertical = Mathf.Sin(angle);
            var horizontal = Mathf.Cos(angle);
           
            var spawnDir = new Vector3(horizontal, 0, vertical);
            
            var pos = Transform.origin + spawnDir * radius;
            
            var t = new Transform(Quat.Identity, pos);
            
            Vector3 scale = minScale.LinearInterpolate(maxScale, AudioAnalyzer.Instance.BusChannels[i]);
    
            t = t.Scaled(scale);

            Color color = _gradient.Interpolate((float) i / Multimesh.InstanceCount);

            Multimesh.SetInstanceColor(i, Colors.Black.LinearInterpolate(color, AudioAnalyzer.Instance.BusChannels[i]));
            
            Multimesh.SetInstanceTransform(i, t);
        }
    }
}

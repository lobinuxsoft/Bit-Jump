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
        Multimesh.InstanceCount = AudioAnalyzer.instance.Bands.Count;
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
            
            var t = Transform.Translated(pos);
            
            Vector3 scale = minScale.LinearInterpolate(maxScale, AudioAnalyzer.instance.Bands[i]);
    
            t = t.Scaled(scale);
            
            Multimesh.SetInstanceColor(i, _gradient.Interpolate((float)i/Multimesh.InstanceCount));
            
            Multimesh.SetInstanceTransform(i, t);
        }
    }
}

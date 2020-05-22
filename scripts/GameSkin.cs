using Godot;
using Godot.Collections;

[System.Serializable]
public class GameSkin : Resource
{
    [Export] public Color jumperColor = Colors.White;
    [Export] public Gradient trailGradient = new Gradient();
    [Export] public Gradient circleGradient = new Gradient();
    [Export] public Gradient timerCircleGradient = new Gradient();
}

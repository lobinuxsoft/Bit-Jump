using Godot;
using System;

public class BaseScreen : CanvasLayer
{
    public Tween tween;

    public override void _Ready()
    {
        tween = GetNode<Tween>("Tween");
    }

    public void Show()
    {
        tween.InterpolateProperty(this, "offset:x", 500, 0, .5f, Tween.TransitionType.Back);
        tween.Start();
    }
    
    public void Hide()
    {
        tween.InterpolateProperty(this, "offset:x", 0, 500, .5f, Tween.TransitionType.Back);
        tween.Start();
    }
}

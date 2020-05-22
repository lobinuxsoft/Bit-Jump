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
        GetTree().CallGroup("buttons", "set_disabled", false);
        tween.InterpolateProperty(this, "offset:x", 500, 0, .5f, Tween.TransitionType.Back);
        tween.Start();
    }
    
    public void Hide()
    {
        GetTree().CallGroup("buttons", "set_disabled", true);
        tween.InterpolateProperty(this, "offset:x", 0, 500, .5f, Tween.TransitionType.Back);
        tween.Start();
    }
}

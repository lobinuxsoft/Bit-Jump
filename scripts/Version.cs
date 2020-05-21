using Godot;
using System;

public class Version : Label
{
    public override void _Ready()
    {
        Text = $"Version {ProjectSettings.GetSetting("application/version")}";
    }
}

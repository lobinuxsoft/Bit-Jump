using Godot;
using System;
using Godot.Collections;

public class SkinManager : Node
{
    public static SkinManager instance;

    [Export] public string skinSelected = "Normal";
    [Export] public Dictionary<string, GameSkin> GameSkins;

    public override void _Ready()
    {
        instance = this;
    }
}

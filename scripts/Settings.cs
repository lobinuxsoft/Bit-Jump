using Godot;

public class Settings : Node
{
    public static Settings instance;
    
    public bool enableSound = true;
    public bool enableMusic = true;

    public int circlesPerLevel = 5;

    public override void _Ready()
    {
        instance = this;
    }
}

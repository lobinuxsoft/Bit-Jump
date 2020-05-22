using Godot;

public class Settings : Node
{
    public static Settings instance;

    public string scoreFile = "user://highscore.save";
    public bool enableSound = true;
    public bool enableMusic = true;

    public int circlesPerLevel = 5;

    public override void _Ready()
    {
        instance = this;
    }

    public int RandWeighted(int[] weights)
    {
        GD.Randomize();
        
        var sum = 0;
        var result = 0;

        foreach (var weight in weights)
        {
            sum += weight;
        }

        var num = (float)GD.RandRange(0, sum);
        for (int i = 0; i < weights.Length; i++)
        {
            if (num < weights[i])
            {
                result = i;
                return result;
            }
            
            num -= weights[i];
        }

        return result;
    }
}

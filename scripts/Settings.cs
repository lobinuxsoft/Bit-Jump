using System;
using System.Threading;
using Godot;
using Object = Godot.Object;

public class Settings : Node
{
    public static Settings instance;

    public string scoreFile = "user://highscore.save";
    public bool enableSound = true;
    public bool enableMusic = true;

    public int circlesPerLevel = 5;

    public Object Firebase;
    
    public event GameEvent OnGameStart;
    public event GameEvent OnGameOver;

    public override void _Ready()
    {
        instance = this;

        if (Engine.HasSingleton("Firebase"))
        {
            Firebase = Engine.GetSingleton("Firebase");
            if (Firebase != null)
            {
                Firebase.Call("init", GetInstanceId());
            }
        }
    }
    
    //Firebase implementation
    public void _on_firebase_receive_message(string tag, string from, object key, object data)
    {
        GD.Print($"TAG: {tag} || From: {from} || Key: {key} || Data: {data}");
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

    public void GameStart()
    {
        OnGameStart?.Invoke();
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
    }
}

public delegate void GameEvent();

public delegate void GameEvent<T>(T data);

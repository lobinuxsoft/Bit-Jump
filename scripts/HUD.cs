using Godot;
using System;

public class HUD : CanvasLayer
{
    private Label _messageLabel;
    private Label _scoreLabel;
    private AnimationPlayer _animationPlayer;
    private MarginContainer _scoreBox;
    
    public override void _Ready()
    {
        _messageLabel = GetNode<Label>("Message");
        _scoreLabel = GetNode<Label>("ScoreBox/HBoxContainer/Score");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _scoreBox = GetNode<MarginContainer>("ScoreBox");
    }

    public void ShowMessage(string message)
    {
        _messageLabel.Text = message;
        _animationPlayer.Play("show_message");
    }

    public void Show()
    {
        _scoreBox.Show();
    }
    
    public void Hide()
    {
        _scoreBox.Hide();
    }

    public void UpdateScore(string value)
    {
        _scoreLabel.Text = value;
    }
}

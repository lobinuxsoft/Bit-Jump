using System;
using Godot;
using Godot.Collections;

public class ScreensManager : Node
{
	[Signal] public delegate void OnStartGame();
	
	private BaseScreen _currentScreen = null;

	private AudioStreamPlayer _audioStreamPlayer;

	private TextureButton musicButton;
	[Export] private Dictionary<bool, Texture> musicTexture;
	private TextureButton soundFXButton;
	[Export] private Dictionary<bool, Texture> soundTexture;

	private Label scoreLabel;
	private Label highScoreLabel;
	private Label levelLabel;

	public override void _Ready()
	{
		RegisterButtons();
		ChangeScreen(GetNode<BaseScreen>("TitleScreen"));

		scoreLabel = GetNode<Label>("GameOverScreen/MarginContainer/VBoxContainer/Score");
		highScoreLabel = GetNode<Label>("GameOverScreen/MarginContainer/VBoxContainer/HighScore");
		levelLabel = GetNode<Label>("GameOverScreen/MarginContainer/VBoxContainer/FarLevel");
		
		_audioStreamPlayer = GetNode<AudioStreamPlayer>("Click");
	}

	private void RegisterButtons()
	{
		var buttons = GetTree().GetNodesInGroup("buttons");
		foreach (var button in buttons)
		{
			if (button is TextureButton)
			{
				var but = (TextureButton) button;
				switch (but.Name)
				{
					case "Home":
						but.Connect("pressed", this, nameof(ToHome));
						break;
					case "Play":
						but.Connect("pressed", this, nameof(ToPlay));
						break;
					case "Settings":
						but.Connect("pressed", this, nameof(ToSetting));
						break;
					case "Music":
						musicButton = but;
						musicButton.Connect("pressed", this, nameof(Music));
						break;
					case "SoundFX":
						soundFXButton = but;
						soundFXButton.Connect("pressed", this, nameof(SoundFX));
						break;
				}
				// but.Connect("pressed", this, nameof(OnButtonPressed), new Array(but.Name));
			}
		}
	}

	public void ToHome()
	{
		ChangeScreen(GetNode<BaseScreen>("TitleScreen"));

		if (Settings.instance.enableSound)
		{
			_audioStreamPlayer.Play();
		}
	}

	public async void ToPlay()
	{
		ChangeScreen(null);
		await ToSignal(GetTree().CreateTimer(.5f), "timeout");
		EmitSignal(nameof(OnStartGame));
		
		if (Settings.instance.enableSound)
		{
			_audioStreamPlayer.Play();
		}
	}

	public void ToSetting()
	{
		ChangeScreen(GetNode<BaseScreen>("SettingScreen"));
		
		if (Settings.instance.enableSound)
		{
			_audioStreamPlayer.Play();
		}
	}

	public void Music()
	{
		Settings.instance.enableMusic = !Settings.instance.enableMusic;
		musicButton.TextureNormal = musicTexture[Settings.instance.enableMusic];
		
		if (Settings.instance.enableSound)
		{
			_audioStreamPlayer.Play();
		}
	}

	public void SoundFX()
	{
		Settings.instance.enableSound = !Settings.instance.enableSound;
		soundFXButton.TextureNormal = soundTexture[Settings.instance.enableSound];
		
		if (Settings.instance.enableSound)
		{
			_audioStreamPlayer.Play();
		}
	}
	
	// public void OnButtonPressed(string buttonName)
	// {
	// 	switch (buttonName)
	// 	{
	// 		case "Home":
	// 			ChangeScreen(GetNode<BaseScreen>("TitleScreen"));
	// 			break;
	// 		case "Play":
	// 			ChangeScreen(null);
	// 			EmitSignal(nameof(OnStartGame));
	// 			break;
	// 		case "Settings":
	// 			ChangeScreen(GetNode<BaseScreen>("SettingScreen"));
	// 			break;
	// 	}
	// }

	private async void ChangeScreen(BaseScreen screen)
	{
		if (_currentScreen != null)
		{
			_currentScreen.Hide();
			await ToSignal(_currentScreen.tween, "tween_completed");
			
		}
		
		_currentScreen = screen;
		
		if (screen != null)
		{
			_currentScreen.Show();
			await ToSignal(_currentScreen.tween, "tween_completed");
		}
	}

	public void GameOver(int score, int highScore, int level)
	{
		scoreLabel.Text = $"Score: {score}";
		highScoreLabel.Text = $"Best: {highScore}";
		levelLabel.Text = $"Best Level: {level}";
		ChangeScreen(GetNode<BaseScreen>("GameOverScreen"));
	}
}

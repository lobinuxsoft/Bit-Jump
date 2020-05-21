using System;
using Godot;
using Godot.Collections;

public class ScreensManager : Node
{
	[Signal] public delegate void OnStartGame();
	
	private BaseScreen _currentScreen = null;

	private Settings _settings;

	private AudioStreamPlayer _audioStreamPlayer;

	private TextureButton musicButton;
	[Export] private Dictionary<bool, Texture> musicTexture;
	private TextureButton soundFXButton;
	[Export] private Dictionary<bool, Texture> soundTexture;

	public override void _Ready()
	{
		RegisterButtons();
		ChangeScreen(GetNode<BaseScreen>("TitleScreen"));

		_settings = GetTree().Root.GetNode<Settings>("Settings");
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

		if (_settings.enableSound)
		{
			_audioStreamPlayer.Play();
		}
	}

	public async void ToPlay()
	{
		ChangeScreen(null);
		await ToSignal(GetTree().CreateTimer(.5f), "timeout");
		EmitSignal(nameof(OnStartGame));
		
		if (_settings.enableSound)
		{
			_audioStreamPlayer.Play();
		}
	}

	public void ToSetting()
	{
		ChangeScreen(GetNode<BaseScreen>("SettingScreen"));
		
		if (_settings.enableSound)
		{
			_audioStreamPlayer.Play();
		}
	}

	public void Music()
	{
		_settings.enableMusic = !_settings.enableMusic;
		musicButton.TextureNormal = musicTexture[_settings.enableMusic];
		
		if (_settings.enableSound)
		{
			_audioStreamPlayer.Play();
		}
	}

	public void SoundFX()
	{
		_settings.enableSound = !_settings.enableSound;
		soundFXButton.TextureNormal = soundTexture[_settings.enableSound];
		
		if (_settings.enableSound)
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

	public void GameOver()
	{
		ChangeScreen(GetNode<BaseScreen>("GameOverScreen"));
	}
}

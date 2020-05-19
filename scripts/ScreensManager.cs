using System;
using Godot;
using Godot.Collections;

public class ScreensManager : Node
{
	[Signal] public delegate void OnStartGame();
	
	private BaseScreen currentScreen = null;

	public override void _Ready()
	{
		RegisterButtons();
		ChangeScreen(GetNode<BaseScreen>("TitleScreen"));
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
				}
				// but.Connect("pressed", this, nameof(OnButtonPressed), new Array(but.Name));
			}
		}
	}

	public void ToHome()
	{
		ChangeScreen(GetNode<BaseScreen>("TitleScreen"));
	}

	public async void ToPlay()
	{
		ChangeScreen(null);
		await ToSignal(GetTree().CreateTimer(.5f), "timeout");
		EmitSignal(nameof(OnStartGame));
	}

	public void ToSetting()
	{
		ChangeScreen(GetNode<BaseScreen>("SettingScreen"));
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
		if (currentScreen != null)
		{
			currentScreen.Hide();
			await ToSignal(currentScreen.tween, "tween_completed");
			
		}
		
		currentScreen = screen;
		
		if (screen != null)
		{
			currentScreen.Show();
			await ToSignal(currentScreen.tween, "tween_completed");
		}
	}

	public void GameOver()
	{
		ChangeScreen(GetNode<BaseScreen>("GameOverScreen"));
	}
}

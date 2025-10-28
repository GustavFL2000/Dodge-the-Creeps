using Godot;
using System;

public partial class Hud : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();

	public override void _Ready()
	{
		// Forbind signaler i kode i stedet for editoren
		GetNode<Button>("StartButton").Pressed += OnStartButtonPressed;
		GetNode<Timer>("MessageTimer").Timeout += OnMessageTimerTimeout;
	}

	public void ShowMessage(string text)
	{
		var message = GetNode<Label>("Message");
		message.Text = text;
		message.Show();

		GetNode<Timer>("MessageTimer").Start();
	}

	public async void ShowGameOver()
	{
		ShowMessage("Game Over");

		var messageTimer = GetNode<Timer>("MessageTimer");
		await ToSignal(messageTimer, Timer.SignalName.Timeout);

		var message = GetNode<Label>("Message");
		message.Text = "Dodge the Creeps!";
		message.Show();

		// Brug SceneTree's CreateTimer i stedet for en separat Timer-node
		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);

		GetNode<Button>("StartButton").Show();
	}

	    public void UpdateScore(int score)
	    {
	        GetNode<Label>("ScoreLabel").Text = score.ToString();
	    }
	
	    public void UpdateCoins(int coins)
	    {
	        GetNode<Label>("CoinsLabel").Text = coins.ToString();
	    }
	
	    private void OnStartButtonPressed()	{
		GetNode<Button>("StartButton").Hide();
		EmitSignal(SignalName.StartGame);
	}

	private void OnMessageTimerTimeout()
	{
		GetNode<Label>("Message").Hide();
	}
}

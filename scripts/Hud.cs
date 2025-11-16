using Godot;
using System;

public partial class Hud : CanvasLayer
{
	[Signal]
	public delegate void StartGameEventHandler();
	[Signal]
	public delegate void OpenShopEventHandler();


	public override void _Ready()
	{
		// Forbind signaler i kode i stedet for editoren
		GetNode<Button>("MenuButtons/StartButton").Pressed += OnStartButtonPressed;
		GetNode<Button>("MenuButtons/ShopButton").Pressed += OnShopButtonPressed;
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

		GetNode<Control>("MenuButtons").Show();
	}

	    public void UpdateScore(int score)
	    {
	        GetNode<Label>("ScoreLabel").Text = score.ToString();
	    }
	
	    public void UpdateCoins(int coins)
	    {
	        GetNode<Label>("CoinsLabel").Text = coins.ToString();
	    }

	    public void UpdateLives(int lives)
	    {
	        GetNode<Label>("LivesLabel").Text = $"❤️ {lives}";
	    }
	
	    private void OnStartButtonPressed()	{
		GetNode<Control>("MenuButtons").Hide();
		EmitSignal(SignalName.StartGame);
	}

	private void OnShopButtonPressed()
	{
		GetNode<Control>("MenuButtons").Hide();
		EmitSignal(SignalName.OpenShop);
	}

	private void OnMessageTimerTimeout()
	{
		GetNode<Label>("Message").Hide();
	}
}

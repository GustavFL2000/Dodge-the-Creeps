using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    [Export]
    public PackedScene CoinScene { get; set; }

    private int _score;
    private int _coins;
    private int _lives = 1;
    private Hud _hud;
    private Player _player;
    private ShopMenu _shopMenu;

    public override void _Ready()
    {
        _player = GetNode<Player>("Player");
        _player.Hit += GameOver;

        _hud = GetNode<Hud>("Hud");
        _hud.StartGame += NewGame;
        _hud.OpenShop += OnOpenShop;

        GetNode<Timer>("MobTimer").Timeout += OnMobTimerTimeout;
        GetNode<Timer>("ScoreTimer").Timeout += OnScoreTimerTimeout;
        GetNode<Timer>("StartTimer").Timeout += OnStartTimerTimeout;
        GetNode<Timer>("CoinTimer").Timeout += OnCoinTimerTimeout;
    }

    private void OnOpenShop()
    {
        var shopScene = GD.Load<PackedScene>("res://scenes/ShopMenu.tscn");
        _shopMenu = shopScene.Instantiate<ShopMenu>();
        _shopMenu.CurrentCoins = _coins;
        _shopMenu.Items = new System.Collections.Generic.List<ShopItem>
        {
            new ShopItem("Speed Boost", 5),
            new ShopItem("Shield", 10),
            new ShopItem("Extra Life", 20)
        };
        _shopMenu.CloseShop += OnShopClosed;
        _shopMenu.ItemPurchased += OnItemPurchased;
        AddChild(_shopMenu);
    }

    private void OnShopClosed()
    {
        _shopMenu.QueueFree();
        _hud.GetNode<Control>("MenuButtons").Show();
    }

    private void OnItemPurchased(string itemName, int price)
    {
        if (_coins >= price)
        {
            _coins -= price;
            _hud.UpdateCoins(_coins);
            _shopMenu.Refresh(_coins); // Refresh shop UI with new coin total

            GD.Print($"Purchased {itemName} for {price} coins.");

            // Apply upgrade
            if (itemName == "Speed Boost")
            {
                _player.Speed += 50;
            }
            else if (itemName == "Shield")
            {
                _player.ActivateShield();
            }
            else if (itemName == "Extra Life")
            {
                _lives++;
                _hud.UpdateLives(_lives);
            }
        }
    }

    public void NewGame()
    {
        _score = 0;
        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
        GetTree().CallGroup("coins", Node.MethodName.QueueFree);

        var startPosition = GetNode<Marker2D>("StartPosition");
        _player.Start(startPosition.Position);

        _hud.UpdateScore(_score);
        _hud.UpdateCoins(_coins);
        _hud.UpdateLives(_lives);
        _hud.ShowMessage("Get Ready!");

        GetNode<Timer>("StartTimer").Start();
        GetNode<AudioStreamPlayer2D>("Music").Play();
    }

    private async void GameOver()
    {
        _lives--;
        _hud.UpdateLives(_lives);

        if (_lives > 0)
        {
            // Respawn
            GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
            await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
            var startPosition = GetNode<Marker2D>("StartPosition");
            _player.Start(startPosition.Position);
        }
        else
        {
            // Real Game Over
            _hud.ShowGameOver();
            GetNode<Timer>("MobTimer").Stop();
            GetNode<Timer>("ScoreTimer").Stop();
            GetNode<Timer>("CoinTimer").Stop();
            GetTree().CallGroup("coins", Node.MethodName.QueueFree);
            GetNode<AudioStreamPlayer2D>("Music").Stop();
            GetNode<AudioStreamPlayer2D>("DeathSound").Play();
            _lives = 1; // Reset lives for next time
        }
    }

    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
        GetNode<Timer>("CoinTimer").Start();
    }

    private void OnScoreTimerTimeout()
    {
        _score++;
        _hud.UpdateScore(_score);
    }

    private void OnMobTimerTimeout()
    {
        Mob mob = MobScene.Instantiate<Mob>();
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;
        mob.Position = mobSpawnLocation.Position;
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);
        AddChild(mob);
    }

    private const float MIN_COIN_DISTANCE = 5.0f;

    private void OnCoinTimerTimeout()
    {
        Coin coin = CoinScene.Instantiate<Coin>();
        Vector2 spawnPosition = Vector2.Zero;
        bool foundSafePosition = false;
        int attempts = 0;
        const int MAX_ATTEMPTS = 50;

        while (!foundSafePosition && attempts < MAX_ATTEMPTS)
        {
            spawnPosition = new Vector2(
                (float)GD.RandRange(0, GetViewport().GetVisibleRect().Size.X),
                (float)GD.RandRange(80.0, GetViewport().GetVisibleRect().Size.Y)
            );
            foundSafePosition = true;
            foreach (Node node in GetTree().GetNodesInGroup("coins"))
            {
                if (node is Coin existingCoin)
                {
                    if (spawnPosition.DistanceTo(existingCoin.Position) < MIN_COIN_DISTANCE)
                    {
                        foundSafePosition = false;
                        break;
                    }
                }
            }
            attempts++;
        }

        if (foundSafePosition)
        {
            coin.Position = spawnPosition;
            AddChild(coin);
            coin.PickedUp += OnCoinPickedUp;
        }
        else
        {
            coin.QueueFree();
        }
    }

    private void OnCoinPickedUp()
    {
        _coins++;
        _hud.UpdateCoins(_coins);
    }
}

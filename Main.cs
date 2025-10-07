using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; } // Reference til mob scenen (drag i editor)

    private int _score;
    private Hud _hud;
    private Player _player;

    public override void _Ready()
    {
        // Player signal
        _player = GetNode<Player>("Player");
        _player.Hit += GameOver; // Når player rammes → GameOver()

        // HUD reference
        _hud = GetNode<Hud>("Hud");
        _hud.StartGame += NewGame;

        // Timer signals
        GetNode<Timer>("MobTimer").Timeout += OnMobTimerTimeout;
        GetNode<Timer>("ScoreTimer").Timeout += OnScoreTimerTimeout;
        GetNode<Timer>("StartTimer").Timeout += OnStartTimerTimeout;

        // Start et nyt spil automatisk (hvis du vil teste uden startknap)
        // NewGame(); <----
        // Tomt i GD script ville man skrive "pass" i stedet for NewGame(); 
        // for ikke at starte spillet automatisk
    }

    public void NewGame()
    {
        _score = 0;

        // Note that for calling Godot-provided methods with strings,
        // we have to use the original Godot snake_case name.
        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);

        // Flyt player til startposition
        var startPosition = GetNode<Marker2D>("StartPosition");
        _player.Start(startPosition.Position);

        // Reset HUD
        _hud.UpdateScore(_score);
        _hud.ShowMessage("Get Ready!");

        // Start nedtælling før mobs spawner
        GetNode<Timer>("StartTimer").Start();

        //Start musikken
        GetNode<AudioStreamPlayer>("Music").Play();
    }

    private void GameOver()
    {
        // Vis "Game Over" på HUD
        _hud.ShowGameOver();

        // Stop mob og score timers
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();

        // Stop musikken
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
    }

    private void OnStartTimerTimeout()
    {
        // Når start-timeren går → aktiver mobs og scoring
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    private void OnScoreTimerTimeout()
    {
        // Øg score og opdater HUD
        _score++;
        _hud.UpdateScore(_score);
    }

    private void OnMobTimerTimeout()
    {
        // Instantiér en ny mob fra PackedScene
        Mob mob = MobScene.Instantiate<Mob>();

        // Find spawn location (PathFollow2D sørger for random placering langs en path)
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf(); // tilfældig position mellem 0.0 og 1.0 på pathen

        // Bestem retning (rotation + 90 grader for at pege udad)
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;
        mob.Position = mobSpawnLocation.Position;

        // Tilføj lidt random variation til retningen
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        // Tilføj random hastighed og roter den i mobbens retning
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

        // Tilføj mob til scenen
        AddChild(mob);
    }
}

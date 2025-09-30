using Godot;
using System;

public partial class Mob : RigidBody2D
{
    public override void _Ready()
    {
        // Hent AnimatedSprite2D og vælg en tilfældig animation
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames();
        animatedSprite2D.Play(mobTypes[GD.Randi() % mobTypes.Length]);

        // Hent VisibleOnScreenNotifier2D og tilknyt signalet via kode
        var notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        notifier.ScreenExited += OnVisibleOnScreenNotifier2DScreenExited;
    }

    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
    }

    public override void _Process(double delta)
    {
        // Hvis du får brug for logik pr. frame, kan du tilføje det her
    }
}

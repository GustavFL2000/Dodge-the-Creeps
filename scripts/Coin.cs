
using Godot;
using System;

public partial class Coin : Area2D
{
    [Signal]
    public delegate void PickedUpEventHandler();

    public override void _Ready()
    {
        AddToGroup("coins");
        AreaEntered += OnAreaEntered;

        var lifeTimer = GetNode<Timer>("LifeTimer");
        lifeTimer.Timeout += OnLifeTimerTimeout;
        lifeTimer.Start();
    }

    private void OnLifeTimerTimeout()
    {
        QueueFree();
    }

    private void OnAreaEntered(Area2D area)
    {
        if (area is Player)
        {
            EmitSignal(SignalName.PickedUp);
            QueueFree();
        }
    }
}

using Godot;
using System;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();

	[Export]
	public int Speed { get; set; } = 400; // Speed of the player

	public Vector2 ScreenSize; // Size of the game window
	public bool HasShield { get; private set; } = false;

	private AnimatedSprite2D _animatedSprite;

	public override void _Ready()
	{
		ScreenSize = GetViewport().GetVisibleRect().Size;
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Hide();

		BodyEntered += OnBodyEntered;
	}

	public void ActivateShield()
	{
		HasShield = true;
		_animatedSprite.Modulate = new Color(0.5f, 0.7f, 1f); // Light blue tint
	}

	private void DeactivateShield()
	{
		HasShield = false;
		_animatedSprite.Modulate = new Color(1f, 1f, 1f); // Reset to normal color
	}

	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
		DeactivateShield(); // Ensure shield is off at the start of a game
	}

	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}
		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}
		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			_animatedSprite.Play();
		}
		else
		{
			_animatedSprite.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		if (velocity.X != 0)
		{
			_animatedSprite.Animation = "walk";
			_animatedSprite.FlipV = false;
			_animatedSprite.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0)
		{
			_animatedSprite.Animation = "up";
			_animatedSprite.FlipV = velocity.Y > 0;
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (HasShield)
		{
			DeactivateShield();
			// We could also make the mob that hit the shield disappear
			if (body.IsInGroup("mobs"))
			{
				body.QueueFree();
			}
			return; // Don't die
		}

		Hide();
		EmitSignal(SignalName.Hit);
		GetNode<CollisionShape2D>("CollisionShape2D")
			.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
}

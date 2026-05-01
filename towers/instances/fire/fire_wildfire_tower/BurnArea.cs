using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class BurnArea : Area2D
{
	private readonly List<Node2D> _enemies = new();

	[Export] public float duration = 0.3f;
	private Source _source;

	private Timer _durationTimer;
	private GpuParticles2D _explosionParticles;
	private CpuParticles2D _cpuExplosion;

	public override void _Ready()
	{
		_durationTimer = GetNode<Timer>("DurationTimer");
		_explosionParticles = GetNode<GpuParticles2D>("ExplosionParticles");
		_cpuExplosion = GetNode<CpuParticles2D>("CPUExplosion");

		_durationTimer.WaitTime = duration;
		Monitoring = false;
	}

	public void Setup(Source burnSource)
	{
		_source = burnSource;
		_durationTimer.Start();
		Monitoring = true;

		_explosionParticles.Restart();
		_explosionParticles.Emitting = true;
		_cpuExplosion.Restart();
		_cpuExplosion.Emitting = true;
	}

	private void OnBodyEntered(Node2D body)
	{
		_enemies.Add(body);
		body.TreeExited += () => _enemies.Remove(body);

		if (!GodotObject.IsInstanceValid(body))
		{
			return;
		}

		EnemyDebuff debuff = _source != null ? EnemyDebuff.CreateBurn(_source) : null;
		if (debuff != null)
		{
			Enemy enemy = body as Enemy;
			enemy?.ApplyDebuff(debuff);
		}
	}

	private void OnDurationTimerTimeout()
	{
		QueueFree();
	}
}
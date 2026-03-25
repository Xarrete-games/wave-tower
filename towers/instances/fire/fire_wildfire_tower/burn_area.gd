class_name BurnArea extends Area2D

var enemies_burned: Array[Enemy] = []
var source: Source

@onready var duration_timer: Timer = $DurationTimer
@onready var explosion_particles: GPUParticles2D = $ExplosionParticles
@onready var cpu_explosion: CPUParticles2D = $CPUExplosion
@onready var animation_player: AnimationPlayer = $AnimationPlayer

func _ready() -> void:
	monitoring = false

func setup(p_source: Source) -> void:
	source = p_source
	
	explosion_particles.emitting = true
	cpu_explosion.emitting = true
	animation_player.play("explosion")
	
	duration_timer.start()
	monitoring = true

func _on_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	if enemies_burned.has(enemy):
		return
	enemy.apply_debuff(EnemyDebuff.create_burn(source))
	enemies_burned.append(enemy)

func _on_duration_timer_timeout() -> void:
	queue_free()

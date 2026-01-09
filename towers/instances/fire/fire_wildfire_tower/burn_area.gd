class_name BurnArea extends Area2D

var enemies_burned: Array[Enemy] = []
var source: Object

@onready var duration_timer: Timer = $DurationTimer

func _ready() -> void:
	monitoring = false

func setup(p_source: Object) -> void:
	source = p_source
	duration_timer.start()
	monitoring = true

func _on_body_entered(body: Node2D) -> void:
	var enemy = body as Enemy
	if enemies_burned.has(enemy):
		return
	enemy.apply_debuff(RunContext.enemy_debuff.get_debuff(EnemyDebuff.Type.BURN, source))
	enemies_burned.append(enemy)

func _on_duration_timer_timeout() -> void:
	queue_free()
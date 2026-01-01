class_name SingleTargetProjectile
extends Node2D

const SPEED: float = 400.0
const HIT_RADIUS: float = 12.0

var enemy: Enemy
var attack: Attack
var debuff: EnemyDebuff

func _process(delta: float) -> void:
	if not is_instance_valid(enemy):
		queue_free()
		return
	
	var target_position: Vector2 = enemy.target_position

	var direction: Vector2 = (target_position - global_position)
	var distance: float = direction.length()

	if distance <= HIT_RADIUS:
		enemy.apply_damage(attack)
		if debuff:
			enemy.apply_debuff(debuff)
		queue_free()
		return

	global_position += direction.normalized() * SPEED * delta


func set_target(p_enemy: Enemy, p_attack: Attack, p_debuff: EnemyDebuff = null) -> void:
	enemy = p_enemy
	attack = p_attack
	debuff = p_debuff

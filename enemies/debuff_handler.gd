class_name DebuffHandler extends Node

const BURN_COLOR =  Color.DARK_ORANGE
const FROST_COLOR = Color.AQUA
const DEFAULT_COLOR = Color.WHITE

var debuffs: Array[EnemyDebuff] = []
var debuffs_stacks: Dictionary[EnemyDebuff.Type, int] = {
	EnemyDebuff.Type.BURN: 0,
	EnemyDebuff.Type.FROST: 0,
}

func add_debuff(debuff: EnemyDebuff, amount: int, enemy: Enemy):
	if debuff.value == 0:
		return
	for i in range(amount):
		if debuffs_stacks[debuff.type] >= debuff.max_stacks:
			return

		debuffs.append(debuff)
		_on_add_debuff(debuff.type, enemy)
		# on apply
	debuff.on_apply(enemy)

func update_all(enemy: Enemy, delta: float): 
	for i in range(debuffs.size() - 1, -1, -1):
		var debuff = debuffs[i]
		debuff.duration = max(0, debuff.duration - delta)
		# on tick
		if debuff.tick_duration > 0:
			debuff.time_to_tick -= delta
			
			if debuff.time_to_tick <= 0:
				debuff.on_tick(enemy)
				debuff.time_to_tick += debuff.tick_duration

		# on update
		debuff.on_update(enemy, delta)

		# expiration
		if debuff.duration <= 0:
			debuff.on_expire(enemy)
			debuffs.remove_at(i)
			_on_remove_debuff(debuff.type, enemy)

func get_stacks(debuff_type: EnemyDebuff.Type) -> int:
	return debuffs_stacks[debuff_type]

func _on_add_debuff(type: EnemyDebuff.Type, enemy: Enemy) -> void:
	debuffs_stacks[type] += 1
	enemy.health_bar.set_debuffs(debuffs_stacks)
	match (type):
		EnemyDebuff.Type.FROST:
			enemy.default_modulate_color = FROST_COLOR
		EnemyDebuff.Type.BURN:
			enemy.default_modulate_color = BURN_COLOR
	enemy.update_visual_color()
			
func _on_remove_debuff(type: EnemyDebuff.Type, enemy: Enemy) -> void:
	debuffs_stacks[type] -= 1
	enemy.health_bar.set_debuffs(debuffs_stacks)
	if debuffs_stacks[EnemyDebuff.Type.FROST] > 0:
		enemy.default_modulate_color = FROST_COLOR
	elif debuffs_stacks[EnemyDebuff.Type.BURN] > 0:
		enemy.default_modulate_color = BURN_COLOR
	else:
		enemy.default_modulate_color = DEFAULT_COLOR
	enemy.update_visual_color()

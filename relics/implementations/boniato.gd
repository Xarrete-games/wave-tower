class_name Boniato extends Relic

func on_enemy_die(enemy: Enemy, attack: Attack) -> void:
	enemy.gold_value += 1

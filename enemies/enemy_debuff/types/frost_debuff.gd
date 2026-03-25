class_name FrostDebuff extends EnemyDebuff



func on_apply(enemy: Enemy):
	enemy.speed_mult -= value / 100

func on_expire(enemy: Enemy):
	enemy.speed_mult += value / 100

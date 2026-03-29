class_name BurnDebuff extends EnemyDebuff

func on_tick(enemy: Enemy):
	var debuff_source = Source.new(Source.SourceType.DEBUFF, data.id, self, source)
	var attack = Attack.new(value, debuff_source)
	enemy.apply_damage(attack)

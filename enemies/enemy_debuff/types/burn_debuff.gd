class_name BurnDebuff extends EnemyDebuff

const SOURCE_TYPE_DEBUFF: int = 3

func on_tick(enemy: Enemy):
	var debuff_source = Source.new()
	debuff_source.setup(SOURCE_TYPE_DEBUFF, data.id, self, source)
	var attack = Attack.new(value, debuff_source)
	enemy.apply_damage(attack)

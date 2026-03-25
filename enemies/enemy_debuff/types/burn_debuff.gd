class_name BurnDebuff extends EnemyDebuff


func on_tick(enemy: Enemy):
	var debuff_source = Source.new(Source.SourceType.DEBUFF, "burn_debuff")
	var attack = Attack.new(value, DamageNumbers.Type.SKILL, debuff_source, source)
	enemy.apply_damage(attack)

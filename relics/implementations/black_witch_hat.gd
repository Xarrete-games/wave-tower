class_name BlackWitchHat extends Relic

const BONUS_DAMAGE: float = 5.0

func modify_damage_additive(amount: float, attack: Attack, target: Enemy) -> float:
	if target.has_any_debuff() and attack.source.type == Source.SourceType.TOWER:
		return amount + BONUS_DAMAGE
	return amount

class_name BigBowlOfMilkAndBiscuits extends Relic

func apply_effect() -> void:
	RunContext.status.max_health += 10
	RunContext.status.health += RunContext.status.max_health

func remove_effect() -> void:
	RunContext.status.max_health -= 10
	

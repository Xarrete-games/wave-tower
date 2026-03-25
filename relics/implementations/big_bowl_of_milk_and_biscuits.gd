class_name BigBowlOfMilkAndBiscuits extends Relic

func on_obtain() -> void:
	RunContext.status.max_health += 10
	RunContext.status.health += RunContext.status.max_health


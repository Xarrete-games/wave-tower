class_name CaptainCap extends Relic

func on_before_relic_reward(ctx: RelicsRewardsContext) -> void:
	ctx.number_of_relics += 1
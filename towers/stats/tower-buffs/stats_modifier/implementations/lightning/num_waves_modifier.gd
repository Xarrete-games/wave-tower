class_name NumWavesModifier extends TowerStatsModifier


func contribute(acc: TowerStatsAccumulator) -> void:
	acc.flat_extra_hits += int(value)

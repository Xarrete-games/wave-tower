class_name NumWavesModifier extends TowerBuffModifier


func contribute(acc: TowerStatsAccumulator) -> void:
	acc.flat_extra_hits += int(value)

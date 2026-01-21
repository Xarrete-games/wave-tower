class_name DamageRecount extends RefCounted

signal damage_recount_change(data: DamageRecountData)

var damage_per_wave: Dictionary[int, Dictionary] = {}

func record_damage(wave: int, id: String, damage: float) -> void:
	if not damage_per_wave.has(wave):
		damage_per_wave[wave] = {}
	if not damage_per_wave[wave].has(id):
		damage_per_wave[wave][id] = DamageRecountData.new()
		damage_per_wave[wave][id].id = id
		damage_per_wave[wave][id].wave = wave
	damage_per_wave[wave][id].damage += damage
	damage_recount_change.emit(damage_per_wave[wave][id])
class_name EnemyDebuffManager extends RefCounted

signal debuff_change(enemy_debuff: EnemyDebuff)

var debuff_templates: Dictionary[EnemyDebuff.Type, EnemyDebuff] = {}
var debuff_data_by_type: Dictionary[EnemyDebuff.Type, EnemyDebuffData] = {}

func _init() -> void:
	for data in DataLoader.get_all_enemy_debuffs():
		register_debuff_data(data)

func register_debuff(type: EnemyDebuff.Type, template: EnemyDebuff) -> void:
	debuff_templates[type] = template
	template.changed.connect(func(): debuff_change.emit(template))

func register_debuff_data(data: EnemyDebuffData) -> void:
	if data == null:
		return

	var template := data.create_item() as EnemyDebuff
	if template == null:
		push_error("[EnemyDebuffManager] Failed creating debuff template from data id: %s" % data.id)
		return

	register_debuff(data.debuff_type, template)
	debuff_data_by_type[data.debuff_type] = data

func get_debuff(type: EnemyDebuff.Type, source: Source) -> EnemyDebuff:
	if debuff_templates.has(type):
		return debuff_templates[type].clone(source)

	push_error("[EnemyDebuffManager] invalid get debuff for type: %s" % type)
	return null

func get_template(type: EnemyDebuff.Type) -> EnemyDebuff:
	return debuff_templates.get(type, null)

func get_data(type: EnemyDebuff.Type) -> EnemyDebuffData:
	return debuff_data_by_type.get(type, null)

class_name EnemyDebuffManager extends RefCounted

signal debuff_change(enemy_debuff: EnemyDebuff)
signal modifier_change(modifiers: Array[DamageTakenModifier])

var debuff_templates: Dictionary[EnemyDebuff.Type, EnemyDebuff] = {}
var debuff_data_by_type: Dictionary[EnemyDebuff.Type, EnemyDebuffData] = {}

var modifier_instances: Array[DamageTakenModifier] = []

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

func add_modifier_from_data(data_id: String) -> void:
	var data := DataLoader.get_modifier_by_id(data_id)
	if data == null:
		push_error("[EnemyDebuffManager] No modifier data found for id: %s" % data_id)
		return

	if has_modifier_data(data_id):
		push_warning("[EnemyDebuffManager] Modifier already active: %s" % data_id)
		return

	var source := Source.new(Source.SourceType.RELIC, data_id)
	var modifier := data.create_item(source) as DamageTakenModifier
	if modifier == null:
		push_error("[EnemyDebuffManager] Failed creating modifier from data id: %s" % data_id)
		return

	modifier_instances.append(modifier)
	modifier_change.emit(modifier_instances.duplicate())

func remove_modifier_from_data(data_id: String) -> void:
	for i in range(modifier_instances.size()):
		if modifier_instances[i].data.id == data_id:
			modifier_instances.remove_at(i)
			modifier_change.emit(modifier_instances.duplicate())
			return

func has_modifier_data(data_id: String) -> bool:
	for modifier in modifier_instances:
		if modifier.data.id == data_id:
			return true
	return false

func get_modifiers() -> Array[DamageTakenModifier]:
	return modifier_instances.duplicate()

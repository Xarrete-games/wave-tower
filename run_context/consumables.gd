class_name ConsumablesManager extends RefCounted

signal consumables_change(consumables: Array[Consumable])
signal consumable_added(consumable: Consumable)
signal consumable_used(consumable: Consumable)
signal consumable_clicked(consumable: Consumable)

var consumables: Array[Consumable] = []

func is_full() -> bool:
    return consumables.size() == 5

func add_consumable(consumable: Consumable) -> void:
    if is_full():
        return

    consumables.append(consumable)
    consumable.clicked.connect(_on_consumable_clicked)
    consumable.used.connect(_on_consumable_used)
    consumables_change.emit(consumables)
    consumable_added.emit(consumable)

func _on_consumable_used(consumable: Consumable) -> void:
    consumable_used.emit(consumable)
    consumables.erase(consumable)
    consumables_change.emit(consumables)

func _on_consumable_clicked(consumable: Consumable) -> void:
    if consumable is ConsumableUsable:
        consumable.use()
        consumable.used.emit(consumable)
    else:
        consumable_clicked.emit(consumable)
    
    


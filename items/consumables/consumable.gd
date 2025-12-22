class_name Consumable extends GameItem

signal used(consumable: Consumable)
signal clicked(consumable: Consumable)

@export var cursor_icon: Texture2D
@export var cursor_icon_used: Texture2D
@export var use_sound: AudioStream
@export var instant: bool = true

func _init(data: ItemData) -> void:
    super(data)
    type = ItemData.Type.CONSUMABLE
    cursor_icon = data.icon_48
    cursor_icon_used = data.icon_48_used



class_name Consumable extends GameItem

signal used(consumable: Consumable)
signal clicked(consumable: Consumable)

var cursor_icon: Texture2D
var cursor_icon_used: Texture2D
var use_sound: AudioStream
var targeting_type: ConsumableTargeteable.TargetType


func _init(data: ConsumableItemData) -> void:
    super(data)
    type = ItemData.Type.CONSUMABLE
    cursor_icon = data.cursor_icon
    cursor_icon_used = data.cursor_icon_used
    use_sound = data.use_sound
    targeting_type = data.targeting_type

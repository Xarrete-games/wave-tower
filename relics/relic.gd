class_name Relic extends AbstractModel

signal changed(relic: Relic)

var data
var disabled: bool = false:
	set(value):
		disabled = value
		changed.emit(self)

var counter: int = 0:
	set(value):
		counter = value
		changed.emit(self)

var id: String:
	get:
		if data == null:
			return ""
		return data.id

func get_source() -> Source:
	return Source.new(Source.SourceType.RELIC, id, self)

func on_obtain() -> void:
	pass

func on_remove() -> void:
	pass

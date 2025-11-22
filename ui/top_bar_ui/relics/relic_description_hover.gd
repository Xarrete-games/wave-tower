class_name RelicDescriptionHover extends CanvasLayer

@onready var description_label: Label = $PanelContainer/DescriptionLabel

func set_description(new_text: String) -> void:
	description_label.text = new_text

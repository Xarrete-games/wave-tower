class_name RelicDescriptionHover extends PanelContainer

@onready var description_label: Label = $DescriptionLabel

func set_description(new_text: String) -> void:
	description_label.text = new_text

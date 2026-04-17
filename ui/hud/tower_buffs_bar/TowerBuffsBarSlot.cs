using Godot;
public partial class TowerBuffsBarSlot : Control {
    [Export] public NodePath texture;
    [Export] public NodePath value_label;
    public TowerBuff tower_buff;
    private int _value = 0;
    public int value {
        get => _value;
        set {
            _value = value;
            if (_valueLabelNode == null) {
                return;
            }
            _valueLabelNode.Text = _value != 0 ? _value.ToString() : string.Empty;
        }
    }
    private TextureRect _textureNode;
    private Label _valueLabelNode;
    public override void _Ready() {
        ResolveNodes();
        if (tower_buff != null) {
            ApplyBuffVisuals();
        }
        value = _value;
    }
    public void set_buff(TowerBuff towerBuff, int stackValue = 0) {
        tower_buff = towerBuff;
        value = stackValue;
        ApplyBuffVisuals();
    }
    private void ResolveNodes() {
        if (_textureNode == null) {
            _textureNode = !texture.IsEmpty ? GetNodeOrNull<TextureRect>(texture) : GetNodeOrNull<TextureRect>("Texture");
        }
        if (_valueLabelNode == null) {
            _valueLabelNode = !value_label.IsEmpty ? GetNodeOrNull<Label>(value_label) : GetNodeOrNull<Label>("Label");
        }
    }
    private void ApplyBuffVisuals() {
        ResolveNodes();
        if (_textureNode == null || tower_buff == null) {
            return;
        }
        BuffData data = tower_buff.data;
        if (data == null) {
            return;
        }
        _textureNode.Texture = data.icon;
    }
}


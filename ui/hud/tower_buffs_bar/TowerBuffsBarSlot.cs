using Godot;
public partial class TowerBuffsBarSlot : Control {
    [Export] public NodePath Texture;
    [Export] public NodePath ValueLabel;
    public TowerBuff TowerBuff;
    private int _value = 0;
    public int Value {
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
        if (TowerBuff != null) {
            ApplyBuffVisuals();
        }
        Value = _value;
    }
    public void SetBuff(TowerBuff towerBuff, int stackValue = 0) {
        TowerBuff = towerBuff;
        Value = stackValue;
        ApplyBuffVisuals();
    }
    private void ResolveNodes() {
        bool hasTexturePath = Texture != null && !Texture.IsEmpty;
        bool hasValueLabelPath = ValueLabel != null && !ValueLabel.IsEmpty;

        if (_textureNode == null) {
            _textureNode = hasTexturePath ? GetNodeOrNull<TextureRect>(Texture) : GetNodeOrNull<TextureRect>("Texture");
        }
        if (_valueLabelNode == null) {
            _valueLabelNode = hasValueLabelPath ? GetNodeOrNull<Label>(ValueLabel) : GetNodeOrNull<Label>("Label");
        }
    }
    private void ApplyBuffVisuals() {
        ResolveNodes();
        if (_textureNode == null || TowerBuff == null) {
            return;
        }
        BuffData data = TowerBuff.Data;
        if (data == null) {
            return;
        }
        _textureNode.Texture = data.Icon;
    }
}


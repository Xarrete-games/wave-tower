using Godot;

public partial class ShopSlotIcon : SubViewportContainer
{
    private Node2D _root2d;
    private Sprite2D _relicTexture;
    private Polygon2D _hexagon;

    public override void _Ready()
    {
        _root2d = GetNode<Node2D>("SubViewport/Root2D");
        _relicTexture = GetNode<Sprite2D>("SubViewport/Root2D/RelicTexture");
        _hexagon = GetNode<Polygon2D>("SubViewport/Root2D/Hexagon");

        _root2d.Position = _root2d.GetViewport().GetVisibleRect().Size * 0.5f;
        SetIconNormalSize();
    }

    public void SetIcon(Texture2D texture)
    {
        _relicTexture.Texture = texture;
    }

    public void SetBackgroundColor(Color color)
    {
        _hexagon.Color = color;
    }

    public void IncreaseIconSize()
    {
        SetSpritePixelSize(_relicTexture, new Vector2(80, 80));
    }

    public void SetIconNormalSize()
    {
        SetSpritePixelSize(_relicTexture, new Vector2(64, 64));
    }

    private void SetSpritePixelSize(Sprite2D sprite, Vector2 targetSize)
    {
        if (sprite.Texture == null)
        {
            return;
        }

        Vector2 texSize = sprite.Texture.GetSize();
        sprite.Scale = targetSize / texSize;
    }
}

using Godot;

public partial class ShopSlotIcon : SubViewportContainer
{
    private Node2D _root2d;
    private Sprite2D _relicTexture;
    private Polygon2D _hexagon;

    public override void _Ready()
    {
        this._root2d = GetNode<Node2D>("SubViewport/Root2D");
        this._relicTexture = GetNode<Sprite2D>("SubViewport/Root2D/RelicTexture");
        this._hexagon = GetNode<Polygon2D>("SubViewport/Root2D/Hexagon");

        this._root2d.Position = this._root2d.GetViewport().GetVisibleRect().Size * 0.5f;
        this.icon_normal_size();
    }

    public void set_icon(Texture2D texture)
    {
        this._relicTexture.Texture = texture;
    }

    public void set_background_color(Color color)
    {
        this._hexagon.Color = color;
    }

    public void increased_icon_size()
    {
        this.SetSpritePixelSize(this._relicTexture, new Vector2(80, 80));
    }

    public void icon_normal_size()
    {
        this.SetSpritePixelSize(this._relicTexture, new Vector2(64, 64));
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

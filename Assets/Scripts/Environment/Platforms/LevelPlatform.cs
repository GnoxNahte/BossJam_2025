using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class LevelPlatformBase : PlatformBase
{
    [SerializeField] private Vector2  size = Vector2.one * 0.1f;

    private SpriteRenderer _sr;
    private BoxCollider2D _col2d;

    protected override void OnValidate()
    {
        base.OnValidate();
        
        if (_sr == null)
            _sr = GetComponent<SpriteRenderer>();
        if (_col2d == null)
            _col2d = GetComponent<BoxCollider2D>();

        _sr.size = size;

        if (_col2d != null)
        {
            _col2d.size = size;

            Vector2 spritePivot = GetSpritePivot(_sr.sprite);
            _col2d.offset = size * spritePivot;

            if (HasPlatformTypeFlag(Type.OneWay))
            {
                _col2d.size = new Vector2(_col2d.size.x, 0.01f);
                _col2d.offset = new Vector2(_col2d.offset.x, size.y * (spritePivot.y + 0.5f) - _col2d.size.y * 0.5f);
            }
        }
    }

    private Vector2 GetSpritePivot(Sprite sprite)
    {
        Bounds bounds = sprite.bounds;
        var pivotX = bounds.center.x / bounds.extents.x * 0.5f;
        var pivotY = bounds.center.y / bounds.extents.y * 0.5f;

        return new Vector2(pivotX, pivotY);
    }
}
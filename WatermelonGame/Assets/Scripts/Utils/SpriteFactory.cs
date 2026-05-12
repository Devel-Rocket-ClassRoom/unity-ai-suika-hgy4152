using UnityEngine;

public static class SpriteFactory
{
    static Sprite _circle;
    static Sprite _square;

    public static Sprite GetCircle()
    {
        if (_circle != null)
            return _circle;

        const int res = 128;
        float center = res * 0.5f;
        float r = center - 1f;

        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        var pixels = new Color[res * res];
        for (int y = 0; y < res; y++)
        for (int x = 0; x < res; x++)
        {
            float dist = Vector2.Distance(
                new Vector2(x + 0.5f, y + 0.5f),
                new Vector2(center, center)
            );
            pixels[y * res + x] = dist <= r ? Color.white : Color.clear;
        }
        tex.SetPixels(pixels);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;

        _circle = Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
        return _circle;
    }

    public static Sprite GetSquare()
    {
        if (_square != null)
            return _square;
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        _square = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        return _square;
    }
}

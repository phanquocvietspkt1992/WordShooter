using UnityEngine;

public static class WordShooterArt
{
    public static Color SpaceBackground => new Color(0.018f, 0.027f, 0.075f);
    public static Color Plasma => new Color(0.2f, 0.92f, 1f);
    public static Color SpaceText => new Color(0.88f, 0.96f, 1f);

    public static Color PlanetColor(int levelIndex)
    {
        switch (levelIndex)
        {
            case 0: return new Color(0.25f, 0.75f, 1f);
            case 1: return new Color(1f, 0.38f, 0.2f);
            case 2: return new Color(1f, 0.78f, 0.3f);
            case 3: return new Color(0.48f, 0.55f, 1f);
            default: throw new System.ArgumentOutOfRangeException(nameof(levelIndex));
        }
    }

    public static Vector2 StarPosition(int index, Vector2 size, float time)
    {
        var layer = index % 3;
        return new Vector2(Mathf.Repeat(index * 97.37f + layer * 43f, size.x),
            Mathf.Repeat(index * 137.13f + time * (3f + layer * 6f), size.y));
    }

    public static Vector2 GalaxyCenter(Vector2 size, float time)
    {
        return new Vector2(size.x * (0.65f + Mathf.Sin(time * 0.045f) * 0.12f),
            size.y * (0.3f + Mathf.Cos(time * 0.035f) * 0.08f));
    }

    public static Vector2 MeteorPosition(int index, Vector2 size, float time, out float opacity)
    {
        var duration = 2.4f;
        var age = Mathf.Repeat(time + index * 2.1f, 8f + index * 1.3f);
        var progress = Mathf.Clamp01(age / duration);
        opacity = age >= duration ? 0f : Mathf.Sin(progress * Mathf.PI) * 0.75f;
        var start = new Vector2(size.x * (0.1f + index * 0.2f), -90f);
        return start + new Vector2(size.x * 0.7f + 150f, size.y + 220f) * progress;
    }

    public static Texture2D LoadGun()
    {
        return LoadArt("Cannon");
    }

    public static Matrix4x4 RotateAtPivot(Matrix4x4 canvas, Vector2 pivot, float angle)
    {
        return canvas * Matrix4x4.TRS(pivot, Quaternion.Euler(0f, 0f, angle), Vector3.one)
            * Matrix4x4.Translate(-pivot);
    }

    private static Texture2D LoadArt(string name)
    {
        var texture = Resources.Load<Texture2D>("Art/" + name);
        if (texture == null) throw new System.InvalidOperationException("Missing WordShooter art: Art/" + name);
        return texture;
    }

    public static Texture2D CreateRadial(bool ring)
    {
        var texture = CreateTexture(64, ring ? "WordShooter shockwave" : "WordShooter glow");
        for (var y = 0; y < 64; y++)
        {
            for (var x = 0; x < 64; x++)
            {
                var radius = new Vector2(x - 31.5f, y - 31.5f).magnitude / 31.5f;
                var alpha = ring ? Mathf.Clamp01(1f - Mathf.Abs(radius - 0.82f) / 0.09f)
                    : Mathf.Pow(Mathf.Clamp01(1f - radius), 2f);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        texture.Apply(false, true);
        return texture;
    }

    public static Texture2D CreateWordCard()
    {
        var texture = new Texture2D(252, 116, TextureFormat.RGBA32, false)
        {
            name = "WordShooter rounded card",
            hideFlags = HideFlags.HideAndDontSave,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        for (var y = 0; y < texture.height; y++)
        {
            for (var x = 0; x < texture.width; x++)
            {
                var point = new Vector2((x + 0.5f) / 2f, (y + 0.5f) / 2f);
                var distance = RoundedCardDistance(point);
                var color = Color.Lerp(new Color(0.55f, 0.65f, 0.78f), Color.white, point.y / 58f);
                if (distance > -1f) color = Color.white;
                color.a = Mathf.Clamp01(0.5f - distance * 2f);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply(false, true);
        return texture;
    }

    public static float RoundedCardDistance(Vector2 point)
    {
        var offset = point - new Vector2(63f, 29f);
        var edge = new Vector2(Mathf.Abs(offset.x), Mathf.Abs(offset.y)) - new Vector2(51f, 17f);
        return new Vector2(Mathf.Max(edge.x, 0f), Mathf.Max(edge.y, 0f)).magnitude
            + Mathf.Min(Mathf.Max(edge.x, edge.y), 0f) - 12f;
    }

    private static Texture2D CreateTexture(int size, string name)
    {
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            name = name,
            hideFlags = HideFlags.HideAndDontSave,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixels(new Color[size * size]);
        return texture;
    }

}

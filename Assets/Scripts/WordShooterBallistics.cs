using UnityEngine;

public static class WordShooterBallistics
{
    public static Vector2 AimDirection(Vector2 origin, Vector2 target)
    {
        var offset = target - origin;
        var angle = Mathf.Clamp(Mathf.Atan2(offset.x, -offset.y) * Mathf.Rad2Deg, -70f, 70f);
        return new Vector2(Mathf.Sin(angle * Mathf.Deg2Rad), -Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public static bool TryHitWord(Vector2 start, Vector2 end, Rect word, float radius, out float fraction)
    {
        var enter = 0f;
        var exit = 1f;
        var movement = end - start;
        var hitX = ClipAxis(start.x, movement.x, word.xMin - radius, word.xMax + radius, ref enter, ref exit);
        var hitY = ClipAxis(start.y, movement.y, word.yMin - radius, word.yMax + radius, ref enter, ref exit);
        fraction = enter;
        return hitX && hitY;
    }

    private static bool ClipAxis(float start, float movement, float min, float max, ref float enter, ref float exit)
    {
        if (Mathf.Abs(movement) < 0.00001f)
        {
            return start >= min && start <= max;
        }

        var first = (min - start) / movement;
        var second = (max - start) / movement;
        enter = Mathf.Max(enter, Mathf.Min(first, second));
        exit = Mathf.Min(exit, Mathf.Max(first, second));
        return enter <= exit;
    }
}

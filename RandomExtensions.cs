namespace YuYueCheerPlugin;

public static class RandomExtensions
{
    private static int _last;

    public static int NextDistinct(this Random random, int max)
    {
        if (max < 2)
        {
            return random.Next(0, max);
        }
        
        while (true)
        {
            var next = random.Next(0, max);
            if (next == _last) continue;
            _last = next;
            return next;
        }
    }
}
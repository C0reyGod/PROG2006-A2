using System.Collections.Generic;

public static class StoryPages
{
    public const string Home = "Home";
    public const string MoonGazing = "MoonGazing";
    public const string ChasingLight = "ChasingLight";
    public const string LostAndFound = "LostAndFound";
    public const string MoonGoddess = "MoonGoddess";
    public const string Credits = "Credits";
}

public static class StoryProgress
{
    static readonly HashSet<string> Done = new HashSet<string>();

    public static bool IsDone(string pageId)
    {
        return !string.IsNullOrEmpty(pageId) && Done.Contains(pageId);
    }

    public static void Mark(string pageId)
    {
        if (!string.IsNullOrEmpty(pageId))
            Done.Add(pageId);
    }

    public static void ResetAll()
    {
        Done.Clear();
    }
}

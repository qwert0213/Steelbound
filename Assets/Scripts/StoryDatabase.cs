public static class StoryDatabase
{
    public static string[] GetIntroLines(int level)
    {
        switch (level)
        {
            case 1:
                return new string[] { "What is this place?", "How did I get here?" };
            case 2:
                return new string[] { "Ouch, the ground is so cold and slippery!", "The workers probably can help me explain what just happened." };
            case 3:
                return new string[] { "The bunker is so dark, I wonder how others see in here.", "What are these things on the floor?" };
            default:
                return null;
        }
    }

    public static string[] GetOutroLines(int level)
    {
        switch (level)
        {
            case 1:
                return new string[] { "Not again...", "Don't do this to me!" }; 
            case 2:
                return new string[] { "I have to see what's in that bunker." };
            case 3:
                return new string[] { "This must be the secret...", "*Alarm clock ringing*" };
            default:
                return null;
        }
    }
}

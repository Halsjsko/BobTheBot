namespace Commands.Helpers {
    public class HeartLevels
    {
        private static readonly HeartLevel[] heartLevels = { new("💔 `0`", 0), new("❤️ `1`", 10), new("💓 `2`", 20), new("💗 `2`", 35), new("💕 `3`", 50), new("💞 `4`", 65), new("💖 `5`", 80), new("💘 `6`", 90), };

        public static string CalculateHeartLevel(float matchPercent)
        {
            string heartLevel = "";
            foreach (HeartLevel level in heartLevels)
            {
                if (matchPercent >= level.min)
                {
                    heartLevel = level.heart;
                }
                else
                {
                    break;
                }
            }

            return heartLevel;
        }
    }
}
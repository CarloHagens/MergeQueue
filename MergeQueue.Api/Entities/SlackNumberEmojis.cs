namespace MergeQueue.Api.Entities
{
    public static class SlackNumberEmojis
    {
        public static string From(int number)
        {
            return number switch
            {
                1 => One,
                2 => Two,
                3 => Three,
                4 => Four,
                5 => Five,
                6 => Six,
                7 => Seven,
                8 => Eight,
                9 => Nine,
                _ => number.ToString()
            };
        }

        private static string One => ":first_place_medal:";
        private static string Two => ":two:";
        private static string Three => ":three:";
        private static string Four => ":four:";
        private static string Five => ":five:";
        private static string Six => ":six:";
        private static string Seven => ":seven:";
        private static string Eight => ":eight:";
        private static string Nine => ":nine:";
    }
}

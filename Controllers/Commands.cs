namespace MergeQueue.Controllers
{
    public static class Commands
    {
        public static string Show => nameof(Show).ToLowerInvariant();
        public static string Join => nameof(Join).ToLowerInvariant();
        public static string Leave => nameof(Leave).ToLowerInvariant();
        public static string Help => nameof(Help).ToLowerInvariant();
    }
}

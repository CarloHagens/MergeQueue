namespace MergeQueue.Extensions
{
    public static class ObjectExtensions
    {
        public static bool Exists(this object thing)
        {
            return thing != null;
        }

        public static bool DoesNotExist(this object thing)
        {
            return thing == null;
        }
    }
}

namespace FluffyByte.MUDServer.Core.Helpers;

public static class CollectionExtensions
{
    public static string ToFormattedList<T>(this IEnumerable<T> items, Func<T, int, string> formatter)
    {
        var itemsList = items.ToList();
        if (!itemsList.Any()) return string.Empty;
        if (itemsList.Count == 1) return formatter(itemsList[0], 0);
        
        var formatted = itemsList.Select((item, index) => formatter(item, index)).ToList();
        
        if (formatted.Count == 2)
            return $"{formatted[0]} and {formatted[1]}";
        
        var allButLast = string.Join(", ", formatted.Take(formatted.Count - 1));
        return $"{allButLast}, and {formatted.Last()}";
    }
    
    public static string ToFormattedList<T>(this IEnumerable<T> items, Func<T, string> formatter)
    {
        return items.ToFormattedList((item, _) => formatter(item));
    }
}
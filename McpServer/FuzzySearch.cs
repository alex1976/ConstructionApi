public static class FuzzySearch
{
    public static bool IsMatch(string? source, string? term)
    {
        if (string.IsNullOrWhiteSpace(term)) return true;
        if (string.IsNullOrWhiteSpace(source)) return false;

        if (source.Contains(term, StringComparison.OrdinalIgnoreCase)) return true;

        var sourceWords = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var termWords = term.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return termWords.All(termWord => sourceWords.Any(sourceWord =>
            LevenshteinDistance(sourceWord, termWord) <= MaxDistance(termWord)));
    }

    private static int MaxDistance(string term) => term.Length switch
    {
        <= 3 => 0,
        <= 5 => 1,
        <= 8 => 2,
        _ => 3
    };

    private static int LevenshteinDistance(string a, string b)
    {
        a = a.ToLowerInvariant();
        b = b.ToLowerInvariant();

        var distances = new int[a.Length + 1, b.Length + 1];
        for (var i = 0; i <= a.Length; i++) distances[i, 0] = i;
        for (var j = 0; j <= b.Length; j++) distances[0, j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            for (var j = 1; j <= b.Length; j++)
            {
                var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                distances[i, j] = Math.Min(
                    Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost);
            }
        }

        return distances[a.Length, b.Length];
    }
}

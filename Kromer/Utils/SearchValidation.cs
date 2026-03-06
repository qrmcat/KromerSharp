using System.Text.RegularExpressions;
using Kromer.Models.Api.Krist.Lookup;
using Kromer.Models.Exceptions;

namespace Kromer.Utils;

public static partial class SearchValidation
{
    public static string ValidateQuery(string? query)
    {
        if (string.IsNullOrEmpty(query))
        {
            throw new KristParameterException("q");
        }

        var trimmedQuery = query.Trim();
        return trimmedQuery.Length > 256
            ? throw new KristParameterException("q")
            : trimmedQuery;
    }

    public static ParsedQuery ParseQuery(string query)
    {
        var cleanId = BadIntChars().Replace(query, "");
        var hasId = int.TryParse(cleanId, out var id);

        var strippedName = Validation.StripNameSuffix(query);
        var parsedQuery = new ParsedQuery
        {
            OriginalQuery = query,

            MatchAddress = Validation.IsValidAddress(query),
            MatchName = !string.IsNullOrWhiteSpace(strippedName) && Validation.IsNameValid(strippedName, true),
            MatchBlock = hasId,
            MatchTransaction = hasId,

            StrippedName = strippedName,
            HasId = hasId,
            CleanId = hasId ? id : null,
        };

        return parsedQuery;
    }

    [GeneratedRegex(@"\W")]
    private static partial Regex BadIntChars();
}
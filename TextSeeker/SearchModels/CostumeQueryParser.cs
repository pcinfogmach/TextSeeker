using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using System.Text.RegularExpressions;

internal class CostumeQueryParser : QueryParser
{
    public CostumeQueryParser(LuceneVersion luceneVersion, string field, Analyzer analyzer)
        : base(luceneVersion, field, analyzer)
    {
        AllowLeadingWildcard = true;
        DefaultOperator = Operator.AND;
    }

    public override Query Parse(string queryText)
    {
        string escapedQueryText = EscapeDoubleQuotes(queryText);
        return base.Parse(escapedQueryText);
    }

    private string EscapeDoubleQuotes(string input)
    {
        return Regex.Replace(input, "\"(?=\\w)", "\\\"");
    }
}

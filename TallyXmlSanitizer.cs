using System.Text.RegularExpressions;

public static class TallyXmlSanitizer
{
    public static string Sanitize(string xml)
    {
        if (string.IsNullOrEmpty(xml)) return string.Empty;
        string sanitized = xml.Replace("&#4;", "");
        sanitized = Regex.Replace(sanitized, "&(?!amp;|lt;|gt;|apos;|quot;)", "&amp;");
        return sanitized;
    }
}

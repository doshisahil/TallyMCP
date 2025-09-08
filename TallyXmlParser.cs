using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

public static class TallyXmlParser
{
    public static object ParseImportResponseToJson(string xml)
    {
        try
        {
            var doc = XDocument.Parse(xml);
            var body = doc.Descendants("BODY").FirstOrDefault();
            if (body == null) return null;
            var importResult = body.Descendants("IMPORTRESULT").FirstOrDefault();
            var cmpInfo = body.Descendants("CMPINFO").FirstOrDefault();
            var result = new Dictionary<string, object>();
            if (importResult != null)
                result["ImportResult"] = importResult.Elements().ToDictionary(e => e.Name.LocalName, e => (object)e.Value);
            if (cmpInfo != null)
                result["CmpInfo"] = cmpInfo.Elements().ToDictionary(e => e.Name.LocalName, e => (object)e.Value);
            return result;
        }
        catch { return null; }
    }

    public static object ParseCompanyListToJson(string xml)
    {
        try
        {
            var doc = XDocument.Parse(xml);
            var companies = doc.Descendants("COMPANY")
                .Select(c => new {
                    Name = c.Attribute("NAME")?.Value ?? c.Element("NAME")?.Value
                })
                .Where(c => !string.IsNullOrEmpty(c.Name))
                .ToList();
            return companies;
        }
        catch { return null; }
    }

    public static object ParseLedgerListToJson(string xml)
    {
        try
        {
            var doc = XDocument.Parse(xml);
            var ledgers = doc.Descendants("LEDGER")
                .Select(l => new {
                    Name = l.Attribute("NAME")?.Value ?? l.Element("NAME")?.Value,
                    Parent = l.Element("PARENT")?.Value
                })
                .Where(l => !string.IsNullOrEmpty(l.Name))
                .ToList();
            return ledgers;
        }
        catch { return null; }
    }
}

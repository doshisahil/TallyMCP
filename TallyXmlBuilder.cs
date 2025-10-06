using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System;

public static class TallyXmlBuilder
{
    public static string BuildCompanyListRequestXml()
    {
        var envelope = new XElement("ENVELOPE",
            new XElement("HEADER",
                new XElement("VERSION", "1"),
                new XElement("TALLYREQUEST", "Export"),
                new XElement("TYPE", "Collection"),
                new XElement("ID", "List of Companies")
            ),
            new XElement("BODY",
                new XElement("DESC",
                    new XElement("TDL",
                        new XElement("TDLMESSAGE",
                            new XElement("COLLECTION",
                                new XAttribute("NAME", "List of Companies"),
                                new XAttribute("ISMODIFY", "No"),
                                new XElement("TYPE", "Company"),
                                new XElement("FETCH", "NAME")
                            )
                        )
                    )
                )
            )
        );
        return envelope.ToString(SaveOptions.DisableFormatting);
    }

    public static string BuildLedgerListRequestXml(string companyName)
    {
        var safeCompanyName = companyName;
        var envelope = new XElement("ENVELOPE",
            new XElement("HEADER",
                new XElement("VERSION", "1"),
                new XElement("TALLYREQUEST", "Export"),
                new XElement("TYPE", "Collection"),
                new XElement("ID", "List of Ledgers")
            ),
            new XElement("BODY",
                new XElement("DESC",
                    new XElement("STATICVARIABLES",
                        new XElement("SVCURRENTCOMPANY", safeCompanyName)
                    ),
                    new XElement("TDL",
                        new XElement("TDLMESSAGE",
                            new XElement("COLLECTION",
                                new XAttribute("NAME", "List of Ledgers"),
                                new XAttribute("ISMODIFY", "No"),
                                new XElement("TYPE", "Ledger"),
                                new XElement("FETCH", "NAME"),
                                new XElement("FETCH", "PARENT")
                            )
                        )
                    )
                )
            )
        );
        return envelope.ToString(SaveOptions.DisableFormatting);
    }

    public static string BuildVoucherImportXml(List<TallyTool.Transaction> transactions, string companyName)
    {
        var tallyMessages = transactions.Select(t =>
            new XElement("VOUCHER",
                new XElement("DATE", FormatDateForTally(t.Date)),
                new XElement("NARRATION", (t.Narration)),
                new XElement("VOUCHERTYPENAME", (t.Type)),
                new XElement("ALLLEDGERENTRIES.LIST",
                    new XElement("LEDGERNAME",  t.ToLedger),
                    new XElement("ISDEEMEDPOSITIVE", "Yes"),
                    new XElement("AMOUNT", $"-{Math.Abs(t.Amount)}")
                ),
                new XElement("ALLLEDGERENTRIES.LIST",
                    new XElement("LEDGERNAME", t.FromAccount),
                    new XElement("ISDEEMEDPOSITIVE", "No"),
                    new XElement("AMOUNT", $"{Math.Abs(t.Amount)}")
                )
            )
        );
        var envelope = new XElement("ENVELOPE",
            new XElement("HEADER",
                new XElement("VERSION", "1"),
                new XElement("TALLYREQUEST", "Import"),
                new XElement("TYPE", "Data"),
                new XElement("ID", "Vouchers")
            ),
            new XElement("BODY",
                new XElement("DESC",
                    new XElement("STATICVARIABLES",
                        new XElement("SVCURRENTCOMPANY", companyName)
                    )
                ),
                new XElement("DATA",
                    tallyMessages.Select(msg => new XElement("TALLYMESSAGE", msg))
                )
            )
        );
        return envelope.ToString(SaveOptions.DisableFormatting);
    }

    private static string FormatDateForTally(string date)
    {
        if (DateTime.TryParseExact(date, new[] { "dd-MM-yyyy", "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy" }, null, System.Globalization.DateTimeStyles.None, out var dt))
            return dt.ToString("yyyyMMdd");
        return new string(date?.Where(char.IsDigit).ToArray() ?? new char[0]);
    }


    private static bool IsReceipt(string type)
    {
        return string.Equals(type?.Trim(), "receipt", StringComparison.OrdinalIgnoreCase);
    }
}

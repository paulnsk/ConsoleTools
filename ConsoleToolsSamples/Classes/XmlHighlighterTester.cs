using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ConsoleTools;

namespace ConsoleToolsSamples.Classes;

public class OrderItem
{
    [XmlAttribute] // Renders Sku as an attribute of the <OrderItem> tag
    public string Sku { get; set; } = "";

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class Customer
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
}

public class Order
{
    [XmlAttribute("ID")] // Renames the attribute to "ID" in the XML output
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public Customer? BilledTo { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public static class XmlHighlighterTester
{
    public static void RunTests()
    {
        Konsole.WriteLineUnderlined("1. Test: Named Class (with XML attributes)", kolor: ConsoleColor.Yellow);
        var order = new Order
        {
            OrderId = 123,
            OrderDate = new DateTime(2025, 10, 26),
            BilledTo = new Customer { FirstName = "John", LastName = "Doe" },
            Items =
            [
                new() { Sku = "SKU-001", Quantity = 2, UnitPrice = 15.50m },
                new() { Sku = "SKU-002", Quantity = 1, UnitPrice = 50.00m }
            ]
        };
        Konsole.WriteLine(order.ToSyntaxHighlightedXml());
        Konsole.WriteLine();

        Konsole.WriteLineUnderlined("2. Test: Anonymous Class (expects serialization error)", kolor: ConsoleColor.Yellow);
        var anonReport = new { ReportTitle = "Quarterly Sales", IsValid = true };
        Konsole.WriteLine(anonReport.ToSyntaxHighlightedXml());
        Konsole.WriteLine();

        Konsole.WriteLineUnderlined("3. Test: Raw XML String", kolor: ConsoleColor.Yellow);
        var xmlString = """
                        <?xml-stylesheet type="text/xsl" href="style.xsl"?>
                        <catalog date="2025-10-26">
                          <!-- This is a list of books -->
                          <book id="bk101">
                            <author>Gambardella, Matthew</author>
                            <title>XML Developer's Guide</title>
                            <price>44.95</price>
                          </book>
                        </catalog>
                        """;
        Konsole.WriteLine(xmlString.ToSyntaxHighlightedXml());

        Konsole.PressAnyKey("Press any key to exit tests...");
    }
}
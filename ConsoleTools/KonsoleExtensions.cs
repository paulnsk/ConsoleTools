using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ConsoleTools
{
    public static class KonsoleExtensions
    {
        public static string ToJson(this object? o)
        {
            if (o == null) return ("NULL");
            return ToJsonNotNull(o);
        }
        private static string ToJsonNotNull(this object o)
        {
            return JsonSerializer.Serialize(o, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, Converters = { new JsonStringEnumConverter() } });
        }

        public static string ToSyntaxHighlightedJson(this object? o)
        {
            return ToJsonNotNull(o).ToSyntaxHighlightedJson();
        }


        //http://joelabrahamsson.com/syntax-highlighting-json-with-c/. Fuck knows how this works.
        public static string ToSyntaxHighlightedJson(this string json)
        {
            return Regex.Replace(
                json,
                @"(¤(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\¤])*¤(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)".Replace('¤', '"'),
                match =>
                {
                    var cls = "♦b";
                    //var cls = "number";
                    if (Regex.IsMatch(match.Value, @"^¤".Replace('¤', '"')))
                    {
                        if (Regex.IsMatch(match.Value, ":$"))
                        {
                            cls = "♦Y";
                            //cls = "key";
                        }
                        else
                        {
                            cls = "♦g";
                            //cls = "string";
                        }
                    }
                    else if (Regex.IsMatch(match.Value, "true|false"))
                    {
                        cls = "♦c";
                        //cls = "boolean";
                    }
                    else if (Regex.IsMatch(match.Value, "null"))
                    {
                        cls = "♦m";
                        //cls = "null";
                    }
                    //return "<span class=\"" + cls + "\">" + match + "</span>";
                    return cls + match + "♦w";
                });
        }


    }
}

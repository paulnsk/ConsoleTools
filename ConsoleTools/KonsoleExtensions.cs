using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ConsoleTools
{
    public static class KonsoleExtensions
    {
        #region toXML

        /// <summary>
        /// Serializes an object to a formatted XML string using the standard XmlSerializer.
        /// Does not support anonymous types.
        /// </summary>
        public static string ToXml(this object? o)
        {
            if (o == null) return "NULL";
            try
            {
                using var stringWriter = new StringWriter();
                var serializer = new XmlSerializer(o.GetType());
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true
                };

                using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    serializer.Serialize(xmlWriter, o, ns);
                }
                return stringWriter.ToString();
            }
            catch (Exception e)
            {
                return $"♦RError serializing to XML:♦w {e.Message}";
            }
        }

        /// <summary>
        /// Converts an object to XML and applies syntax highlighting.
        /// </summary>
        public static string ToSyntaxHighlightedXml(this object? o)
        {
            return o == null ? "" : o.ToXml().ToSyntaxHighlightedXml();
        }

        /// <summary>
        /// Applies syntax highlighting to an XML string.
        /// </summary>
        public static string ToSyntaxHighlightedXml(this string xml)
        {
            // This regex splits the XML into four main groups: comments, processing instructions, tags, and text content.
            // The order is important: more specific patterns (like comments) must come before general ones (like tags).
            var mainRegex = new Regex(
                @"(?<comment><!--.*?-->)|(?<pi><\?.*?\?>)|(?<tag><[^>]+>)|(?<content>[^<>]+)",
                RegexOptions.Singleline);

            return mainRegex.Replace(xml, mainMatch =>
            {
                if (mainMatch.Groups["comment"].Success) return $"♦G{mainMatch.Value}♦w";
                if (mainMatch.Groups["pi"].Success) return $"♦m{mainMatch.Value}♦w";
                if (mainMatch.Groups["content"].Success)
                {
                    if (decimal.TryParse(mainMatch.Value.Trim(), out _)) return $"♦y{mainMatch.Value}♦w"; // Numbers are yellow
                    return $"♦w{mainMatch.Value}";
                }
                if (mainMatch.Groups["tag"].Success)
                {
                    var tag = mainMatch.Value;
                    var tagRegex = new Regex(
                        @"(?<punctuation><\/?|\/?>)|" +
                        @"(?<tagName>[\w-:]+)|" +
                        @"(?<attrName>\s+[\w-:]+)|" +
                        @"(?<equals>\s*=\s*)|" +
                        @"(?<attrValue>""[^""]*"")"
                    );
                    return tagRegex.Replace(tag, tagMatch => {
                        if (tagMatch.Groups["punctuation"].Success) return $"♦C{tagMatch.Value}♦w"; 
                        if (tagMatch.Groups["tagName"].Success) return $"♦c{tagMatch.Value}♦w";
                        if (tagMatch.Groups["attrName"].Success) return $"♦b{tagMatch.Value}♦w";
                        if (tagMatch.Groups["equals"].Success) return $"♦A{tagMatch.Value}♦w";
                        if (tagMatch.Groups["attrValue"].Success) return $"♦g{tagMatch.Value}♦w";
                        return tagMatch.Value;
                    });
                }
                return mainMatch.Value;
            });
        }
        
        #endregion


        #region ToJson

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
            return o == null ? "" : ToJsonNotNull(o).ToSyntaxHighlightedJson();
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
                    if (Regex.IsMatch(match.Value, @"^¤".Replace('¤', '"')))
                    {
                        if (Regex.IsMatch(match.Value, ":$"))
                        {
                            cls = "♦Y";
                        }
                        else
                        {
                            cls = "♦g";
                        }
                    }
                    else if (Regex.IsMatch(match.Value, "true|false"))
                    {
                        cls = "♦c";
                    }
                    else if (Regex.IsMatch(match.Value, "null"))
                    {
                        cls = "♦m";
                    }
                    return cls + match + "♦w";
                });
        }


        #endregion

    }
}

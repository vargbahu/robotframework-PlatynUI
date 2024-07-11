using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using PlatynUI.Technology.UiAutomation.Client;

namespace PlatynUI.Technology.UiAutomation.Core;

public class UiaXsltContext : XsltContext
{
    public override bool Whitespace => true;

    public override int CompareDocument(string baseUri, string nextbaseUri)
    {
        return string.CompareOrdinal(baseUri, nextbaseUri);
    }

    public override bool PreserveWhitespace(XPathNavigator node)
    {
        return false;
    }

    public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
    {
        if (name == "ends-with")
        {
            return endsWith;
        }

        return null!;
    }

    public override IXsltContextVariable ResolveVariable(string prefix, string name)
    {
        return null!;
    }

    internal readonly IXsltContextFunction endsWith = new UiaXsltFunctionEndsWith();

    internal class UiaXsltFunctionEndsWith : IXsltContextFunction
    {
        public int Minargs => 2;

        public int Maxargs => 2;

        public XPathResultType ReturnType => XPathResultType.Boolean;

        public XPathResultType[] ArgTypes => [XPathResultType.String, XPathResultType.String];

        public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
        {
            if (args[0] is string s && args[1] is string suffix)
            {
                return s.EndsWith(suffix);
            }
            else if (args[0] is XPathNodeIterator nodeIterator && args[1] is string suffix1)
            {
                while (nodeIterator.MoveNext())
                {
                    if (nodeIterator.Current != null && nodeIterator.Current.Value.EndsWith(suffix1))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

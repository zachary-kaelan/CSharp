using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XmlConfiguration;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace ZachLib.Parsing
{
    public class XMLModelCreator
    {
        private static KeyValuePair<string, string> RootElement = new KeyValuePair<string, string>("XML", "Root");

        public XMLModelCreator(string xml)
        {
            //Root = GetNodeString(GetNodeNames(xml), RootElement);
        }

        private Dictionary<int, KeyValuePair<Dictionary<string, Dictionary<string, string>>, Dictionary<string, Dictionary<string, string>>>> Types = new Dictionary<int, KeyValuePair<Dictionary<string, Dictionary<string, string>>, Dictionary<string, Dictionary<string, string>>>>();

        public void PrintModel()
        {
            var max = Layers.Keys.Max();
            PrintModel("XML", "Root", Layers[0][RootElement]);
        }

        private void PrintModel(string parentName, string name, IEnumerable<KeyValuePair<string, string>> children, int tabs = 0)
        {
            var tabsString = new string('\t', tabs);
            Console.WriteLine(tabs + name);
            var values = children.Where(c => c.Value != "object").OrderBy(c => c.Key);
            var parents = children.Where(c => c.Value == "object").Select(c => c.Key).OrderBy();

            int newTabs = tabs + 1;
            tabsString += "\t";
            foreach(var value in values)
            {
                Console.WriteLine(tabsString + value.Key + ": " + value.Value);
            }

            foreach(var parent in parents)
            {
                PrintModel(name, parent, Layers[newTabs][new KeyValuePair<string, string>(name, parent)], newTabs);
            }
        }

        private string Root = null;
        private Dictionary<int, Dictionary<KeyValuePair<string, string>, Dictionary<string, string>>> Layers = new Dictionary<int, Dictionary<KeyValuePair<string, string>, Dictionary<string, string>>>();

        public string GetNodeString(KeyValuePair<bool, object> node, KeyValuePair<string, string> names, int tabs = 0)
        {
            if (node.Key)
            {
                int newTabs = tabs + 1;
                string joinString = "\r\n" + new string('\t', tabs);
                var dict = (Dictionary<string, KeyValuePair<bool, object>>)node.Value;

                if (!Layers.TryGetValue(tabs, out Dictionary<KeyValuePair<string, string>, Dictionary<string, string>> layer))
                    Layers.Add(tabs, new Dictionary<KeyValuePair<string, string>, Dictionary<string, string>>());

                if (layer != null && layer.TryGetValue(names, out Dictionary<string, string> children))
                {
                    var childrenKeys = children.Keys.ToArray();
                    var keys = dict.Keys.Where(k => !childrenKeys.Contains(k) || dict[k].Key).ToArray();
                    if (keys.Length > 0)
                    {
                        foreach (var key in keys)
                        {
                            children.TryAdd(
                                key,
                                GetNodeString(
                                    dict[key],
                                    new KeyValuePair<string, string>(names.Value, key),
                                    newTabs
                                )
                            );
                        }
                        Layers[tabs][names] = children;
                    }
                }
                else
                    Layers[tabs].Add(
                        names, 
                        dict.ToDictionary(
                            kv => kv.Key,
                            kv => GetNodeString(
                                kv.Value,
                                new KeyValuePair<string, string>(names.Value, kv.Key),
                                newTabs
                            )
                        )
                    );

                return "object";
            }

            return node.Value.ToString();
        }

        private static string GetNodeType(string xml)
        {
            //XmlReader reader = XmlReader.Create(new StringReader(xml));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            //return GetNodeType(doc.DocumentElement);
            return null;
        }

        private static bool HasChildren(XmlNode node, out XmlNode[] children)
        {
            if (node.HasChildNodes)
            {
                children = node.ChildNodes.Cast<XmlNode>().Where(
                    n => n.NodeType == XmlNodeType.Element
                ).ToArray();
                return children.Any();
            }
            else
            {
                children = null;
                return false;
            }
        }

        private  string GetNodeType(XmlNode node, bool allObjects, int depthLevel = 0)
        {
            XmlNode[] children = null;

            if (HasChildren(node, out children))
            {
                int newDepthLevel = depthLevel + 1;
                var dict = children.ToDictionary(
                    n => n,
                    n => HasChildren(n, out XmlNode[] childrenTemp) ?
                        new KeyValuePair<bool, XmlNode[]>(true, childrenTemp) :
                        new KeyValuePair<bool, XmlNode[]>(false, null)
                );

                var types = dict.GroupBy(
                    kv => kv.Value,
                    kv => kv.Key,
                    (k, g) => k
                );

                if (types.Count() == 1)
                    return "Dictionary<string, " + types.Single() + ">";
                else
                {

                }
            }
            else
                return GetStringType(node.InnerText);

            return null;
        }

        private string GetNodeType(string name, string parentName, XmlNode[] children, bool allObjects, int depthLevel)
        {
            int newDepthLevel = depthLevel + 1;
            var dict = children.ToDictionary(
                n => n,
                n => HasChildren(n, out XmlNode[] childrenTemp) ?
                    new KeyValuePair<bool, XmlNode[]>(true, childrenTemp) :
                    new KeyValuePair<bool, XmlNode[]>(false, null)
            );

            var types = dict.GroupBy(
                kv => kv.Value.Key,
                kv => new KeyValuePair<XmlNode, XmlNode[]>(kv.Key, kv.Value.Value)
            );

            bool thisAllObjects = false;

            if (types.Count() == 1)
            {
                if (types.Single().Key)
                    thisAllObjects = true;
                else
                    return "Dictionary<string, " + GetStringType(dict.Keys.First().InnerText) + ">";
            }
            dict = null;

            Dictionary<string, string> type = new Dictionary<string, string>();
            foreach(var typeGroup in types)
            {
                if (typeGroup.Key)
                {
                    foreach(var child in typeGroup)
                    {
                        string childName = child.Key.Name;
                        type.Add(childName, GetNodeType(childName, name, child.Value, thisAllObjects, newDepthLevel));
                    }
                }
                else
                {
                    foreach(var child in typeGroup)
                    {
                        type.Add(child.Key.Name, GetStringType(child.Key.InnerText));
                    }
                }
            }
            types = null;

            if (Types.TryGetValue(depthLevel, out KeyValuePair<Dictionary<string, Dictionary<string, string>>, Dictionary<string, Dictionary<string, string>>> otherTypes))
            {
                if (!allObjects)
                {
                    var typeName = name + "_OBJ";
                    if (otherTypes.Value.TryGetValue(typeName, out Dictionary<string, string> otherType))
                    {
                        var otherKeys = otherType.Keys.ToArray();
                        Array.Sort(otherKeys);
                        var keys = type.Keys.Where(k => Array.BinarySearch<string>(otherKeys, k) == -1);
                        if (keys.Any())
                        {
                            foreach (var key in keys)
                            {
                                otherType.Add(key, type[key]);
                            }
                            Types[depthLevel].Value[typeName] = otherType;
                        }
                    }
                    else
                        Types[depthLevel].Value.Add(typeName, type);

                    return typeName;
                }
                else
                {
                    var keys = type.Keys.ToArray();
                    var midpoint = Math.Min((keys.Length / 2) + 1, keys.Length);
                    foreach(var otherType in otherTypes.Key)
                    {
                        var intersection = otherType.Value.Keys.Intersect(keys);
                        if (intersection.Count() >= midpoint)
                        {
                            keys = keys.Except(intersection).ToArray();
                            var newType = otherType.Value;
                            foreach(var key in keys)
                            {
                                newType.Add(key, type[key]);
                            }
                            Types[depthLevel].Key[otherType.Key] = newType;
                            return otherType.Key;
                        }
                    }
                    string typeName = parentName + "_OBJ_";
                }
            }

            Dictionary<string, Dictionary<string, string>> dict1 = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> dict2 = new Dictionary<string, Dictionary<string, string>>()
            {
                { "", null  }
            };

            return null;
        }

        private static string GetStringType(string str)
        {
            str = str.ToLower().Trim();
            if (str == "true" || str == "false")
                return "bool";
            else if (String.IsNullOrEmpty(str))
                return "null";

            var chars = str.AsEnumerable();
            var noDigits = chars.Where(c => !Char.IsDigit(c));

            if (noDigits.Any())
            {
                if (noDigits.Count() == 1 && noDigits.Single() == '.' && chars.Count() != 1)
                    return "double";
                else
                    return "string";
            }
            else
                return "int";
        }
    }
}

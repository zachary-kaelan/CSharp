﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZachRegex
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeSpan ts10 = TimeSpan.FromMilliseconds(10);
            TimeSpan ts25 = TimeSpan.FromMilliseconds(25);
            TimeSpan ts50 = TimeSpan.FromMilliseconds(50);
            TimeSpan ts100 = TimeSpan.FromMilliseconds(100);
            TimeSpan ts250 = TimeSpan.FromMilliseconds(250);

            //string guid = "d60a793c-365b-4d4a-96a7-81b21e81d015";
            AssemblyName name = new AssemblyName("ZachRGX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=41ff425b50d3cee4");
            name.KeyPair = new StrongNameKeyPair(File.OpenRead(@"C:\Users\ZACH-GAMING\Source\StrongKeys\ZachRGX.snk"));

            Type attribute = typeof(PrimaryInteropAssemblyAttribute);
            Type intType = typeof(int);
            Type attribute2 = typeof(GuidAttribute);
            Type attribute3 = typeof(ImportedFromTypeLibAttribute);

            Regex.CompileToAssembly(
                new RegexCompilationInfo[] {
                    new RegexCompilationInfo(@"^\(?(\d{3})[\) -]*(\d{3})-?(\d{4})", RegexOptions.None, "Phone", "RGX.UTILS", true, ts10),
                    new RegexCompilationInfo(@"(\d+)[^\d\\]*\.pdf$", RegexOptions.RightToLeft, "FileDigit", "RGX.UTILS", true, ts10),
                    new RegexCompilationInfo("^\"?(.+) :=: ([^\"]+)", RegexOptions.Multiline, "FileDictionary", "RGX.UTILS", true, ts250),
                    new RegexCompilationInfo(@"\[\[([^\]]+)\]\]", RegexOptions.None, "SecondaryFormatting", "RGX.UTILS", true, ts50),
                    new RegexCompilationInfo(@"[^A-Za-z0-9_]", RegexOptions.CultureInvariant, "Symbols", "RGX.UTILS", true, ts25),
                    new RegexCompilationInfo(@"[^A-Za-z0-9]", RegexOptions.CultureInvariant, "NonAlphaNumeric", "RGX.UTILS", true, ts25),
                    new RegexCompilationInfo(@"[\r\n\t~#%&*{}\:<>?/+|" + "\"" + @"]|(?:^[._]*)|(?:\.$)|(?:\.{2,})", RegexOptions.None, "MakeFilenameFriendly", "RGX.UTILS", true, ts25),
                    new RegexCompilationInfo(@">([^<\r\n]+)<", RegexOptions.RightToLeft, "XmlValue", "RGX.UTILS", true, ts10),
                    new RegexCompilationInfo(@"^\s*<(?<Key>[^>]+)>(?<Value>[^<\r\n]+)<\/", RegexOptions.Multiline, "XmlKeyValue", "RGX.UTILS", true, ts25),

                    //--------------------------------//
                    //------------- HTML -------------//
                    //--------------------------------//

                    new RegexCompilationInfo("_(?<Key>[^\"]+)\">(?<Value>[^<]+)<" + @"\/span><\/li>", RegexOptions.RightToLeft, "ListElements", "RGX.HTML", true, ts250),
                    new RegexCompilationInfo(@"\bname=" + "\"(?<Name>(?:[^\"]+" + @"\[)?_token\]?)" + "\" value=\"(?<Token>[^\"]+)\"", RegexOptions.None, "Token", "RGX.HTML", true, ts250),
                        // Group 1: Token name
                        // Group 2: Token
                    new RegexCompilationInfo(@"\b(?:action=" + "\"(?=" + @"\/)|href=" + "\"https?:" + @"\/\/(?<Host>[^\/]+)\/)(?<Resource>[\/a-z-]+\/)(?<Code>[A-Za-z0-9=]{24,})" + "\">", RegexOptions.None, "URLCode", "RGX.HTML", true, ts250),
                        // Group 1: Host
                        // Group 2: Resource
                        // Group 3: Code
                    new RegexCompilationInfo("<[^>]+>", RegexOptions.None, "WebTags", "RGX.HTML", true, ts100),
                    new RegexCompilationInfo(@"<a [^\/\n>:]+(?<Value>\/[^" + "\"]+)\">(?<Key>[^<]+)", RegexOptions.None, "Links", "RGX.HTML", true, ts250),
                        // Key = Text
                        // Value = URL
                    new RegexCompilationInfo(@"<p[^>]*>(?:<p[^>]*>|\s)*(?<Paragraph>[^\n]+(?:[^<>\/]{5}\n{2}[^<>\/]{5}[^\n]+)?)(?:<\/p>|\s)*<\/p>(?:<\/li>$)?", RegexOptions.Multiline, "Paragraphs", "RGX.HTML", true, ts250),
                        // Paragraph

                    //-----------------------------------//
                    //------------- HEADERS -------------//
                    //-----------------------------------//

                    new RegexCompilationInfo(@"^\s*<h1[^>]*>\n?(?(?=<)<[^>]+>)*(?<Header>[^<]+)(?:\s*<\/h1>\s*)?", RegexOptions.Multiline, "Header1", "RGX.HTML.HEADERS", true, ts100),
                        // Header
                    new RegexCompilationInfo(@"^\s*<h2[^>]*>\n?(?(?=<)<[^>]+>)*(?<Header>[^<]+)(?:\s*<\/h2>\s*)?", RegexOptions.Multiline, "Header2", "RGX.HTML.HEADERS", true, ts100),
                        // Header
                    new RegexCompilationInfo(@"^\s*<h3[^>]*>\n?(?(?=<)<[^>]+>)*(?<Header>[^<]+)(?:\s*<\/h3>\s*)?", RegexOptions.Multiline, "Header3", "RGX.HTML.HEADERS", true, ts100),
                        // Header
                    new RegexCompilationInfo(@"^\s*<h4[^>]*>\n?(?(?=<)<[^>]+>)*(?<Header>[^<]+)(?:\s*<\/h4>\s*)?", RegexOptions.Multiline, "Header4", "RGX.HTML.HEADERS", true, ts100),
                        // Header
                    new RegexCompilationInfo(@"^\s*<h5[^>]*>\n?(?(?=<)<[^>]+>)*(?<Header>[^<]+)(?:\s*<\/h5>\s*)?", RegexOptions.Multiline, "Header5", "RGX.HTML.HEADERS", true, ts100),
                        // Header
                    new RegexCompilationInfo(@"^\s*<h6[^>]*>\n?(?(?=<)<[^>]+>)*(?<Header>[^<]+)(?:\s*<\/h6>\s*)?", RegexOptions.Multiline, "Header6", "RGX.HTML.HEADERS", true, ts100),
                        // Header

                    //-----------------------------------//
                    //------------- REDDIT --------------//
                    //-----------------------------------//

                    new RegexCompilationInfo(@">\s*\/?r\/?thathappened", RegexOptions.IgnoreCase, "CheckQuote1", "RGX.Reddit.LFS", true, ts25),
                    new RegexCompilationInfo("\"" + @".*\/?r\/?thathappened.*" + "\"", RegexOptions.IgnoreCase, "CheckQuote2", "RGX.Reddit.LFS", true, ts25),
                    new RegexCompilationInfo(@"\/r\/thathappened.*\/s", RegexOptions.Singleline | RegexOptions.IgnoreCase, "CheckSarcasm", "RGX.Reddit", true, ts25),

                    //---------------------------------//
                    //------------ EXAMINE ------------//
                    //---------------------------------//

                    new RegexCompilationInfo(@"icons\/grade-(?<LevelOfEvidence>[abcd])(?:[^\n]+\n){4}<a href=.\/topics\/(?<URLName>[^\/]+)[^\n]+\n(?<Effect>[^<]+)\t(?:[^\n]+\n){4}(?(?=-)|[^" + "\"]+(?:[^-]+-){2})(?<Magnitude>-|(?:up|down)-" + @"\d)(?:(?:[^\n]*\n){11}<div class=.comments.>\n(?<Comments>[^<]+))?", RegexOptions.None, "EffectsMatrix", "RGX.Examine", true, ts250),
                        // LevelOfEvidence
                        // URLName
                        // Effect
                        // Magnitude
                        // Comments
                    new RegexCompilationInfo("\"(?<Tag>[^\"]+)\">[^>]+><.l", RegexOptions.None, "Tags", "RGX.Examine", true, ts250),
                        // Tag
                    new RegexCompilationInfo("^<title>(?<Title>.+?) - Scientific Review on Usage, Dosage, Side Effects", RegexOptions.Multiline, "Title", "RGX.Examine", true, ts250),
                        // Title
                    new RegexCompilationInfo("data-content=\"(?<Value>[^\"]+)\" data-title=\"(?<Key>[^\"]+)\"", RegexOptions.None, "Tooltips", "RGX.Examine", true, ts250),
                        // Key
                        // Value
                    new RegexCompilationInfo(@"\/user\/(?<User>[^\/]+)", RegexOptions.None, "EditorUsername", "RGX.Examine", true, ts100),
                        // User
                    new RegexCompilationInfo(@"^\nQ: (?<Key>.+?)\n\nA: (?<Value>.+)", RegexOptions.Singleline, "QnA", "RGX.Examine", true, ts100),
                        // Key = Question
                        // Value = Answer
                    new RegexCompilationInfo(@"^<li>\n(?:<[^s].+\n)?(?:<span>(?<Author>[^<]+)<\/span>\n)?<a href=" + "\"(?<URL>[^\"" + @"]+)[^>]+>(?<Title>[^<]+)<\/a><span>.<\/span>\n(?:<em>(?<Publisher>[^<]+)<[^\d]+(?<Year>\d+))?", RegexOptions.Multiline, "Citations", "RGX.Examine", true, ts250),
                    
                    //---------------------------------//
                    //------------ PSYCHONAUTS --------//
                    //---------------------------------//

                    new RegexCompilationInfo(@"^(?:<ul>)?<li>[^>]+>(?<Effect>[^<]+)", RegexOptions.Multiline, "SubjectiveEffectsIndex", "RGX.Psychonauts", true, ts100),

                    //---------------------------//
                    //----------- MUSIC ---------//
                    //---------------------------//

                    new RegexCompilationInfo(@"\/(?<Param1>[^-]+) ?(?<Type>-|by|:) ?(?<Param2>[^.(]+) ?(?:\((?<Param3>[^).]*)\))? ?\.[a-z3]{1,4}$", RegexOptions.RightToLeft, "Filename", "RGX.MUSIC", true, ts100),

                    //---------------------------//
                    //----------- STEAM ---------//
                    //---------------------------//

                    new RegexCompilationInfo(@"^\s{12}[^" + "\"]+\"app_tag\">(?<Tag>[^<]+)", RegexOptions.Multiline, "AppTags", "RGX.STEAM", true, ts100),
                }, name/*,
                new CustomAttributeBuilder[] {
                    new CustomAttributeBuilder(
                        attribute3.GetConstructor(new Type[] { typeof(string) }),
                        new object[] { @"C:\Users\ZACH-GAMING\Documents\Visual Studio 2017\Projects\ZachRegex\bin\Debug\ZachRGX.dll" }
                    )
                    new CustomAttributeBuilder(
                        attribute.GetConstructor(
                            new Type[] { intType, intType }
                        ), new object[] { 1, 0 }
                    ), new CustomAttributeBuilder(
                        attribute2.GetConstructor(new Type[] { typeof(string) }),
                        new object[] { guid }
                    )
                }*/
            );
        }
    }
}

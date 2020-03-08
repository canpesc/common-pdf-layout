﻿using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using SelectPdf;

namespace pesctranscriptconverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();

            }
            var option = args[0];
            var inputfilepath = string.Empty;
            var outputfilepath = string.Empty;
            var xsltPath = string.Empty;

            FileStream fsIn;
            TextReader textReader;

            AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);

            try
            {

                switch (option)
                {
                    case "tohtml":

                        if (args.Length != 4)
                        {
                            PrintUsage();
                        }
                        inputfilepath = args[1];
                        outputfilepath = args[2];
                        xsltPath = args[3];


                        fsIn = new FileStream(inputfilepath, FileMode.Open);
                        textReader = new StreamReader(fsIn);


                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(textReader.ReadToEnd());

                        XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
                        XsltArgumentList xsltArgumentList = new XsltArgumentList();

                        FileStream fs = new FileStream(outputfilepath, FileMode.Create, FileAccess.Write);
                        TextWriter writer = new StreamWriter(fs);

                        xslCompiledTransform.Load(xsltPath);
                        xslCompiledTransform.Transform(xmlDocument, xsltArgumentList, writer);

                        fs.Close();

                        Console.WriteLine("Converted to HTML");
                        break;

                    case "topdf":
                        if (args.Length != 3)
                        {
                            PrintUsage();
                        }
                        inputfilepath = args[1];
                        outputfilepath = args[2];

                        fsIn = new FileStream(inputfilepath, FileMode.Open);
                        textReader = new StreamReader(fsIn);


                        // instantiate the html to pdf converter
                        HtmlToPdf converter = new HtmlToPdf();

                        // convert the url to pdf
                        PdfDocument doc = converter.ConvertHtmlString(textReader.ReadToEnd());

                        // save pdf document
                        doc.Save(outputfilepath);

                        // close pdf document
                        doc.Close();

                        Console.WriteLine("Converted to PDF");
                        break;
                    default:
                        PrintUsage();
                        break;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");

                PrintUsage();
            }

        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: pesctranscriptconvert [tohtml|topdf] inputfilepath outputfilepath [xsltfilepath]");
            Console.WriteLine("Example: pesctranscriptconvert tohtml inputfile.xml outputfile.html transform.xslt");
            Console.WriteLine("Example: pesctranscriptconvert topdf inputfile.html outputfile.pdf");

        }

        private static string TransformXMLToHTML(string inputXml, string xsltString)
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            using (XmlReader reader = XmlReader.Create(new StringReader(xsltString)))
            {
                transform.Load(reader);
            }
            StringWriter results = new StringWriter();
            using (XmlReader reader = XmlReader.Create(new StringReader(inputXml)))
            {
                transform.Transform(reader, null, results);
            }
            return results.ToString();
        }
    }
}

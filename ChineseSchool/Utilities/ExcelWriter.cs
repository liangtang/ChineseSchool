using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Microsoft.VisualBasic.FileIO;

namespace ChineseSchool.Utilities
{
    [Serializable()]
    public class ExcelWriter : MarshalByRefObject
    {
        public static void WriteExcel(string excelPath, string sheetName, IEnumerable<string> head, IEnumerable<IEnumerable<string>> data)
        {
            SpreadsheetDocument spreadsheet;
            WorkbookPart workbook;
            WorksheetPart worksheet;
            OpenXmlWriter writer;
            Row r = new Row();
            Cell c = new Cell();
            CellValue v = new CellValue();
            bool fileExists = File.Exists(excelPath);

            using (spreadsheet = fileExists ? SpreadsheetDocument.Open(excelPath, true) : SpreadsheetDocument.Create(excelPath, SpreadsheetDocumentType.Workbook))
            {
                List<OpenXmlAttribute> oxa;
                if (!fileExists)
                {
                    spreadsheet.AddWorkbookPart();
                }

                workbook = spreadsheet.WorkbookPart;

                worksheet = workbook.AddNewPart<WorksheetPart>();




                if (!fileExists)
                {
                    var stylesPart = workbook.AddNewPart<WorkbookStylesPart>();
                    var style = GenerateStyleSheet();
                    style.NumberingFormats = new NumberingFormats();
                    style.NumberingFormats.AppendChild<NumberingFormat>(new NumberingFormat() { NumberFormatId = UInt32Value.FromUInt32(166), FormatCode = "$0.00" });
                    style.NumberingFormats.AppendChild<NumberingFormat>(new NumberingFormat() { NumberFormatId = UInt32Value.FromUInt32(167), FormatCode = "0.000" });
                    style.NumberingFormats.Count = 2;
                    stylesPart.Stylesheet = style;
                    stylesPart.Stylesheet.Save();
                }
                writer = OpenXmlWriter.Create(worksheet);
                writer.WriteStartElement(new Worksheet());
                writer.WriteStartElement(new SheetData());


                int i = 1;

                oxa = new List<OpenXmlAttribute>();
                oxa.Add(new OpenXmlAttribute("r", null, "1"));
                writer.WriteStartElement(new Row(), oxa);
                foreach (string title in head)
                {
                    oxa = new List<OpenXmlAttribute>();
                    oxa.Add(new OpenXmlAttribute("t", null, "str"));
                    writer.WriteStartElement(new Cell(), oxa);
                    if (!String.IsNullOrEmpty(title))
                    {
                        writer.WriteElement(new CellValue(title));
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                foreach (var row in data)
                {
                    i++;
                    oxa = new List<OpenXmlAttribute>();
                    oxa.Add(new OpenXmlAttribute("r", null, i.ToString()));
                    writer.WriteStartElement(new Row(), oxa);
                    foreach (var col in row)
                    {
                        oxa = new List<OpenXmlAttribute>();
                        oxa.Add(new OpenXmlAttribute("t", null, "str"));
                        writer.WriteStartElement(new Cell(), oxa);
                        if (!String.IsNullOrEmpty(col))
                        {
                            writer.WriteElement(new CellValue(col.ToString()));
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();
                if (fileExists)
                {
                    Sheets sheets = workbook.Workbook.Sheets;
                    sheets.Append(new Sheet() { Id = workbook.GetIdOfPart(worksheet), SheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1, Name = sheetName });

                }
                else
                {

                    writer = OpenXmlWriter.Create(workbook);
                    writer.WriteStartElement(new Workbook());
                    writer.WriteStartElement(new Sheets());

                    writer.WriteElement(new Sheet() { Id = workbook.GetIdOfPart(worksheet), SheetId = 1, Name = sheetName });

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.Close();
                }

                spreadsheet.Close();
            }
        }
         public static Stylesheet GenerateStyleSheet()
        {
            return new Stylesheet(
                new Fonts(
                    new Font(                                                               // Index 0 - The default font.
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "00000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                               // Index 1 - The bold font.
                        new Bold(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "00000000" } },
                        new FontName() { Val = "Calibri" })
                ),
                new Fills(
                    new Fill(                                                           // Index 0 - The default fill.
                        new PatternFill() { PatternType = PatternValues.None })
                ),
                new Borders(
                    new Border(                                                         // Index 0 - The default border.
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder())
                ),
                //new NumberingFormats(
                //        new NumberingFormat() { NumberFormatId = UInt32Value.FromUInt32(166) ,FormatCode = "0.000%"}
                //    ),
                new CellFormats(
                    new CellFormat()
                    { FontId = 0, FillId = 0, BorderId = 0 }
                    ,new CellFormat() { FontId = 0, FillId =0, BorderId = 0, ApplyNumberFormat = true, NumberFormatId = 1 }                        
                    ,new CellFormat() { FontId = 0, FillId = 0, BorderId = 0, ApplyNumberFormat = true, NumberFormatId = 166 }       
                    , new CellFormat() { FontId = 0, FillId = 0, BorderId = 0,ApplyNumberFormat = true,NumberFormatId =167
        }
                )
            );
        }

       
    
    }
}
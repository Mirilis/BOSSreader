using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOSSreader
{
    public class BHNHeader
    {
            private const string Comma = ",";                    //for csv
            private const string CRLF = "\r\n";                 //required line ending
            private const string Label1 = "\"LO:\"";              //header line 2
            private const string Label2 = "\"LW:\"";              //header line 2
            private const string Label3 = "\"HW:\"";              //header line 2
            private const string Label4 = "\"HI:\"";              //header line 2
            private const string DataLine = "** Data **";           //header line 14
            private const string DateCodeLabel = "DATE CODE      ";      //header line 4
            private const string PartNumberLabel = "PART #         ";      //header line 5
            private const string PartNameLabel = "PART NAME      ";      //header line 6
            private const string CustomerLabel = "CUSTOMER       ";      //header line 7
            private const string SpecLabel = "SPECIFICATION  ";      //header line 8
            private const string RangeLabel = "RANGE          ";      //header line 9
            private const string Comment1Label = "COMMENT1       ";      //header line 10 
            private const string Comment2Label = "COMMENT2       ";      //header line 11
            private const string Blank1Label = "               ";      //header line 12
            private const string Blank2Label = "               ";      //header line 13


            private string FileSignatureVersion = "WinMT92V V3.0 ";      // len 14, pad right
            private string RecordCount = "    0";                // len 5, pad left
            private string RecordLength = "  0";                  // len 3, pad left
            private string ExtraHeaderLength = "592";                  // len 3, pad left
            private string AvgCnt = " 1";                   // len 2, pad left
            private string ConvertScale = "DIA  ";                // len 5, pad right
            private string LoadTime = "10";                   // len 2, pad left
            private string Flags = "   A6";                // len 5, pad left
            private string Header2Len = " 27";                  // len 3, pad left
            private string ExtraFieldDefLen = "  0";                  // len 3, pad left
            private string MaxStdDev = "   99.9";              // len 7, pad left
            private string Objective = "   30";                // len 5, pad left
            private string Load = " 3000.0";              // len 7, pad left
            private string Ball = "10.0";                // len 4, pad left
            private string Scale = "HBW  ";                // len 5, pad right
            private string LowTol = "    179";              // len 7, pad left
            private string LoWarn = "    187";              // len 7, pad left
            private string HiWarn = "    217";              // len 7, pad left
            private string HiTol = "    229";              //len 7, pad left

            private string DateCode = "                    "; //len 20, pad right
            private string PartNumber = "6Y6932              "; //len 20, pad right
            private string PartName = "                    "; //len 20, pad right
            private string Customer = "                    "; //len 20, pad right
            private string Specification = "1E0356              "; //len 20, pad right
            private string Range = "156-217             "; //len 20, pad right
            private string blank1Comment = "                    "; //len 20,pad right
            private string blank2Comment = "                    "; //len 20,pad right
            private string blank1blank = "                    "; //len 20,pad right
            private string blank2blank = "                    "; //len 20,pad right
            private string FileComment = "Generated With NWC Auto BHN Data File Generator";


            public BHNHeader(BossRangeData p)
            {
                this.PartNumber = p.Product.PadRight(20).Substring(0, 20);
                if (p.Specification != null)
                {
                    this.Specification = p.Specification.PadRight(20).Substring(0, 20);
                }
                else { this.Specification = ("N/A").PadRight(20).Substring(0, 20); }
                var r = p.RangeLow.ToString() + " - " + p.RangeHigh.ToString();
                this.Range = r.PadRight(20).Substring(0, 20);

                this.LowTol = p.RangeLow.Value.ToString().PadLeft(7);
                this.HiTol = p.RangeHigh.Value.ToString().PadLeft(7);
      
                
                int LowIndexValue = BrinellNumbers.List.Aggregate((x, y) => Math.Abs(x - p.RangeLow.Value) < Math.Abs(y - p.RangeLow.Value) ? x : y);
                int HighIndexValue = BrinellNumbers.List.Aggregate((x, y) => Math.Abs(x - p.RangeHigh.Value) < Math.Abs(y - p.RangeHigh.Value) ? x : y);

                int LowIndex = BrinellNumbers.List.IndexOf(LowIndexValue);
                int HighIndex = BrinellNumbers.List.IndexOf(HighIndexValue);

                this.LoWarn = BrinellNumbers.List[LowIndex + 1].ToString().PadLeft(7);
                this.HiWarn = BrinellNumbers.List[HighIndex - 1].ToString().PadLeft(7);

                if (p.Product == "000-DAILY-SCOPE-CHECK")
                {
                    this.PartNumber = ("000DAILYSCOPECHECK").PadRight(20).Substring(0, 20);
                    this.HiWarn = (p.RangeHigh.Value - 1).ToString().PadLeft(7);
                    this.LoWarn = (p.RangeLow.Value + 1).ToString().PadLeft(7);
                }

                this.FileComment = this.FileComment.PadRight(80).Substring(0, 80);
            }

            public override string ToString()
            {
                return
                FileSignatureVersion + Comma + RecordCount + Comma + RecordLength + Comma + ExtraHeaderLength + Comma + AvgCnt + Comma +
                ConvertScale + Comma + LoadTime + Comma + Flags + Comma + Header2Len + Comma + ExtraFieldDefLen +
                Comma + MaxStdDev + Comma + Objective + Comma + Load + Comma + Ball + CRLF +
                Scale + Comma + Label1 + Comma + LowTol + Comma + Label2 + Comma + LoWarn + Comma +
                Label3 + Comma + HiWarn + Comma + Label4 + Comma + HiTol + CRLF +
                FileComment + CRLF +
                DateCodeLabel + Comma + DateCode + CRLF +
                PartNumberLabel + Comma + PartNumber + CRLF +
                PartNameLabel + Comma + PartName + CRLF +
                CustomerLabel + Comma + Customer + CRLF +
                SpecLabel + Comma + Specification + CRLF +
                RangeLabel + Comma + Range + CRLF +
                Comment1Label + Comma + blank1Comment + CRLF +
                Comment2Label + Comma + blank2Comment + CRLF +
                Blank1Label + Comma + blank1blank + CRLF +
                Blank2Label + Comma + blank2blank + CRLF +
                DataLine + CRLF;
            }

        }
    }


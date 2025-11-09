using OpenIso8583Net;
using OpenIso8583Net.MasterCardPDS;
using System.Reflection;
using System.Text;


namespace ISO8583Display
{
#pragma warning disable S3776 // Refactor this method to reduce its Cognitive Complexity
    /// <summary>
    /// takes the content from a MasterCard clearing file and displays it to screen
    /// can do ASCII and EBCDIC files...
    /// </summary>
    internal class Program
    {
        protected Program() { }


        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                DisplayHelp();
                return 1;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            CommandLineOptions opts;
            try
            {
                opts = new CommandLineOptions(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                DisplayHelp();
                return 2;
            }

            List<string> merchantNames = new List<string>();

            long clearingRecCount = 0;
            decimal clearingTotal = 0.0m;
            long collectionsRecCount = 0;
            decimal collectionsTotal = 0.0m;
            DateOnly FileDate = DateOnly.MinValue;
            int SequenceNumber = 0;
            string pds105 = "";
            List<string> ReversalFiles = new List<string>();

            if (opts.ClearingTotals && opts.CSV)
                Console.WriteLine("FileName,FileID_PDS0105,FileSeq,Date,Records,Total,collectionsRecCount,collectionsTotal");

            var netSettlement = new List<NetSettlementDetails>();

            //--foreach (var f in opts.Files)
            for (int fileCnt = 0; fileCnt < opts.Files.Count; fileCnt++)
            {
                string f = opts.Files[fileCnt];
                object[] data;
                using (var fs = File.Open(f, FileMode.Open, FileAccess.Read))
                {
                    MasterCardFileEngine proc = (opts.AutoDetectFormat) ?
                        new MasterCardFileEngine() :
                        new MasterCardFileEngine(opts.BlockingMode, opts.RDWType, opts.HeaderType) { DefaultEncoding = opts.Encoding };
                    if (opts.MaxResultMessages != 0)
                        proc.StopParsingAfterRecords = opts.MaxResultMessages;

                    data = proc.ReadFile(fs);
                    int i = 0;

                    if (opts.AutoDetectFormat && !opts.CSV)
                    {
                        Console.WriteLine($"Autodetected : Blocking [{proc.Mode}], RDW [{proc.RDW}], Headers [{proc.Headers}], Encoding [{proc.DefaultEncoding}]");
                    }

                    if (opts.CSV && data.Length > 0 && data[0] is Iso8583MasterCard mm)
                        Console.WriteLine(mm.CSVHeader(opts.CSVFields));

                    if (!opts.PrintDetails && opts.CollectReversals)
                        Console.WriteLine($"({fileCnt} of {opts.Files.Count}) {f} - Checking {data.Length} records for reversals");


                    foreach (object o in data)
                    {
                        if (o is Iso8583MasterCard mc)
                        {
                            // does the file have any reversals
                            if (opts.CollectReversals && !ReversalFiles.Contains(f) && (mc.PDSFields.ContainsKey("0025") || mc.PDSFields.ContainsKey("0026")))
                            {
                                Console.WriteLine($"{f} has reversals - stopping");
                                ReversalFiles.Add(f);
                                break;
                            }

                            if (opts.PrintDetails)
                            {
                                if (opts.CSV)
                                    Console.WriteLine(mc.CSVRow(opts.CSVFields));
                                else
                                    Console.WriteLine(mc.ToString());
                                i++;
                            }

                            if (mc.MessageType == 1644
                                && mc.IsFieldSet(24)
                                && mc[24] == "697"
                                && mc.PDSFields.ContainsKey("0105"))
                            {
                                pds105 = mc.PDSFields["0105"];
                                var pds = Pds0105.Parse(mc.PDSFields["0105"]);
                                SequenceNumber = pds.SequenceNumber;
                                FileDate = pds.ReferenceDate;
                            }


                            if (opts.ClearingTotals
                                && mc.MessageType == 1240
                                && mc.IsFieldSet(24)
                                && mc[24] == "200"
                                && mc.IsFieldSet(4))
                            {
                                clearingRecCount++;
                                clearingTotal += (decimal.Parse(mc[4]!) / 100);

                                if (mc.PDSFields.ContainsKey("0165") && mc.PDSFields["0165"].StartsWith('C'))
                                {
                                    collectionsRecCount++;
                                    collectionsTotal += (decimal.Parse(mc[4]!) / 100);
                                }
                            }

                            if (opts.SettlementTotals
                                && mc.MessageType == 1644
                                && mc.IsFieldSet(24)
                                && mc[24] == "688")
                            {
                                var netSet = NetSettlementDetails.FromMasterCardMessage(mc);
                                netSettlement.Add(netSet);
                            }


                            if (opts.ClearingMerchants
                                && mc.IsFieldSet(43)
                                && !merchantNames.Contains(mc[43]!)
                                )
                                merchantNames.Add(mc[43]!);
                        }
                    }

                    if (opts.ClearingTotals)
                    {
                        if (!opts.CSV)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Clearing Totals");
                            Console.WriteLine("---------------");
                            Console.WriteLine($"Total Records       : {clearingRecCount}");
                            Console.WriteLine($"Total Value         : {clearingTotal:#0.00}");
                            Console.WriteLine($"Collections Records : {collectionsRecCount}");
                            Console.WriteLine($"Collections Value   : {collectionsTotal:#0.00}");
                        }
                        else
                        {
                            Console.WriteLine($"{Path.GetFileName(f)},MC-{pds105},{SequenceNumber},{FileDate:yy-MM-dd},{clearingRecCount},{clearingTotal},{collectionsRecCount},{collectionsTotal}");
                        }
                    }



                    if (opts.ClearingMerchants)
                        DisplayMerchants(merchantNames);
                }

                // if we're outputting the file
                if (!string.IsNullOrWhiteSpace(opts.OutputFile))
                {


                    // make reversal?
                    if (opts.OutputFileReversal && data[0] is Iso8583MasterCard mc && mc.MessageType == 1644 && mc[24] == "697")
                    {
                        DateOnly originalDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
                        Pds0105 fileID;
                        if (mc.PDSFields.ContainsKey("0105")) // file id
                        {
                            fileID = Pds0105.Parse(mc.PDSFields["0105"]);
                            originalDate = fileID.ReferenceDate;
                            // change the file date to today 
                            fileID.ReferenceDate = DateOnly.FromDateTime(DateTime.Now);
                            if (opts.OutputFileSequence != 0)
                                fileID.SequenceNumber = opts.OutputFileSequence;
                            mc.PDSFields["0105"] = fileID.ToMsg();

                            if (mc.PDSFields.ContainsKey("0110"))
                                mc.PDSFields["0110"] = fileID.ToMsg();

                            if (data[data.Length - 1] is Iso8583MasterCard footer
                                && footer.MessageType == 1644 && footer[24] == "695"
                                && footer.PDSFields.ContainsKey("0105"))
                            {
                                footer.PDSFields["0105"] = fileID.ToMsg();
                            }
                        }



                        if (mc.PDSFields.ContainsKey("0026")) // FILE reversal
                        {
                            if (opts.PrintDetails)
                            {
                                var col = Console.ForegroundColor;
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("FILE IS ALREADY REVERSAL FILE");
                                Console.ForegroundColor = col;
                            }
                            mc.PDSFields["0026"] = $"R{originalDate:yyMMdd}";
                        }
                        else
                        {
                            mc.PDSFields.Add("0026", $"R{originalDate:yyMMdd}");
                        }
                    }


                    string fn = opts.OutputFile.Replace("{fullfilename}", f).Replace("{filename}", Path.GetFileName(f)).Replace("{date}", DateTime.Now.ToString("yyyy-MM-dd")).Replace("{time}", DateTime.Now.ToString("HHmmss"));
                    using (var streamOut = new FileStream(fn, FileMode.Create))
                    {
                        using (var bw = new BinaryWriter(streamOut))
                        {
                            var writer = new MasterCardFileEngine(opts.OutputBlockingMode, opts.OutputRDW, opts.OutputHeaderType);
                            writer.WriteStream(bw, data, opts.OutputEncoding);
                        }
                    }
                }

                clearingRecCount = 0;
                clearingTotal = 0.0m;
                collectionsRecCount = 0;
                collectionsTotal = 0.0m;
            }


            if (opts.CollectReversals)
            {
                Console.WriteLine("");
                Console.WriteLine("Reversal Files");
                Console.WriteLine("--------------");
                foreach (var rev in ReversalFiles)
                    Console.WriteLine(rev);
            }

            if (opts.SettlementTotals && opts.CSV)
            {

                Dictionary<string, Tuple<decimal, DateOnly?, decimal?, decimal?>> collections = new();
                foreach (var ns in netSettlement.Where(w => w.Activity == "A"))
                {
                    if (collections.ContainsKey(ns.MasterCardFile!))
                        collections[ns.MasterCardFile!] = new Tuple<decimal, DateOnly?, decimal?, decimal?>(collections[ns.MasterCardFile!].Item1 + ns.AmountNetTransaction!.Value, ns.SettlementDate, collections[ns.MasterCardFile!].Item3 + ns.AmountNetTotal, collections[ns.MasterCardFile!].Item4 + ns.AmountNetFee);
                    else
                        collections.Add(ns.MasterCardFile!, new Tuple<decimal, DateOnly?, decimal?, decimal?>(ns.AmountNetTransaction!.Value, ns.SettlementDate, ns.AmountNetTotal, ns.AmountNetFee));
                }

                Console.WriteLine("MasterCardFile,seq,txnTotal,settlementDate,NetTotal,NetFees");
                foreach (var ns in collections)
                {
                    Console.WriteLine($"MC-{ns.Key},{int.Parse(ns.Key[^5..])},{ns.Value.Item1},{ns.Value.Item2},{ns.Value.Item3},{ns.Value.Item4}");
                }
            }
            return 0;
        }

        private static void DisplayMerchants(List<string> merchantNames)
        {
            Console.WriteLine("");
            Console.WriteLine("Merchant Names");
            Console.WriteLine("--------------");
            int invalidCnt = 0;
            StringBuilder line1 = new();
            StringBuilder line2 = new();

            foreach (string merch in merchantNames)
            {
                if (!IsValid(merch))
                {
                    Console.WriteLine(merch);
                    byte[] rawData = ASCIIEncoding.ASCII.GetBytes(merch);
                    foreach (var b in rawData)
                    {
                        line1.Append(ASCIIEncoding.ASCII.GetString(new byte[] { b }) + " ");
                        line2.Append(b.ToString("x2"));
                    }
                    Console.WriteLine($"[{line1.ToString()}]");
                    Console.WriteLine($"[{line2.ToString()}]");

                    invalidCnt++;
                }
            }
            if (invalidCnt == 0)
                Console.WriteLine("All Merchants valid");
            Console.WriteLine("--------------");
        }

        private static void DisplayHelp()
        {
            string appName = $"ISO8583 Display v{Assembly.GetExecutingAssembly().GetName().Version}";
            Console.WriteLine("".PadRight(appName.Length, '='));
            Console.WriteLine(appName);
            Console.WriteLine("".PadRight(appName.Length, '='));
            Console.WriteLine();
            Console.WriteLine("command line parameters :");
            Console.WriteLine("-------------------------");
            Console.WriteLine(" -p Profile, can be 'mc', 'MasterCard' for MasterCard or 'bs', 'bankserv' for Bankserv, 'mf' or 'cmm' for Microfocus formatted files");
            Console.WriteLine("");
            Console.WriteLine(" -e (optional) file is EBCDIC");
            Console.WriteLine(" -enc <codepage> (optional) ");
            Console.WriteLine(" -bf (optional) Variable Length Record Blocking on File");
            Console.WriteLine(" -bm (optional) Variable Length Record Blocking on MIP");
            Console.WriteLine(" -bn (optional) No Variable Length Record");
            Console.WriteLine(" -a (optional) file is ASCII (assumed if -e or -a not specified)");
            Console.WriteLine(" -rdw (optional) file has RDW (4 bytes before each record)");
            Console.WriteLine(" -rdwn (optional) file does not contain RDW (4 bytes before each record)");
            Console.WriteLine(" -h <headerType> (optional) header type. expects bs for BankServ or mf for Micro Focus / CMM format");
            Console.WriteLine(" -ct (optional) Calculate Clearing Totals (#Recs, Sum Amount) (1240-200 records)");
            Console.WriteLine(" -st (optional) Calculate Settlement Totals (1644-688 records) ");
            Console.WriteLine(" -csv (optional) outputs the clearing totals in CSV format");
            Console.WriteLine(" -csd (optional) outputs the clearing totals in CSV format, with field specification after");
            Console.WriteLine("      field specification is a comma seperated list of field or pds numbers. e.g. the default of");
            Console.WriteLine("      \"F24,P165,F71,F12,F2,F23,F14,F49,P148,F4,F37,F38,P158,F41,F42,F43\"");
            Console.WriteLine("      will return :");
            Console.WriteLine("      \"Message Type,PDS 165,Function Code,Message No,Local Txn Time,PAN,Card Seq No,Exp Date,Txn Currency Code,PDS 148,Txn Amount,Ret Ref No,Approval Code,PDS 158,Terminal ID,Card Acceptor ID,Card Name Locations\"");
            Console.WriteLine("      ");
            Console.WriteLine("      to modify the default, add an asterisk to the start of the -csd definition, using a + or - to add or remove that field");
            Console.WriteLine("      e.g. \"*+P165,-F24,F2\" will modify the default, removing field F24 from the output and add PDS 0165, and F2 to the end of the CSV output");
            Console.WriteLine(" -cm (optional) Clearing Check Merchant Names");
            Console.WriteLine(" -dn or -q (optional) details none or quiet mode - don't output the record details to console");
            Console.WriteLine(" -max <number> (optional) returns the maximum number of records from the file being parsed");
            Console.WriteLine("");
            Console.WriteLine("one of the following :-");
            Console.WriteLine(" -f <folder\\*.*>  the folder and files you want to process (wild cards apply)");
            Console.WriteLine(" <fileName>   the file you want to display the contents of");
            Console.WriteLine("");
            Console.WriteLine("Defaults");
            Console.WriteLine(" -e");
            Console.WriteLine(" -bf");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine(" Output Options :");
            Console.WriteLine(" ----------------");
            Console.WriteLine(" -obn : output not in 1014 length blocks");
            Console.WriteLine(" -obf : output Variable Length Record Blocking on File, (1014 length blocks with 2 empty/null bytes at the end)");
            Console.WriteLine(" -obm : output Variable Length Record Blocking on MIP, (1014 length blocks with 2 empty/null bytes at the start with the value 0x03, 0xF6)");
            Console.WriteLine(" -oe : output file is EBCDIC");
            Console.WriteLine(" -oa : output file is ASCII");
            Console.WriteLine(" -oenc <codepage> : output encoding with the code page that should be used to encode");
            Console.WriteLine(" -orev : adds a file reversal to the header (PDS 0025) using the file ReferenceDate (PDS 0105, S2) as subfield 2 ");
            Console.WriteLine(" -orevseq <number> : the file sequence number to use (incase the original file date is the same as todays date)");
            Console.WriteLine(" -ordw : output file has RDW (4 bytes before each record)");
            Console.WriteLine(" -ordwn : output file does not contain RDW (4 bytes before each record)");
            Console.WriteLine(" -oh <headerType> : output header type. expects bs for BankServ or mf for Micro Focus / CMM format");
            Console.WriteLine(" -o <fileName> : the output file name, you can use {fullfilename} or {filename} to specify the original file name and add to it");
            //Console.WriteLine(" -os <sequenceNumber> : update the sequence number to the one specified");
            //Console.WriteLine(" -od <date> : date in the format yyyyMMdd");
            Console.WriteLine();
            Console.WriteLine(" for CSV output, where the value is secured (e.g. truncated PAN), you can specify an I instead of F to get the unsecured/raw value");
            Console.WriteLine();
        }

        private static bool IsValid(string value)
        {
            int part = 0;
            foreach (int b in value)
            {
                if (b == '\\')
                    part++;

                if (b < ' ' || b > ' ' && b < '0' && b != '\'' && b != '&' && b != '-' && b != '*' && b != '.' && b != '/' || b > '9' && b < 'A' || b > 'Z' && b < 'a' && b != '\\' || b > 'z')
                    return false;
            }

            if (part > 3)
                return false;
            return true;
        }
    }

    public static class ObjectCsv
    {
        public static string CSVHeader(this object obj, List<Tuple<char, int>> fields)
        {
            StringBuilder sb = new StringBuilder();
            if (obj is AMessage msg)
            {
                List<string> headers = new List<string>() { "Mesage Type" };
                foreach (var fld in fields)
                {
                    switch (fld.Item1)
                    {
                        case 'I':
                        case '#':
                        case 'F':
                            IFieldDescriptor? fieldDescriptor = msg.FieldInfo(fld.Item2);
                            if (fieldDescriptor != null && !string.IsNullOrWhiteSpace(fieldDescriptor.DisplayName))
                                headers.Add(fieldDescriptor.DisplayName);
                            else
                                headers.Add($"Field {fld.Item2}");
                            break;
                        case 'P':
                            headers.Add($"PDS {fld.Item2}");
                            break;
                        default:
                            headers.Add($"{fld.Item1} {fld.Item2}");
                            break;
                    }
                }
                return string.Join(',', headers);
            }

            Type typ = obj.GetType();
            PropertyInfo[] props = typ.GetProperties();

            foreach (var prop in props)
            {
                sb.Append(prop.Name);
                sb.Append(',');
            }
            string header = sb.ToString();
            return header.Substring(0, header.Length - 1);
        }

#pragma warning disable S3776 // Refactor this method to reduce its Cognitive Complexity
        public static string CSVRow(this object obj, List<Tuple<char, int>> fields)
#pragma warning restore S3776 // Refactor this method to reduce its Cognitive Complexity
        {
            if (obj is AMessage msg)
            {
                List<string> values = new List<string>();

                if (obj is Iso8583MasterCard mca)
                    values.Add(mca.MessageType.ToString());
                if (obj is Iso8583 gen)
                    values.Add(gen.MessageType.ToString());

                foreach (var fld in fields)
                {
                    string? v = null;
                    switch (fld.Item1)
                    {
                        case 'F':
                            switch (fld.Item2)
                            {
                                case 2: // PAN
                                    v = msg[fld.Item2]; // truncate pan
                                    if (!string.IsNullOrWhiteSpace(v) && v.Length > 10)
                                        v = v[0..8] + "*".PadLeft(v.Length - 10, '*') + v[^2..];
                                    break;
                                case 12: // Local Txn Time
                                    v = msg[fld.Item2]; // truncate pan
                                    if (!string.IsNullOrWhiteSpace(v) && v.Length == 12)
                                        v = $"{v[0..2]}/{v[2..4]}/{v[4..6]} {v[6..8]}:{v[8..10]}:{v[10..12]}";
                                    break;
                                default:
                                    v = msg[fld.Item2];
                                    break;
                            }
                            break;
                        case 'I':
                            v = msg[fld.Item2];
                            break;
                        case '#':
                            v = "#" + msg[fld.Item2];
                            break;
                        case 'P':
                            if (obj is Iso8583MasterCard mc)
                            {
                                string key = $"{fld.Item2:0000}";
                                if (mc.PDSFields.ContainsKey(key))
                                    v = mc.PDSFields[key];
                            }
                            if (obj is Iso8583Post pstl)
                                v = pstl.Private[fld.Item2];
                            break;
                    }
                    values.Add(v.CsvValue() ?? string.Empty);
                }
                return string.Join(',', values);
            }

            StringBuilder sb = new StringBuilder();
            Type typ = obj.GetType();
            PropertyInfo[] props = typ.GetProperties();

            foreach (var prop in props)
            {
                string val = prop.GetValue(obj)?.ToString() ?? "";
                sb.Append(val.CsvValue());
                sb.Append(',');
            }
            string row = sb.ToString();
            row = row.Substring(0, row.Length - 1);
            return row;
        }

        public static string CsvValue(this string? val)
        {
            if (val == null)
                return string.Empty;

            if (val.Contains(',') || val.Contains('\"') || val.Contains('\r') || val.Contains('\n'))
                return '\"' + val.Replace("\"", "\"\"") + "\"";
            return val;
        }
    }

    internal class CommandLineOptions
    {
        public CommandLineOptions() { }

#pragma warning disable S3776 // Refactor this constructor to reduce its Cognitive Complexity
        public CommandLineOptions(string[] args)
#pragma warning restore S3776 // Refactor this constructor to reduce its Cognitive Complexity
        {
            ApplyProfile(args);

            // apply any overrides
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

#pragma warning disable S1479 // Consider reworking this 'switch' to reduce the number of 'case's to at most 30
                switch (arg.ToLower())
#pragma warning restore S1479 // Consider reworking this 'switch' to reduce the number of 'case's to at most 30
                {
                    case "-a":
                        Encoding = Encoding.ASCII;
                        AutoDetectFormat = false;
                        break;
                    case "-e":
                        Encoding = Encoding.GetEncoding(1140);
                        AutoDetectFormat = false;
                        break;
                    case "-enc":
                        AutoDetectFormat = false;
                        Encoding = GetEncoding(args, i);
                        i++;
                        break;
                    case "-f":
                        GetFiles(args, i);
                        i++;
                        break;
                    case "-filter":
                        if ((args.Length < i + 1) || args[i + 1].StartsWith('-'))
                            throw new ArgumentException("-filter specified without a filter parameter");
                        Filter = args[i + 1];
                        i++;
                        break;
                    case "-p": // we just move on, as we applied this first before the rest of the parameters
                        i++;
                        AutoDetectFormat = false;
                        break;
                    case "-bf": AutoDetectFormat = false; BlockingMode = BlockingType.File; break;
                    case "-bm": AutoDetectFormat = false; BlockingMode = BlockingType.MIP; break;
                    case "-bn": AutoDetectFormat = false; BlockingMode = BlockingType.None; break;
                    case "-rdw": AutoDetectFormat = false; RDWType = RDWType.Default; break;
                    case "-rdwle": AutoDetectFormat = false; RDWType = RDWType.LittleEndian; break;
                    case "-rdwbe": AutoDetectFormat = false; RDWType = RDWType.BigEndian; break;
                    case "-rdwn": AutoDetectFormat = false; RDWType = RDWType.None; break;
                    case "-rdwmf": AutoDetectFormat = false; RDWType = RDWType.Microfocus; break;
                    case "-h":
                        AutoDetectFormat = false;
                        HeaderType = GetHeader(args, i);
                        i++;
                        break;
                    case "-csv": CSV = true; break;
                    case "-csd":
                        CSV = true;
                        GetCSVFields(args, i);
                        i++;
                        break;
                    case "-ct": ClearingTotals = true; break;
                    case "-cm": ClearingMerchants = true; break;
                    case "-m":
                    case "-max":
                        if ((args.Length < i + 1) || args[i + 1].StartsWith('-'))
                            throw new ArgumentException("-max specified without a positive number of records to read from the file");
                        i++;
                        if (int.TryParse(args[i], out int recs))
                            MaxResultMessages = recs;
                        else
                            throw new ArgumentException($"-max specified without a positive number of records to read from the file got [{args[i]}] instead");
                        break;
                    case "-q":
                    case "-dn": PrintDetails = false; break;
                    case "-st": SettlementTotals = true; break;

                    case "-hasreversals": CollectReversals = true; break;

                    // ----------------------------------------------------
                    // output options...
                    // ----------------------------------------------------
                    case "-obn": OutputBlockingMode = BlockingType.None; break;
                    case "-obf": OutputBlockingMode = BlockingType.File; break;
                    case "-obm": OutputBlockingMode = BlockingType.MIP; break;

                    case "-oe": OutputEncoding = Encoding.GetEncoding(1140); break;
                    case "-oa": OutputEncoding = Encoding.ASCII; break;
                    case "-oenc": OutputEncoding = GetEncoding(args, i); i++; break;

                    case "-of":
                        if ((args.Length < i + 1) || args[i + 1].StartsWith('-'))
                            throw new ArgumentException("-of specified without the output file name");
                        i++;
                        OutputFile = args[i];
                        break;

                    case "-oh":
                        OutputHeaderType = GetHeader(args, i);
                        i++;
                        break;

                    case "-op": // we just move on, as we applied this first before the rest of the parameters
                        i++;
                        break;

                    case "-orev": OutputFileReversal = true; break;
                    case "-orevseq":
                    case "-os":
                        if (args[i] == "-orevseq")
                            OutputFileReversal = true;
                        if ((args.Length < i + 1) || args[i + 1].StartsWith('-') || !int.TryParse(args[i + 1], out int seq))
                            throw new ArgumentException("-orevseq specified without a number following it");
                        if (seq < 1 || seq > 99999)
                            throw new ArgumentException("-orevseq sequence number needs to be between 1 and 99999 (including)");
                        OutputFileSequence = seq;
                        i++;
                        break;

                    case "-ordw": OutputRDW = RDWType.Default; break;
                    case "-ordwle": OutputRDW = RDWType.LittleEndian; break;
                    case "-ordwbe": OutputRDW = RDWType.BigEndian; break;
                    case "-ordwn": OutputRDW = RDWType.None; break;


                    default: Files.Add(arg); break;
                }
            }



            // check we have a file name
            if (Files.Count == 0)
                throw new ArgumentException("No Filename or folder found");

            // if we don't have what we need, use defaults (MC profile)
            if (Encoding == null)
                ApplyProfile("mc");
        }

        private static MasterCardFileEngine.HeaderTypes GetHeader(string[] args, int i)
        {
            if ((args.Length < i + 1) || args[i + 1].StartsWith('-'))
                throw new ArgumentException("header parameter missing it's value");

            switch (args[i + 1].ToLower())
            {
                case "bs": return MasterCardFileEngine.HeaderTypes.BankServ;
                case "cmm":
                case "mf": return MasterCardFileEngine.HeaderTypes.MicroFocus;
                case "none": return MasterCardFileEngine.HeaderTypes.None;
                default: throw new ArgumentOutOfRangeException($"header expects a value of either bs or mf (bankserv or microfocus) '{args[i]}' is not valid");
            }
        }

#pragma warning disable S3776 // Refactor this method to reduce its Cognitive Complexity to the 15 allowed.
        private void GetCSVFields(string[] args, int i)
#pragma warning restore S3776 // Refactor this method to reduce its Cognitive Complexity to the 15 allowed.
        {
            string def = args[i + 1];

            if (!def.StartsWith('*'))
                CSVFields.Clear();
            else
                def = def[1..]; // remove the * from the start of the string

            var fields = new List<string>();
            fields.AddRange(def.ToUpper().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()));

            for (int cnt = 0; cnt < fields.Count; cnt++)
            {
                string fld = fields[cnt];
                bool tobeRemoved = false;
                if (fld.StartsWith('-')) // are we removing or adding the field
                {
                    fld = fld[1..]; // remove first char
                    tobeRemoved = true;
                }
                if (fld.StartsWith('+'))
                {
                    fld = fld[1..]; // remove first char
                }

                Tuple<char, int> csvField;

                if (int.TryParse(fld, out int val))
                    csvField = new Tuple<char, int>('F', val);
                else
                {

                    if (!(fld.StartsWith('F') || fld.StartsWith('I') || fld.StartsWith('P') || fld.StartsWith('#')))
                        throw new ArgumentException("-csd field specified without a (P)ds, (F)ield, (I)nsecure or # preceding it");
                    if (int.TryParse(fld[1..], out int fv))
                        csvField = new Tuple<char, int>(fld[0], fv);
                    else
                        throw new ArgumentException("-csd field specified without a number after the (P)ds or (F)ield");
                }

                if (csvField != null)
                {

                    if (CSVFields.Contains(csvField) && tobeRemoved)
                        CSVFields.Remove(csvField);
                    if (!tobeRemoved)
                        CSVFields.Add(csvField);
                }
            }
        }

        private static Encoding GetEncoding(string[] args, int i)
        {
            if ((args.Length < i + 1) || args[i + 1].StartsWith('-'))
                throw new ArgumentException("-enc specified without the encoding (positive) number that should follow");
            if (int.TryParse(args[i + 1], out int enc))
                return Encoding.GetEncoding(enc);
            else
                throw new ArgumentException($"-enc specified, but the value [{args[i + 1]}] is not a number");
        }

        private void GetFiles(string[] args, int i)
        {
            if ((args.Length < i + 1) || args[i + 1].StartsWith('-'))
                throw new ArgumentException("-f specified without the folder");
            string dir = args[i + 1];

            if (Directory.Exists(Path.GetDirectoryName(dir)))
            {
                var directorySelected = new DirectoryInfo(Path.GetDirectoryName(dir)!);
                var files = directorySelected.GetFiles(Path.GetFileName(dir));
                foreach (var f in files)
                    Files.Add(f.FullName);
            }
        }

        void ApplyProfile(string[] args)
        {
            // get the profile and apply it, the rest of the parameters override the profile
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-profile" || args[i].ToLower() == "-p")
                {
                    if ((args.Length < i + 1) || args[i + 1].StartsWith('-'))
                    {
                        throw new ArgumentException("-p specified without a profile name");
                    }
                    ApplyProfile(args[i + 1]);
                    break;
                }

                if (args[i].ToLower() == "-op")
                {
                    if ((args.Length < i + 1) || args[i + 1].StartsWith('-'))
                    {
                        throw new ArgumentException("-op specified without a profile name");
                    }
                    ApplyOutputProfile(args[i + 1]);
                }
            }
        }

        private void ApplyProfile(string profile)
        {
            if (string.IsNullOrWhiteSpace(profile))
                return;

            switch (profile.ToLower())
            {
                case "mastercard":
                case "mc":
                    Encoding = Encoding.ASCII;
                    RDWType = RDWType.Default;
                    HeaderType = MasterCardFileEngine.HeaderTypes.None;
                    BlockingMode = BlockingType.File;
                    break;

                case "bankserv":
                case "bs":
                    Encoding = Encoding.ASCII;
                    RDWType = RDWType.None;
                    HeaderType = MasterCardFileEngine.HeaderTypes.BankServ;
                    BlockingMode = BlockingType.None;
                    break;
                case "mf":
                case "microfocus":
                case "cmm":
                    Encoding = Encoding.ASCII;
                    RDWType = RDWType.Microfocus;
                    HeaderType = MasterCardFileEngine.HeaderTypes.MicroFocus;
                    BlockingMode = BlockingType.None;
                    break;
                default:
                    throw new ArgumentException($"unknown profile [{profile}] requested.");
            }
        }

        private void ApplyOutputProfile(string profile)
        {
            if (string.IsNullOrWhiteSpace(profile))
                return;

            OutputFile = "{fullfilename}." + profile.Trim() + ".out";
            switch (profile.ToLower())
            {
                case "mastercard":
                case "mc":
                    OutputEncoding = Encoding.ASCII;
                    OutputBlockingMode = BlockingType.File;
                    break;

                case "bankserv":
                case "bs":
                    OutputEncoding = Encoding.ASCII;
                    OutputRDW = RDWType.None;
                    OutputHeaderType = MasterCardFileEngine.HeaderTypes.BankServ;
                    OutputBlockingMode = BlockingType.None;
                    break;
                case "mf":
                case "microfocus":
                case "cmm":
                    OutputEncoding = Encoding.ASCII;
                    OutputRDW = RDWType.Default;
                    OutputHeaderType = MasterCardFileEngine.HeaderTypes.MicroFocus;
                    OutputBlockingMode = BlockingType.None;
                    break;
                default:
                    throw new ArgumentException($"unknown profile [{profile}] requested.");
            }
        }

        public BlockingType BlockingMode { get; set; } = BlockingType.None;

        public RDWType RDWType { get; set; }

        public MasterCardFileEngine.HeaderTypes HeaderType { get; set; } = MasterCardFileEngine.HeaderTypes.None;

        public Encoding Encoding { get; set; } = Encoding.GetEncoding(1140); // default to Ebcdic

        public bool ClearingTotals { get; set; } = false;
        public bool ClearingMerchants { get; set; } = false;
        public bool SettlementTotals { get; set; } = false;

        public string Filter { get; set; } = string.Empty;

        public bool PrintDetails { get; set; } = true;

        public bool CSV { get; set; } = false;

        public List<Tuple<char, int>> CSVFields { get; set; } = new List<Tuple<char, int>>
        {
            new Tuple<char, int>('F',24), // Function Code
            new Tuple<char, int>('P',165),// Settlement
            new Tuple<char, int>('F',71), // Message No
            new Tuple<char, int>('F',12), // Local Txn Time
            new Tuple<char, int>('F',2),  // PAN
            new Tuple<char, int>('F',23), // Card Seq No
            new Tuple<char, int>('F',14), // Exp Date

            new Tuple<char, int>('F',49), // Txn Currency Code
            new Tuple<char, int>('P',148),// Currency / exponents
            new Tuple<char, int>('F',4),  // Txn Amount

            new Tuple<char, int>('F',37), // Retrieval ref number
            new Tuple<char, int>('F',38), // Approval Code
            new Tuple<char, int>('P',158), //  Business Activity

            new Tuple<char, int>('F',41), // Terminal ID
            new Tuple<char, int>('F',42), // Card Acceptor ID
            new Tuple<char, int>('F',43), // Card Name Locations
        };

        public int MaxResultMessages { get; set; }

        public List<string> Files { get; set; } = new List<string>();

        public string OutputFile { get; set; } = string.Empty;

        public string OutputProfile { get; set; } = string.Empty;
        public MasterCardFileEngine.HeaderTypes OutputHeaderType { get; private set; } = MasterCardFileEngine.HeaderTypes.None;
        public Encoding? OutputEncoding { get; set; } = null;
        public BlockingType OutputBlockingMode { get; set; } = BlockingType.None;
        public RDWType OutputRDW { get; set; } = RDWType.Default;
        public bool OutputFileReversal { get; set; } = false;
        public int OutputFileSequence { get; set; } = 0;
        public bool AutoDetectFormat { get; set; } = true;
        public bool CollectReversals { get; set; } = false;
    }
#pragma warning restore S3776 // Refactor this method to reduce its Cognitive Complexity
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sheetmaker
{
    class Program
    {
        static List<Dive> dives;
        static List<Diver> divers;
        static String[][] sheet;
        static String EventName;
        static String board;
	static String testing;
        static string dirname;
        static void Main(string[] args)
        {
		testing = "elephant";
		Console.WriteLine(testing);
	        string init = Console.ReadLine();
			if (init.Trim().ToLower() == "batch") {
				Log("Creating Dives");
				dives = new List<Dive>();
				divers = new List<Diver>();
				String[] divelistb = File.ReadAllText("divelist.csv").Split('\n');
				File.AppendAllText("csvlog.log", "divelist.csv");
				for (int i = 1; i < divelistb.Length; i++) dives.Add(new Dive(divelistb[i]));
				start:
				String filenamebx = "nulll";
					DirectoryInfo d = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
					string csvlog = File.ReadAllText("csvlog.log");
					foreach (var file in d.GetFiles("*.csv"))
					{
						if (!csvlog.Contains(file.Name))
						{
							filenamebx = file.Name;
							File.AppendAllText("csvlog.log", file.Name);
							break;
						}
					}
				if (filenamebx == "null")
				{
					Log("FINISHED");
					Console.ReadLine();
					Environment.Exit(0);
				}
				Log("Filename: " + filenamebx);
				string csvfileb = File.ReadAllText(filenamebx);
				Log("Creating sheet");
				string[] rowsb = csvfileb.Split('\n');
				sheet = new String[rowsb.Length][];
				for (int i = 0; i < sheet.Length; i++)
				{
					sheet[i] = rowsb[i].Split(',');
				}
				divers = new List<Diver>();
				Console.WriteLine("What is the event called?");
				EventName = Console.ReadLine().Trim();
				Console.WriteLine("What is the height of the board?(In the format of 1M, or 3M)");
				board = Console.ReadLine().ToUpper();
				Console.WriteLine("What are the names of the divers?(Seperate names by commas or you can do ranges like *,David)");
				string nameinputb = Console.ReadLine();
				string[] namesb = nameinputb.Split(',');
				dirname = EventName.Replace(" ", "") + "-" + board;
				Directory.CreateDirectory(dirname);
				Log("Getting all divers");
				Log("Stopping at " + namesb[1]);
				if (namesb[0].Trim() == "*")
				{
					for (int i = 0; i < sheet.Length; i++)
					{
						if (sheet[i].Length == 0) ;
						else if (sheet[i][0].Trim().Equals(namesb[1].Trim()))
						{
							divers.Add(CreateDiver(namesb[1].Trim()));
							Error("Should be breaking!");
							break;
						}
						else if (sheet[i][0].Trim() != "")
						{
							divers.Add(CreateDiver(sheet[i][0].Trim()));
						}
					}
				}
				else
				{
					foreach (string x in namesb)
					{
						divers.Add(CreateDiver(x.Trim()));
					}
				}
				Console.WriteLine("1. Generate Announcement Reports\n2. Generate Score Reports");
				int choiceb = Int32.Parse(Console.ReadLine());
				if (choiceb == 2)
				{
					Log("Generating Reports(This will take a while)");
					CreateResults();
					for (int i = 0; i < divers.Count; i++)
					{
						divers[i].GenerateReport();
					}
					CreateCombinedPDF();
				}
				else if (choiceb == 1)
				{
					Log("Generating Reports(This will take a while)");
					string template = File.ReadAllText("AnnouncementTemplate.tex");
					template = template.Replace("//Event//", EventName + " - " + board);
					template = template.Replace("//Diver Count//", divers.Count + "");
					string list = "";
					for (int i = 0; i < divers.Count; i++)
					{
						string listtemp = File.ReadAllText("DivelistTemplate.tex");
						listtemp = listtemp.Replace("//Diver Name//", divers[i].Name);
						string data = "";
						for (int j = 0; j < divers[i].Dives.Length; j++)
						{
							data += (j + 1) + "&" + divers[i].Dives[j].Code + "&" + divers[i].Dives[j].Description + @"&\underline{\hspace{1cm}}\\" + "\n";
						}
						listtemp = listtemp.Replace("//Data//", data);
						list += listtemp;
					}
					template = template.Replace("//List//", list);
					string filename = dirname + "/AnnouncersList";
					File.WriteAllText(filename + ".tex", template);
					System.Diagnostics.Process.Start("CMD.exe", "/C pdflatex -output-directory=" + dirname + " " + filename + ".tex");
				}
				else { Error("Invalid Choice"); }
				Log("FINISHED");
				Console.ReadLine();
				goto start;
			}
	        else if(init.Trim().Length != 0){
                string template = File.ReadAllText("CombinedTemplate.tex");
                DirectoryInfo d = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory() + "/Reports");
                string combined = "";
                foreach(var dd in d.GetDirectories()){
                    foreach(var f in dd.GetFiles("*.pdf")){
                        combined += @"\includepdf{" + f.FullName.Replace(@"\", "/") + "}\n";
                    }
                }
                template = template.Replace("//Pdf includes//", combined);
                File.WriteAllText("combinedreports.tex",template);
                System.Diagnostics.Process.Start("CMD.exe", "/C pdflatex combinedreports.tex");
            }
            Log("Creating Dives");
            dives = new List<Dive>();
            divers = new List<Diver>();
            String[] divelist = File.ReadAllText("divelist.csv").Split('\n');
            File.AppendAllText("csvlog.log", "divelist.csv");
            for (int i = 1; i < divelist.Length; i++) dives.Add(new Dive(divelist[i]));
            restart:
            Console.WriteLine("What is the file name of the .csv file? or type auto to detect a new csv file");
            string choicex = Console.ReadLine().Trim();
            string filenamex = choicex;
            if (choicex.ToLower().Equals("auto"))
            {
                DirectoryInfo d = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
                string csvlog = File.ReadAllText("csvlog.log");
                foreach (var file in d.GetFiles("*.csv"))
                {
                    if (!csvlog.Contains(file.Name))
                    {
                        filenamex = file.Name;
                        File.AppendAllText("csvlog.log", file.Name);
                        break;
                    }
                }
            }
            string csvfile = File.ReadAllText(filenamex);
            Log("Creating sheet");
            string[] rows = csvfile.Split('\n');
            sheet = new String[rows.Length][];
            for (int i = 0; i < sheet.Length; i++)
            {
                sheet[i] = rows[i].Split(',');
            }divers = new List<Diver>();
            Console.WriteLine("What is the event called?");
            EventName = Console.ReadLine().Trim();
            Console.WriteLine("What is the height of the board?(In the format of 1M, or 3M)");
            board = Console.ReadLine().ToUpper();
            Console.WriteLine("What are the names of the divers?(Seperate names by commas or you can do ranges like *,David)");
            string nameinput = Console.ReadLine();
            string[] names = nameinput.Split(',');
            dirname = EventName.Replace(" ", "") + "-" + board;
            Directory.CreateDirectory(dirname);
            Log("Getting all divers");
            Log("Stopping at " + names[1]);
            if (names[0].Trim() == "*")
            {
                for (int i = 0; i < sheet.Length; i++)
                {
                    if (sheet[i].Length == 0) ;
                    else if (sheet[i][0].Trim().Equals(names[1].Trim()))
                    {
                        divers.Add(CreateDiver(names[1].Trim()));
                        Error("Should be breaking!");
                        break;
                    }
                    else if (sheet[i][0].Trim() != "")
                    {
                        divers.Add(CreateDiver(sheet[i][0].Trim()));
                    }
                }
            }
            else
            {
                foreach (string x in names)
                {
                    divers.Add(CreateDiver(x.Trim()));
                }
            }
            Console.WriteLine("1. Generate Announcement Reports\n2. Generate Score Reports");
            int choice = Int32.Parse(Console.ReadLine());
            if (choice == 2)
            {
                Log("Generating Reports(This will take a while)");
                CreateResults();
                for (int i = 0; i < divers.Count; i++)
                {
                    divers[i].GenerateReport();
                }
                CreateCombinedPDF();
            }
            else if (choice == 1)
            {
                Log("Generating Reports(This will take a while)");
                string template = File.ReadAllText("AnnouncementTemplate.tex");
                template = template.Replace("//Event//", EventName + " - " + board);
                template = template.Replace("//Diver Count//", divers.Count + "");
                string list = "";
                for (int i = 0; i < divers.Count; i++)
                {
                    string listtemp = File.ReadAllText("DivelistTemplate.tex");
                    listtemp = listtemp.Replace("//Diver Name//", divers[i].Name);
                    string data = "";
                    for (int j = 0; j < divers[i].Dives.Length; j++)
                    {
                        data += (j + 1) + "&" + divers[i].Dives[j].Code + "&" + divers[i].Dives[j].Description + @"&\underline{\hspace{1cm}}\\" + "\n";
                    }
                    listtemp = listtemp.Replace("//Data//", data);
                    list += listtemp;
                }
                template = template.Replace("//List//", list);
                string filename = dirname + "/AnnouncersList";
                File.WriteAllText(filename + ".tex", template);
                System.Diagnostics.Process.Start("CMD.exe", "/C pdflatex -output-directory=" + dirname + " " + filename + ".tex");
            }
            else { Error("Invalid Choice"); }
            Log("FINISHED");
            Console.ReadLine();
            goto restart;
        }

        static Diver CreateDiver(string divername)
        {
            Log("Searching ofr " + divername);
            int index = 0;
            for (int i = 0; i < sheet.Length; i++)
            {
                if (sheet[i][0].Trim().Equals(divername.Trim()))
                {
                    index = i;
                    break;
                }
            }
            List<Dive> currentdives = new List<Dive>();
            List<double[]> scores = new List<double[]>();
            bool stop = false;
            index++;
            while (!stop)
            {
                string divecode = sheet[index][1];
                if (divecode.Trim().Length == 0)
                {
                    stop = true;
                    break;
                }
                currentdives.Add(GetDive(divecode, board));
                double score1 = 0.0;
                double score2 = 0.0;
                double score3 = 0.0;
                double.TryParse(sheet[index][2].Trim(), out score1);
                double.TryParse(sheet[index][3].Trim(), out score2);
                double.TryParse(sheet[index][4].Trim(), out score3);
                scores.Add(new double[] { score1, score2, score3});
                index++;
            }
            return new Diver(divername, EventName, board, currentdives.ToArray(), scores.ToArray(), dirname);
        }

        /// <summary>
        /// This displays a log message in the console. It changes the background to white and the foreground to black. It changes it back to the original colors after it prints a line. 
        /// </summary>
        /// <param name="msg">This is the message to send</param>
        static void Log(string msg)
        {
            ConsoleColor oldBack = Console.BackgroundColor;
            ConsoleColor oldFore = Console.ForegroundColor;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(msg);
            Console.BackgroundColor = oldBack;
            Console.ForegroundColor = oldFore;
        }

        /// <summary>
        /// This displays an error message in the console. It changes the background to white and the foreground to red. It changes it back to the original colors after it prints a line. 
        /// </summary>
        /// <param name="msg">This is the message to send</param>
        static void Error(string msg)
        {
            ConsoleColor oldBack = Console.BackgroundColor;
            ConsoleColor oldFore = Console.ForegroundColor;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.BackgroundColor = oldBack;
            Console.ForegroundColor = oldFore;
        }


        /// <summary>
        /// Search for the dive that has the given dive code and boardHeight
        /// </summary>
        /// <param name="diveCode">The code of the dive. Captilization doesn't matter.</param>
        /// <param name="boardHeight">The height of the board in meters.</param>
        /// <returns>A Dive object that corresponds to the query or null if not found.</returns>
        static Dive GetDive(string diveCode, string boardHeight)
        {
            if (dives.Count == 0)
            {
                Error("Tried to get dive before dive list was initialized");
                return new Dive();
            }
            Dive d = dives.FirstOrDefault(x => x.Code == diveCode.ToUpper() && x.Board == boardHeight.ToUpper());
            if (d == null) d = new Dive();
            return d;
        }

        static void CreateCombinedPDF()
        {
            Thread.Sleep(5000);
            string filename = dirname + "/Combined";
            string template = File.ReadAllText("CombinedTemplate.tex");
            string includes = @"\includepdf{results.pdf}" + "\n";
            foreach (Diver d in divers)
            {
                includes += @"\includepdf{" + d.Pdf.Split('/')[d.Pdf.Split('/').Length - 1] + "}\n";
            }
            template = template.Replace("//Pdf includes//", includes);
            File.WriteAllText(filename + ".tex", template);
            System.Diagnostics.Process.Start("CMD.exe", "/C pdflatex -output-directory=" + dirname + " " + filename + ".tex");
            Thread.Sleep(2000);
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                WorkingDirectory = dirname,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal,
                FileName = "powershell.exe",
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            process.StartInfo = startInfo;
            process.Start();
            process.StandardInput.WriteLine("latexmk -c");
        }

        static void CreateResults()
        {
            string filename = dirname + "/Results";
            string template = File.ReadAllText("ResultsTemplate.tex");
            template = template.Replace("//Event//", EventName + " - " + board);
            template = template.Replace("//Diver Count//", "" + divers.Count);
            List<Diver> sorted = divers.OrderByDescending(o => o.TotalScore).ToList();
            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].Place = i + 1;
            }
            string results = "";
            for (int i = 0; i < sorted.Count; i++)
            {
                results += (i + 1) + "&" + sorted[i].Name + "&" + sorted[i].TotalScore + @"\\\midrule" + "\n";
            }
            divers = sorted;
            template = template.Replace("//Results//", results);
            File.WriteAllText(filename + ".tex", template);
            System.Diagnostics.Process.Start("CMD.exe", "/C pdflatex -output-directory=" + dirname + " " + filename + ".tex");
        }
    }

    class Dive
    {
        public string Code { get; }
        public string Board { get; }
        public string Description { get; }
        public Double DD { get; }
        public string DiveData { get; }
        public Dive()
        {
            this.Code = "000A";
            this.Board = "0M";
            this.Description = "INVALID DIVE";
            this.DD = 0.0;
            this.DiveData = "000A,0M,INVALID DIVE,0.0";
        }

        public Dive(String diveData)
        {
            this.DiveData = diveData;
            String[] data = diveData.Split(',');
            this.Code = data[0];
            this.Board = data[1];
            this.Description = data[2];
            this.DD = Double.Parse(data[3]);
        }

        public override string ToString()
        {
            return DiveData;
        }
    }

    class Diver
    {
        public string Name { get; set; }
        public string EventName { get; set; }
        public string Board { get; set; }
        public Dive[] Dives { get; set; }
        public double[][] Scores { get; set; }
        public double[][] SubScores { get; set; }
        public double TotalScore { get; set; }
        public string Pdf { get; set; }
        public int Place { get; set; }
        private string dirname { get; set; }

        public Diver(string name, string eventname, string board, Dive[] dives, double[][] Scores, string dirname)
        {
            this.dirname = dirname;
            this.Name = name;
            this.EventName = eventname;
            this.Board = board.ToUpper();
            this.Dives = dives;
            this.Scores = Scores;
            this.Pdf = "null";
            this.Place = 0;
            CalculateScore();
        }

        private void CalculateScore()
        {
            SubScores = new double[Scores.Length][];
            for (int i = 0; i < Scores.Length; i++)
            {
                double subscore = 0;
                for (int j = 0; j < Scores[i].Length; j++)
                {
                    subscore += Scores[i][j];
                }
                double subscoredd = subscore * Dives[i].DD;
                SubScores[i] = new Double[] { subscore, subscoredd };
                TotalScore += subscoredd;
            }
        }


        public string GenerateReport()
        {
            string template = File.ReadAllText("DiveScoreTemplate.tex");
            string filename = dirname + "/" + Name.Replace(" ", "") + "-" + Board;
            template = template.Replace("//Event//", EventName + " - " + Board);
            template = template.Replace("//Board//", Board);
            template = template.Replace("//Diver Name//", Name);
            template = template.Replace("//Placement//", "" + Place);
            template = template.Replace("//total score//", TotalScore + "");
            string table = "";
            for (int i = 0; i < Dives.Length; i++)
            {
                table += Dives[i].Code + "&" + Dives[i].Description + "&" + Scores[i][0] + "&" + Scores[i][1] + "&" + Scores[i][2] + "&" + SubScores[i][0] + "&" + SubScores[i][1] + @"\\\midrule" + "\n";
            }
            template = template.Replace("//tabledata//", table);
            File.WriteAllText(filename + ".tex", template);
            System.Diagnostics.Process.Start("CMD.exe", "/C pdflatex -output-directory=" + dirname + " " + filename + ".tex");
            Pdf = filename + ".pdf";
            return Pdf;
        }
    }
}
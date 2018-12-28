using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Core
{
    private bool canStart;
    private List<Event> runningEvents;
    public List<Dive> dives;
    public List<Event> events;
    public int curEvent;
    public int curDiver;
    public int totalDives;
    public int completedDives;
    public Core()
    {
        totalDives = 0;
        completedDives = 0;
        canStart = false;
        dives = new List<Dive>();
        events = new List<Event>();
        runningEvents = new List<Event>();
        // Create list of dives

        String[] divelistb = File.ReadAllText("./divelist.csv").Split('\n');
        for (int i = 1; i < divelistb.Length; i++) dives.Add(new Dive(divelistb[i]));
    }

    public void Start()
    {
        for(int i = 0; i < events.Count; i++)
        {
            for(int j = 0; j < events[i].divers.Count; j++)
            {
                totalDives += events[i].divers[j].Dives.Length;
            }
        }
        runningEvents.AddRange(events);
        canStart = true;
    }

    public void NextScore(double[] scores)
    {
        if(!canStart) return; // Don't start if start method not ran
        if(runningEvents[curEvent].AddScore(scores))
        {
            if (runningEvents[curEvent].IsDone())
            {
                runningEvents.RemoveAt(curEvent);
            }
            else
            {
                curEvent++;
            }
            curEvent = curEvent % events.Count;
        }
        completedDives++;
    }

    public void SetScore(int eventIndex, int diverIndex, int diveIndex, double[] scores)
    {
        events[eventIndex].SetScore(diverIndex, diveIndex, scores);
    }

    public void AddEvent(Event e)
    {
        events.Add(e);
    }

    public CoreState State()
    {
        return new CoreState(events[curEvent].curDiver, events[curEvent], completedDives, totalDives);
    }
    

    /// <summary>
    /// Search for the dive that has the given dive code and boardHeight
    /// </summary>
    /// <param name="diveCode">The code of the dive. Capitalization doesn't matter.</param>
    /// <param name="boardHeight">The height of the board in meters. eg: 1m</param>
    /// <returns>A Dive object that corresponds to the query or null if not found.</returns>
    public Dive GetDive(string diveCode, string boardHeight)
    {
        Dive d = dives.FirstOrDefault(x => x.Code == diveCode.ToUpper() && x.Board == boardHeight.ToUpper());
        if (d == null) d = new Dive();
        return d;
    }

    public void GenerateReports()
    {
        List<string> pdfs = new List<string>();
        for(int i = 0; i < events.Count; i++){
            pdfs.AddRange(events[i].GenerateReports());
        }

        Thread.Sleep(5000);
        string filename = "pdfs/" + "/CombinedReport";
        string template = File.ReadAllText("CombinedTemplate.tex");
        string includes = "";
        foreach (string d in pdfs)
        {
            includes += @"\includepdf{" + d + "}\n";
        }
        template = template.Replace("//Pdf includes//", includes);
        File.WriteAllText(filename + ".tex", template);
        System.Diagnostics.Process.Start("CMD.exe", "/C pdflatex -output-directory=" + "pdfs" + " " + filename + ".tex");
    }

    public void GenerateDiveList(){
        List<string> pdfs = new List<string>();
        for(int i = 0; i < events.Count; i++){
            pdfs.Add(events[i].GenerateDiveList());
        }
        Thread.Sleep(5000);
        string filename = "pdfs/" + "/CombinedDiveList";
        string template = File.ReadAllText("CombinedTemplate.tex");
        string includes = "";
        foreach (string d in pdfs)
        {
            includes += @"\includepdf{" + d + "}\n";
        }
        template = template.Replace("//Pdf includes//", includes);
        File.WriteAllText(filename + ".tex", template);
        System.Diagnostics.Process.Start("CMD.exe", "/C pdflatex -output-directory=" + "pdfs" + " " + filename + ".tex");
    }

    public void AutoSave()
    {
        string data = JsonConvert.SerializeObject(this);
        File.WriteAllText("LatestCore.json", data);
    }
}


public class CoreState
{
    public Diver curDiver { get; }
    public Event curEvent { get; }
    public int completedDives { get; }
    public int remainingDives { get; }
    public int totalDives { get; }
    public CoreState(Diver curDiver, Event curEvent, int completedDives, int totalDives)
    {
        this.curDiver = curDiver;
        this.curEvent = curEvent;
        this.completedDives = completedDives;
        this.totalDives = totalDives;
        this.remainingDives = totalDives - completedDives;
    }
}

public class Event
{
    public String name;
    public List<Diver> divers;
    private int nextDiver;
    public Diver curDiver;
    public string Board;
    private int completedDives;
    public int at;
    private readonly string dirname = "pdfs";
	public Event(String Name, String Board)
	{
	    this.completedDives = 0;
        this.Board = Board;
	    divers = new List<Diver>();
	    this.nextDiver = 0;
	    this.at = 0;
	    this.name = Name;
	}

    public void AddDiver(Diver d)
    {
        d.EventName = this.name;
        divers.Add(d);
        if(divers.Count == 1) curDiver = d;
    }

    public bool NextDiver()
    {
        curDiver = divers[nextDiver];
        at = nextDiver;
        nextDiver++;
        if (nextDiver == divers.Count)
        {
            nextDiver = 0;
            return true;
        }

        return false;
    }

    public bool AddScore(double[] scores)
    {
        divers[at].AddScore(scores);
        completedDives++;
        return NextDiver();
    }

    public void SetScore(int diverIndex, int diveIndex, double[] scores){
        divers[diverIndex].SetScore(diveIndex, scores);
    }

    public List<string> GenerateReports()
    {
        for(int i = 0; i < divers.Count; i++)
        {
            divers[i].CalculateScore();
        }
        string filename = dirname + "/" + name.Replace(" ", "");
            string template = File.ReadAllText("ResultsTemplate.tex");
            template = template.Replace("//Event//", name + " - " + Board);
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
            List<string> pdfs = new List<string>();
            pdfs.Add(name.Replace(" ", "") + ".pdf");
            foreach (Diver d in divers)
            {
                pdfs.Add(d.Pdf.Split('/')[1]);
            }
            return pdfs;
    }

    public string GenerateDiveList(){
        string template = File.ReadAllText("AnnouncementTemplate.tex");
                template = template.Replace("//Event//", name + " - " + Board);
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
                string filename = dirname + "/AnnouncersList-"+name.Replace(" ", "");
                File.WriteAllText(filename + ".tex", template);
                System.Diagnostics.Process.Start("CMD.exe", "/C pdflatex -output-directory=" + dirname + " " + filename + ".tex");
                return filename.Split('/')[1] +".pdf";
    }

    public bool IsDone()
    {
        int totalDives = divers[0].Dives.Length * divers.Count;
        return (totalDives - completedDives <= 0);
    }

    public override string ToString()
    {
        return name;
    }
}


public class Dive
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

public class Diver
{
    public string Name { get; set; }
    public string EventName { get; set; }
    public string Board { get; set; }
    public Dive[] Dives { get; set; }
    public List<List<double>> Scores { get; set; }
    public double[][] SubScores { get; set; }
    public double TotalScore { get; set; }
    public string Pdf { get; set; }
    public int Place { get; set; }
    public string dirname { get; set; }

    public Diver(string name, string board, Dive[] dives)
    {
        this.Scores = new List<List<double>>();
        this.dirname = "pdfs";
        this.Name = name;
        this.Board = board.ToUpper();
        this.Dives = dives;
        this.Pdf = "null";
        this.Place = 0;
    }

    public Diver(string name)
    {
        this.Name = name;
    }

    public void SetScore(int index, double[] judgeScores)
    {
        SetScore(index, new List<double>(judgeScores));
    }

    public void SetScore(int index, List<double> judgeScores)
    {
        Scores[index] = judgeScores;
    }

    public void AddScore(double[] judgeScores)
    {
        AddScore(new List<double>(judgeScores));
    }

    public void AddScore(List<double> judgeScores)
    {
        Scores.Add(judgeScores);
    }

    public void SetPlace(int place)
    {
        this.Place = place;
    }
    public void CalculateScore()
    {
        SubScores = new double[Scores.Count][];
        for (int i = 0; i < Scores.Count; i++)
        {
            double subscore = 0;
            for (int j = 0; j < Scores[i].Count; j++)
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
    public override string ToString()
    {
        return Name;
    }
}
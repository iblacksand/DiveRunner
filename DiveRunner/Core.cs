using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
public class Core
{
    List<Dive> dives;
    private List<Event> events;
    public Core()
    {
        // Create list of dives
        String[] divelistb = File.ReadAllText("divelist.csv").Split('\n');
        for (int i = 1; i < divelistb.Length; i++) dives.Add(new Dive(divelistb[i]));
    }

    public void Load(CoreSettings cset)
    {

    }

    /// <summary>
    /// Search for the dive that has the given dive code and boardHeight
    /// </summary>
    /// <param name="diveCode">The code of the dive. Capitalization doesn't matter.</param>
    /// <param name="boardHeight">The height of the board in meters. eg: 1m</param>
    /// <returns>A Dive object that corresponds to the query or null if not found.</returns>
    Dive GetDive(string diveCode, string boardHeight)
    {
        Dive d = dives.FirstOrDefault(x => x.Code == diveCode.ToUpper() && x.Board == boardHeight.ToUpper());
        if (d == null) d = new Dive();
        return d;
    }
}

public class CoreSettings
{
    public CoreSettings()
    {

    }
}

public class Toolbox
{
    
}

public class Event
{
	public Event()
	{
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
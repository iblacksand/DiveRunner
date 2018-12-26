using System;
using System.Collections.Generic;
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
    public Core()
    {

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
}

public class Toolbox
{
    
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

public class Event
{
	public Event()
	{
	}
}

using System.Collections.Generic;

public class BoundaryLine {
    public BoundaryPoint A;
    public BoundaryPoint B;

    public BoundaryLine( BoundaryPoint p1, BoundaryPoint p2 )
    {
        A = p1;
        B = p2;
    }

    public List<Faction> CommonFactions()
    {
        // The factions need to be the same
        List<Faction> commonFactions = new List<Faction>();
        foreach ( Faction f in A.FactionsClaiming )
        {
            if ( B.FactionsClaiming.Contains(f) ) commonFactions.Add(f);
        }

        return commonFactions;
    }
}
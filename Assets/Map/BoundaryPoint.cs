using UnityEngine;
using System.Collections.Generic;

public class BoundaryPoint {
    private List<StarSystem> _systemsClaimingPoint = new List<StarSystem>();
    public List<Faction> FactionsClaiming = new List<Faction>();
    public List<Faction> FactionsToDraw = new List<Faction>();
    public Vector2 Point;

    public BoundaryPoint( Vector2 point )
    {
        Point = point;
    }

    public void Claim( StarSystem claimer )
    {
        _systemsClaimingPoint.Add( claimer );
        if ( !FactionsClaiming.Contains(claimer.faction) )
        {
            FactionsClaiming.Add(claimer.faction);
        }
    }

    public void MarkDrawn( Faction f )
    {
        FactionsToDraw.Remove(f);
    }

    public void ResetDraw()
    {
        FactionsToDraw = new List<Faction>();
        
        foreach ( StarSystem tc in _systemsClaimingPoint )
        {
            if ( !FactionsToDraw.Contains(tc.faction) )
            {
                FactionsToDraw.Add(tc.faction);
            }
        }

        // No reason to draw if multiple fations aren't claiming it
        if ( FactionsToDraw.Count == 1 ) FactionsToDraw.Clear();
    }

}
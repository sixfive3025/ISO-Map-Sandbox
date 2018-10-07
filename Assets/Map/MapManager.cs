using System.Collections.Generic;
using UnityEngine;

// Generates the outlines for all the map blobs
public class MapManager {
    private Dictionary< PointPair,BoundaryLine > _boundaryLines = new Dictionary< PointPair,BoundaryLine>( new PointPairComparer() );
    private Dictionary<Vector2,BoundaryPoint> _boundaryPoints = new Dictionary<Vector2, BoundaryPoint>();
    private Dictionary<BoundaryPoint, List<BoundaryLine> > _lineReference = new Dictionary<BoundaryPoint, List<BoundaryLine> >();
    private List< List<BoundaryPoint> > _blobs = new List< List<BoundaryPoint> >();

    public void ClaimBoundary( StarSystem claimingTerritory, Vector2 b1, Vector2 b2 )
    {
        PointPair incomingPair = new PointPair();
        incomingPair.p1 = b1;
        incomingPair.p2 = b2;

        // Index each point so we only create them once
        if (!_boundaryPoints.ContainsKey(incomingPair.p1)) 
        _boundaryPoints[incomingPair.p1] = new BoundaryPoint(incomingPair.p1);
        
        if (!_boundaryPoints.ContainsKey(incomingPair.p2)) 
        _boundaryPoints[incomingPair.p2] = new BoundaryPoint(incomingPair.p2);

        // Index each border line so we only create it once
        if ( !_boundaryLines.ContainsKey(incomingPair) )
            _boundaryLines.Add( incomingPair, new BoundaryLine( _boundaryPoints[incomingPair.p1], _boundaryPoints[incomingPair.p2] ) );

        // Allow border lined to be looked up by the component points
        if (!_lineReference.ContainsKey(_boundaryPoints[incomingPair.p1])) 
            _lineReference[_boundaryPoints[incomingPair.p1]] = new List<BoundaryLine>();
        _lineReference[_boundaryPoints[incomingPair.p1]].Add(_boundaryLines[incomingPair]);

        if (!_lineReference.ContainsKey(_boundaryPoints[incomingPair.p2])) 
            _lineReference[_boundaryPoints[incomingPair.p2]] = new List<BoundaryLine>();
        _lineReference[_boundaryPoints[incomingPair.p2]].Add(_boundaryLines[incomingPair]);

        // System claims the boundary points
        _boundaryLines[incomingPair].A.Claim(claimingTerritory);
        _boundaryLines[incomingPair].B.Claim(claimingTerritory);
    }

    // MAP DRAWING PROCESS
    // List of BoundaryLines
    // Pick any BoundaryLine and ask it if it still needs to be included
    // BoundaryLine knows if needs inclusion by the number of factions claiming each of its points
    // If neither point claimed, skip it and tell it not to be included
    // Pick faction claimed and draw the whole border
    public void Draw()
    {
        foreach( BoundaryPoint resetPoint in _boundaryPoints.Values ) resetPoint.ResetDraw();

        bool stillDrawing = true;
        while (stillDrawing) // Run passes through the points till they've been drawn for all factions
        {
            stillDrawing = false;
            foreach( BoundaryPoint drawPoint in _boundaryPoints.Values )
            {
                // Keep going till we find a point where factions still need to be drawn
                if ( drawPoint.FactionsToDraw.Count == 0 ) continue;

                // Find any cross-faction border lines for the point
                List<BoundaryLine> linesWithPoint = _lineReference[drawPoint];
                Faction currentFaction = Faction.None;
                BoundaryLine firstNextLine = null;
                
                foreach ( BoundaryLine l in linesWithPoint )
                {
                    if ( firstNextLine != null ) break;

                    List<Faction> commonFactions = l.CommonFactions();
                    if ( commonFactions.Count < 2 ) continue; // Needs to be a border line

                    // Make sure that faction that still needs to be drawn is part of the border
                    foreach ( Faction f in drawPoint.FactionsToDraw )
                    {
                        if ( commonFactions.Contains(f) )
                        {
                            currentFaction = f;
                            firstNextLine = l;
                            break;
                        }
                    }
                }
                
                stillDrawing = true;
                // We got a live one! Time to start a new blob.
                List<BoundaryPoint> blobPoints = new List<BoundaryPoint>();
                
                BoundaryPoint currentPoint = drawPoint;
                BoundaryPoint nextPoint = null;
                if ( drawPoint == firstNextLine.A ) nextPoint = firstNextLine.B;
                    else nextPoint = firstNextLine.A;
                BoundaryLine lastLine = firstNextLine;
                
                // Add till we get back to original point
                while ( !blobPoints.Contains(currentPoint) )
                {
                    blobPoints.Add(currentPoint);
                    currentPoint = nextPoint;
                    nextPoint = null;
                    
                    // Find the next point
                    linesWithPoint = _lineReference[currentPoint];
                    linesWithPoint.Remove(lastLine); // Don't go backward!

                    BoundaryLine nextLine = null;
                    foreach ( BoundaryLine l in linesWithPoint )
                    {
                        if ( nextLine != null ) break;

                        List<Faction> commonFactions = l.CommonFactions();
                        if ( commonFactions.Count > 1 && commonFactions.Contains(currentFaction) ) nextLine = l;
                    }

                    if ( currentPoint == nextLine.A ) nextPoint = nextLine.B;
                    else nextPoint = nextLine.A;
                    lastLine = nextLine;
                }

                foreach( BoundaryPoint p in blobPoints )
                    p.MarkDrawn(currentFaction);

                _blobs.Add( blobPoints );

                for( int i = 0; i < blobPoints.Count; i++ )
                {
                    int nextI = i+1;
                    if (nextI == blobPoints.Count) nextI = 0;
                    GameObject go = new GameObject("Line");
                    GizmoLine lineToDraw = go.AddComponent<GizmoLine>();
                    lineToDraw.A = blobPoints[i].Point;
                    lineToDraw.B = blobPoints[nextI].Point;
                    lineToDraw.drawMe = true;
                }
                Debug.Log("BLOB DONE: " + blobPoints.Count);
                return;
            }
        }
    }

    private class PointPair
    {
        public Vector2 p1;
        public Vector2 p2;
    }

    private class PointPairComparer : IEqualityComparer<PointPair>
    {
        public bool Equals ( PointPair pair1, PointPair pair2 )
        {
            return ((pair1.p1 == pair2.p1) && (pair1.p2 == pair2.p2)) ||
                   ((pair1.p1 == pair2.p2) && (pair1.p2 == pair2.p1));
        }
        
        public int GetHashCode( PointPair pair )
        {
            unchecked
            {
                string hashstring = "";

                // Generate the same hash code even if the points are in a different order
                if ( pair.p1.x > pair.p2.x )
                {
                    hashstring = pair.p1.x + "," + pair.p1.y + "-" + pair.p2.x + "," + pair.p2.y;
                }
                else if ( pair.p1.x < pair.p2.x )
                {
                    hashstring = pair.p2.x + "," + pair.p2.y + "-" + pair.p1.x + "," + pair.p1.y;
                }
                else
                {
                    if ( pair.p1.y >= pair.p2.y )
                    {
                        hashstring = pair.p1.x + "," + pair.p1.y + "-" + pair.p2.x + "," + pair.p2.y;
                    }
                    else if ( pair.p1.y < pair.p2.y )
                    {
                        hashstring = pair.p2.x + "," + pair.p2.y + "-" + pair.p1.x + "," + pair.p1.y;
                    }
                }

                return hashstring.GetHashCode();
            }
        }
    }
}
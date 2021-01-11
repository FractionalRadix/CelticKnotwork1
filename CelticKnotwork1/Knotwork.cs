using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CelticKnotwork1
{
    class Knotwork
    {
        private class GridConnection
        {
            public GridCoordinates Source { set; get; }
            public LineSegment Connector { set; get; }

            public GridCoordinates Target( )
            {
                return Connector.Target(Source);
            }
        }
        private List<GridConnection> Connections = new List<GridConnection>();


        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public Knotwork(int rows, int cols)
        {
            this.Rows = rows;
            this.Cols = cols;
        }

        public void AddLine(int row, int col, LineSegment line)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Cols)
            {
                return; //TODO?+ Issue a warning?
            }

            GridCoordinates source = new GridCoordinates { Row = row, Col = col };
            GridConnection newConnection = new GridConnection { Source = source, Connector = line };
            Connections.Add(newConnection);
        }

        public IEnumerable<Tuple<GridCoordinates,LineSegment>> GetAllLines()
        {
            return Connections.Select(x => new Tuple<GridCoordinates,LineSegment>(x.Source, x.Connector));
        }

        public IEnumerable<GridCoordinates> GetConnectionsFor(GridCoordinates coor)
        {
            var outgoingConnections = Connections
                .FindAll(x => x.Source.Equals(coor))
                .Select(x => x.Target())
                ;
            var incomingConnections = Connections
                .FindAll(x => x.Target().Equals(coor))
                .Select(x => x.Source)
                ;

            var res = outgoingConnections.Concat(incomingConnections);

            return res;
        }

        /// <summary>
        /// Find the line segment that connects points p1 and p2.
        /// Note that two points should never be connected by more than one line segment.
        /// </summary>
        /// <param name="p1">First point on the grid.</param>
        /// <param name="p2">Second point on the grid.</param>
        /// <returns>The connector that connects p1 and p2, if it exists; <code>null</code> otherwise.</returns>
        public LineSegment GetLine(GridCoordinates p1, GridCoordinates p2)
        {
            GridConnection conn = Connections.Find(x => x.Source.Equals(p1) && x.Target().Equals(p2));
            if (conn != null)
            {
                return conn.Connector;
            }

            conn = Connections.Find(x => x.Source.Equals(p2) && x.Target().Equals(p1));
            if (conn != null)
            {
                return conn.Connector;
            }

            return null;
        }
    }
}

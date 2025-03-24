using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMeansClusteringTest
{
    internal class ClusterPoint
    {
        public Point Point { get; set; }   // Der Punkt (Koordinaten)
        public int ClusterID { get; set; } // Die ID des Clusters, zu dem der Punkt gehört
        public int x { get; set; }
        public int y { get; set; }

        // Konstruktor
        public ClusterPoint(Point p, int clusterID)
        {
            this.Point = p;
            this.ClusterID = clusterID;
        }

        public ClusterPoint setCluster(Point p, int clusterID)
        {
            return new ClusterPoint(p, clusterID);
        }
    }
}

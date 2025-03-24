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
        public int ClusterID { get; set; } 
        public float x { get; set; }
        public float y { get; set; }

        // Konstruktor
        public ClusterPoint(float x, float y, int clusterID)
        {
            this.x = x;
            this.y = y;
            this.ClusterID = clusterID;
        }

        public ClusterPoint setCluster(int clusterID)
        {
            return new ClusterPoint(x,y, clusterID);
        }
    }
}

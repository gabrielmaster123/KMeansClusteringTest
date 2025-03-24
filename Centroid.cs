using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMeansClusteringTest
{
    internal class Centroid
    {
        public ClusterPoint p { get; set; }   // Der Punkt (Koordinaten)
        public int ClusterID { get; set; } // Die ID des Clusters, zu dem der Cluster gehört
        public Centroid(ClusterPoint p, int clusterID)
        {
            this.p = p;
            this.ClusterID = clusterID;
        }

        public void recalculate(ClusterPoint[] dataset)
        {
            int x = 0;
            int y = 0;
            int i = 0;
            foreach (var data in dataset)
            {
                if(data.ClusterID == ClusterID)
                {
                    x += data.x;
                    y += data.y;
                    i++;
                }
            }
            p = new ClusterPoint(new Point(x / i, y / i), ClusterID);
        }
    }
    
    }

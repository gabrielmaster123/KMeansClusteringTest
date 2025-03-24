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
        public ClusterPoint p;
        public int ClusterID;

        public Centroid(ClusterPoint point, int clusterID)
        {
            p = point;
            ClusterID = clusterID;
        }

        public void recalculate(List<ClusterPoint> dataset)
        {
            var pointsInCluster = dataset.Where(data => data.ClusterID == ClusterID).ToList();

            if (pointsInCluster.Count > 0)
            {
                float meanX = pointsInCluster.Average(p => p.x);
                float meanY = pointsInCluster.Average(p => p.y);

                p.x = meanX;
                p.y = meanY;
            }
        }
    }
    
    }

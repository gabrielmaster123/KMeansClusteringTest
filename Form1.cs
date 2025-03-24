using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMeansClusteringTest
{
    public partial class Form1 : Form
    {

        private ClusterPoint[] ClusterPoints = new ClusterPoint[100];
        private Centroid[] Centroids;
        Random rand = new Random();
        public Form1()
        {
            InitializeComponent();

            // Initialisiere das Array mit zufälligen Punkten

            for (int i = 0; i < ClusterPoints.Length; i++)
            {
                // Zufällige X- und Y-Koordinaten im Bereich der Formgröße
                ClusterPoints[i] = new ClusterPoint(new Point(rand.Next(0, this.Width), rand.Next(0, this.Height)),0);
            }

            // Sicherstellen, dass das Paint-Event abgerufen wird
            this.Paint += new PaintEventHandler(Form1_Paint);
        }

        // Paint-Event zum Zeichnen der Punkte
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Erhalte das Graphics-Objekt zum Zeichnen auf der Form
            Graphics g = e.Graphics;
            
            // Bürste für die Punkte (wir wählen Blau)

            ClusterPoints = KMeans(ClusterPoints, 3);
            // Zeichne jeden Punkt als kleinen Kreis (Ellipse)
            foreach (var data in ClusterPoints)
            {
                Brush PointBrush = Brushes.Black;
                switch (data.ClusterID) {
                    case 1:
                        PointBrush = Brushes.Red;
                        break;
                    case 2:
                        PointBrush = Brushes.Green;
                        break;
                    case 3:
                        PointBrush = Brushes.Blue;
                        break;
                    case 0:
                    default:
                        PointBrush = Brushes.Black;
                        break;
                }
                
                // Zeichne einen Punkt als kleinen Kreis (10x10 Pixel)
                g.FillEllipse(PointBrush, data.x - 5, data.y - 5, 10, 10);
            }
        }

        private ClusterPoint[] KMeans(ClusterPoint[] dataset, int k)
        {
            Centroids = new Centroid[k];
            for (int i = 1; i < k; i++) 
            { 
                int p = rand.Next(0, dataset.Length);
                while(dataset[p].ClusterID != 0)
                {
                    p = rand.Next(0, dataset.Length);
                }
                dataset[p].ClusterID = i;
                Centroids[i] = new Centroid(dataset[i],i);
            }
            foreach (var c in Centroids)
            {
                c.recalculate(dataset);
            }
            while (!CheckDone(dataset))
            {
                foreach (var data in dataset)
                {
                    if (data.ClusterID == 0)
                    {
                        Centroid near = Centroids[1];
                        for (int i = 0; i < Centroids.Length; i++)
                        {
                            var data2 = Centroids[i];

                            if (getDistance(data, data2.p) < getDistance(data, Centroids[i].p))
                            {
                                near = data2;
                            }
                        }
                        data.ClusterID = near.ClusterID;
                    }
                }
            }

            
            return dataset;
        }

        private bool CheckDone(ClusterPoint[] dataset)
        {
            foreach (var data in dataset)
            {
                if (data.ClusterID == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private int getDistance(ClusterPoint a, ClusterPoint b)
        { 
            int dx = b.Point.X - a.Point.X;
            int dy = b.Point.Y - a.Point.Y;
            return (int)Math.Sqrt(dx * dx + dy * dy);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

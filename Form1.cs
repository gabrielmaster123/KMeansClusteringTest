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
using System.IO;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace KMeansClusteringTest
{
    public partial class Form1 : Form
    {
        private int k = 3;
        private List<ClusterPoint> ClusterPoints;
        private List<Centroid> Centroids;
        Random rand = new Random();

        string filePath = "data.csv";
        

        public Form1()
        {
            InitializeComponent();
            ClusterPoints = new List<ClusterPoint>(); // Initialize the list
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            }))
            {
                var culture = new CultureInfo("en-US");
                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    // Use the specified culture to parse the values correctly
                    float x = float.Parse(record.X, culture); 
                    float y = float.Parse(record.Y, culture); 
                    ClusterPoints.Add(new ClusterPoint(x, y, 0));
                }
            }
            
            this.Paint += new PaintEventHandler(Form1_Paint);
            Task.Run(() => RunKMeans());

        }

        private async Task RunKMeans()
        {
            foreach (var data in ClusterPoints)
            {
                data.ClusterID = 0;
            }
            ClusterPoints = await Task.Run(() => KMeans(ClusterPoints, k));
            this.Invalidate();
        }

        // Paint-Event zum Zeichnen der Punkte
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (ClusterPoints.Count == 0)
                return;

            float minX = ClusterPoints.Min(p => p.x);
            float maxX = ClusterPoints.Max(p => p.x);
            float minY = ClusterPoints.Min(p => p.y);
            float maxY = ClusterPoints.Max(p => p.y);

            float rangeX = maxX - minX;
            float rangeY = maxY - minY;
            if (rangeX == 0) rangeX = 1;
            if (rangeY == 0) rangeY = 1;

            float scaleX = (ClientSize.Width - 20) / rangeX;
            float scaleY = (ClientSize.Height - 20) / rangeY;
            Console.WriteLine($"minX: {minX}, maxX: {maxX}, rangeX: {rangeX}, scaleX: {scaleX}");
            Console.WriteLine($"minY: {minY}, maxY: {maxY}, rangeY: {rangeY}, scaleY: {scaleY}");

            foreach (var data in ClusterPoints)
            {
                Brush PointBrush = Brushes.Black;
                switch (data.ClusterID)
                {
                    case 1:
                        PointBrush = Brushes.Red;
                        break;
                    case 2:
                        PointBrush = Brushes.Green;
                        break;
                    case 3:
                        PointBrush = Brushes.Blue;
                        break;
                    case 4:
                        PointBrush = Brushes.Yellow;
                        break;
                    case 5:
                        PointBrush = Brushes.Purple;
                        break;
                    case 6:
                        PointBrush = Brushes.Orange;
                        break;
                    case 7:
                        PointBrush = Brushes.Cyan;
                        break;
                    case 8:
                        PointBrush = Brushes.Magenta;
                        break;
                    case 9:
                        PointBrush = Brushes.Brown;
                        break;
                    case 10:
                        PointBrush = Brushes.Pink;
                        break;
                    case 11:
                        PointBrush = Brushes.LightBlue;
                        break;
                    case 12:
                        PointBrush = Brushes.LightGreen;
                        break;
                    case 13:
                        PointBrush = Brushes.LightYellow;
                        break;
                    case 0:
                    default:
                        PointBrush = Brushes.Gray; // Clusterless points as gray
                        break;
                }

                float normalizedX = (data.x - minX) * scaleX + 10;
                float normalizedY = (data.y - minY) * scaleY + 10;

                g.FillEllipse(PointBrush, normalizedX - 5, normalizedY - 5, 10, 10);
            }
        }

        private List<ClusterPoint> KMeans(List<ClusterPoint> dataset, int k)
        {
            Console.WriteLine("KMeans started");
            Centroids = new List<Centroid>();
            for (int i = 0; i < k; i++)
            {
                int p = rand.Next(0, dataset.Count);
                while (dataset[p].ClusterID != 0)
                {
                    p = rand.Next(0, dataset.Count);
                }
                dataset[p].ClusterID = i + 1;
                Centroids.Add(new Centroid(dataset[p], i + 1));
            }

            bool centroidsChanged = true;
            while (centroidsChanged)
            {
                centroidsChanged = false;

                foreach (var data in dataset)
                {
                    int nearestCentroidIndex = -1;
                    float minDistance = float.MaxValue;

                    for (int i = 0; i < Centroids.Count; i++)
                    {
                        var dist = getDistance(data, Centroids[i].p);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            nearestCentroidIndex = i;
                        }
                    }

                    if (data.ClusterID != Centroids[nearestCentroidIndex].ClusterID)
                    {
                        data.ClusterID = Centroids[nearestCentroidIndex].ClusterID;
                        centroidsChanged = true;
                    }
                }

                foreach (var centroid in Centroids)
                {
                    centroid.recalculate(dataset);
                }
            }
            Console.WriteLine("KMeans finished");
            return dataset;
        }

        private bool CheckDone(List<ClusterPoint> dataset)
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

       private float getDistance(ClusterPoint a, ClusterPoint b) 
{ 
    float dx = b.x - a.x;
    float dy = b.y - a.y;
    return (float)Math.Sqrt(dx * dx + dy * dy); // Use float instead of int
}

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e) { 
        
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int newK) && newK > 0)
            {
                k = newK;
                await RunKMeans();
                this.BeginInvoke(new Action(() => this.Invalidate()));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }



}


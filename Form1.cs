using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;

namespace KMeansClusteringTest
{
    public partial class Form1 : Form
    {
        private int k = 3;
        private List<ClusterPoint> ClusterPoints;
        private List<Centroid> Centroids;
        private Random rand = new Random();

        string filePath = "data.csv";

        public Form1()
        {
            InitializeComponent();
            ClusterPoints = new List<ClusterPoint>();
            LoadData();
            this.Paint += new PaintEventHandler(Form1_Paint);
        }

        private void LoadData()
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true
            }))
            {
                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    // Adjust these values based on your CSV format
                    float x = float.Parse(record.X, CultureInfo.InvariantCulture);
                    float y = float.Parse(record.Y, CultureInfo.InvariantCulture);
                    ClusterPoints.Add(new ClusterPoint(x, y, 0));
                }
            }
        }

        private async Task RunKMeans()
        {
            ClusterPoints = await Task.Run(() => KMeans(ClusterPoints, k));
            this.Invalidate();
        }

        // Paint-Event zum Zeichnen der Punkte
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (ClusterPoints.Count == 0)
                return;

            // Calculate bounds to scale points accordingly
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

            foreach (var data in ClusterPoints)
            {
                Brush PointBrush = GetClusterColor(data.ClusterID);

                float normalizedX = (data.x - minX) * scaleX + 10;
                float normalizedY = (data.y - minY) * scaleY + 10;

                g.FillEllipse(PointBrush, normalizedX - 5, normalizedY - 5, 10, 10);
            }
        }

        private Brush GetClusterColor(int clusterId)
        {
            // Assign different colors to different clusters
            switch (clusterId)
            {
                case 1:
                    return Brushes.Red;
                case 2:
                    return Brushes.Green;
                case 3:
                    return Brushes.Blue;
                case 4:
                    return Brushes.Yellow;
                case 5:
                    return Brushes.Purple;
                case 6:
                    return Brushes.Orange;
                case 7:
                    return Brushes.Cyan;
                case 8:
                    return Brushes.Magenta;
                case 9:
                    return Brushes.Brown;
                case 10:
                    return Brushes.Pink;
                default:
                    return GetRandomColor(clusterId); // Default to a random color for higher cluster IDs
            }
        }

        private Brush GetRandomColor(int clusterId)
        {
            Random random = new Random(clusterId);
            return new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
        }

        private List<ClusterPoint> KMeans(List<ClusterPoint> dataset, int k)
        {
            foreach (var data in ClusterPoints)
            {
                data.ClusterID = 0;
            }
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

                // Recalculate centroids
                foreach (var centroid in Centroids)
                {
                    centroid.recalculate(dataset);
                }
            }
            Console.WriteLine("KMeans finished");
            return dataset;
        }

        private float getDistance(ClusterPoint a, ClusterPoint b)
        {
            float dx = b.x - a.x;
            float dy = b.y - a.y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        private async void buttonRunKMeans_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int newK) && newK > 0)
            {
                k = newK; // Get k value from TextBox
                await RunKMeans();
            }
        }

        private async Task buttonRunElbowMethod_Click(object sender, EventArgs e)
        {
            int maxK = 10;
            int optimalK = await Task.Run(() => FindOptimalK(ClusterPoints, maxK));
            textBox1.Text = optimalK.ToString();
        }

        private int FindOptimalK(List<ClusterPoint> dataPoints, int maxK)
        {
            List<double> distortions = new List<double>();

            for (int k = 1; k <= maxK; k++)
            {
                var clusters = KMeans(dataPoints, k);
                distortions.Add(CalculateDistortion(dataPoints, clusters));
            }

            double minDistortion = distortions[0];
            int optimalK = 1;

            for (int i = 1; i < distortions.Count - 1; i++)
            {
                double slope1 = distortions[i] - distortions[i - 1];
                double slope2 = distortions[i + 1] - distortions[i];
                if (slope1 > 0 && slope2 < 0)
                {
                    optimalK = i + 1;
                    break;
                }
            }

            return optimalK;
        }

        private double CalculateDistortion(List<ClusterPoint> dataPoints, List<ClusterPoint> clusters)
        {
            double distortion = 0;
            foreach (var point in dataPoints)
            {
                distortion += getDistance(point, clusters[point.ClusterID - 1]);
            }
            return distortion;
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

        private async void button2_Click(object sender, EventArgs e)
        {
            await buttonRunElbowMethod_Click(sender, e);
            k = int.Parse(textBox1.Text);
            await RunKMeans();
            this.BeginInvoke(new Action(() => this.Invalidate()));
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

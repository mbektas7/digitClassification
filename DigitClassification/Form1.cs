using DigitClassification.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigitClassification
{
    public partial class Form1 : Form
    {

        private string[] _files;
        private string[] _testfiles;
        private List<double[]> trainingSet;
        List<Centroid> centroidList;
        double[][] covarianceMatris = null;
        int k;
        KMeans km;
        public Form1()
        {
            trainingSet = new List<double[]>();
            centroidList = new List<Centroid>();
            InitializeComponent();
            k = 10;
            sinifSayisi.Text = k.ToString();
            txtHeight.Text = "28";
            txtwidth.Text = "28";
          

        }


        //eğitim seti yükle
        private void button1_Click(object sender, EventArgs e)
        {

            if (!String.IsNullOrEmpty(txtHeight.Text) && !String.IsNullOrEmpty(txtwidth.Text))
            {
                int height = Convert.ToInt32(txtHeight.Text);
                int width = Convert.ToInt32(txtwidth.Text);
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.SelectedPath = @"C:\Users\Mirac\source\repos\DigitClassification\DigitClassification\bin\Debug\testSet100";

                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        _files = Directory.GetFiles(fbd.SelectedPath);
                        BackgroundWorker objBackgroundWorker = new BackgroundWorker();
                        objBackgroundWorker.WorkerReportsProgress = true;
                        objBackgroundWorker.DoWork += (o, ea) =>
                        {
                            int i = 1;
                            foreach (string file in _files)
                            {
                                Bitmap bmpImage = ResizeImage(file, new Size(width, height));
                                double[] processResult = grayLevelSelect.Checked ? GetGrayPixels(bmpImage) : GetAverageRGB(bmpImage);
                                trainingSet.Add(processResult);
                                double percent = (100.0 * i) / _files.Length;
                                ((BackgroundWorker)o).ReportProgress((int)percent);
                                i++;
                            }
                        };
                        objBackgroundWorker.ProgressChanged += (o, ea) => { progressBar1.Value = ea.ProgressPercentage; };
                        objBackgroundWorker.RunWorkerAsync();
                    }
                }
            }
            else
            {
                MessageBox.Show("Görüntü boyutlarını giriniz.");
            }

        }


        //eğitime başla
        private void button2_Click(object sender, EventArgs e)
        {


            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += (o, eventArgs) =>
            {

                k = Convert.ToInt32(sinifSayisi.Text);
                if (trainingSet.Count > 0)
                {

                    if (centroidList.Count<1)
                    {
                        // Başlangıçta verilen K küme sayısına göre rastgele kümeler oluşturuluyor.
                        for (int i = 0; i < k; i++)
                        {
                            Centroid centroid = new Centroid(trainingSet.ToArray(), Misc.centroidColors[i]);
                            centroidList.Add(centroid);
                        }
                    }
                   



                    double[][] dataSet = trainingSet.ToArray();


                    while (true)
                    {
                        foreach (Centroid centroid in centroidList)
                            centroid.Reset();

                        for (int i = 0; i < dataSet.GetLength(0); i++)
                        {
                            double[] point = dataSet[i];
                            int closestIndex = -1;
                            double minDistance = Double.MaxValue;
                            for (int k = 0; k < centroidList.Count; k++)
                            {
                                double distance = calcDistance(centroidList[k].Array, point);

                                if (distance < minDistance)
                                {
                                    closestIndex = k;
                                    minDistance = distance;
                                }

                            }
                            centroidList[closestIndex].addPoint(point);
                            double progressPercent = (100 * (i + 1)) / (int)dataSet.GetLength(0);
                            ((BackgroundWorker)o).ReportProgress((int)progressPercent);
                        }

                        foreach (Centroid centroid in centroidList)
                            centroid.MoveCentroid();


                        bool hasChanged = false;
                        foreach (Centroid centroid in centroidList)
                        {
                            if (centroid.HasChanged())
                            {
                                hasChanged = true;
                                break;

                            }
                        }

                        if (!hasChanged)
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Eğitim Kümesi yükleyiniz!");
                }

            };
            backgroundWorker.ProgressChanged += (o, eventArgsProgressChanged) =>
            {

                progressBar1.Value = eventArgsProgressChanged.ProgressPercentage;
            };
            backgroundWorker.RunWorkerAsync();


        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (trainingSet.Count < 1)
            {
                MessageBox.Show("Eğitim Kümesi yükleyiniz!");
            }
            else
            {
                listBox0.Items.Clear();
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                listBox4.Items.Clear();
                listBox5.Items.Clear();
                listBox6.Items.Clear();
                listBox7.Items.Clear();
                listBox8.Items.Clear();
                listBox9.Items.Clear();


                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.SelectedPath = @"C:\Users\Mirac\source\repos\DigitClassification\DigitClassification\bin\Debug\testSet100";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        _testfiles = Directory.GetFiles(fbd.SelectedPath);


                        foreach (string file in _testfiles)
                        {

                            Bitmap bmpImage = ResizeImage(file, new Size(Convert.ToInt32(txtwidth.Text), Convert.ToInt32(txtHeight.Text)));

                            double[] processResult = grayLevelSelect.Checked ? GetGrayPixels(bmpImage) : GetAverageRGB(bmpImage);

                            int closestIndex = -1;
                            double minDistance = Double.MaxValue;
                            for (int k = 0; k < centroidList.Count; k++)
                            {
                                double distance = calcDistance(centroidList[k].Array, processResult);
                                if (distance < minDistance)
                                {
                                    closestIndex = k;
                                    minDistance = distance;
                                }
                            }

                            

                            if (closestIndex == 0)
                                listBox0.Items.Add(file);
                            else if (closestIndex == 1)
                                listBox1.Items.Add(file);
                            else if (closestIndex == 2)
                                listBox2.Items.Add(file);
                            else if (closestIndex == 3)
                                listBox3.Items.Add(file);
                            else if (closestIndex == 4)
                                listBox4.Items.Add(file);
                            else if (closestIndex == 5)
                                listBox5.Items.Add(file);
                            else if (closestIndex == 6)
                                listBox6.Items.Add(file);
                            else if (closestIndex == 7)
                                listBox7.Items.Add(file);
                            else if (closestIndex == 8)
                                listBox8.Items.Add(file);
                            else if (closestIndex == 9)
                                listBox9.Items.Add(file);
          

                        }


                    }
                }
            }
        }

        private double calcDistance(double[] array1, double[] array2)
        {
            double res = 0;
            for (int i = 0; i < array1.Length; i++)
            {
                res += Math.Pow(array1[i] - array2[i], 2);
            }
            return Math.Sqrt(res);
        }

        Bitmap ResizeImage(string file, Size size)
        {
            return new Bitmap(Image.FromFile(file), size);
        }
        double[] GetAverageRGB(Bitmap bmpImage)
        {
            double[] result = new double[3];
            int numberOfPixlels = 0;

            for (int i = 0; i < bmpImage.Width; i++)
            {
                for (int j = 0; j < bmpImage.Height; j++)
                {
                    Color c = bmpImage.GetPixel(i, j);
                    result[0] += c.R;
                    result[1] += c.G;
                    result[2] += c.B;
                    numberOfPixlels++;
                }
            }

            bmpImage.Dispose();

            result[0] /= numberOfPixlels;
            result[1] /= numberOfPixlels;
            result[2] /= numberOfPixlels;
            return result;
        }
        double[] GetGrayPixels(Bitmap bmpImage)
        {
            double[] result = new double[bmpImage.Width * bmpImage.Height];
            int numberOfPixlels = 0;

            for (int i = 0; i < bmpImage.Width; i++)
            {
                for (int j = 0; j < bmpImage.Height; j++)
                {
                    Color oc = bmpImage.GetPixel(i, j);
                    int grayScale = (int)((oc.R * 0.2125) + (oc.G * 0.7154) + (oc.B * 0.0721));
                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);

                    result[numberOfPixlels] = nc.R;
                    numberOfPixlels++;
                }
            }

            bmpImage.Dispose();
            return result;
        }
      


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(listBox1.Items[listBox1.SelectedIndex].ToString());

        }

        private void listBox0_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox0.Image = Image.FromFile(listBox0.Items[listBox0.SelectedIndex].ToString());
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox2.Image = Image.FromFile(listBox2.Items[listBox2.SelectedIndex].ToString());
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox3.Image = Image.FromFile(listBox3.Items[listBox3.SelectedIndex].ToString());
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox4.Image = Image.FromFile(listBox4.Items[listBox4.SelectedIndex].ToString());
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox5.Image = Image.FromFile(listBox5.Items[listBox5.SelectedIndex].ToString());
        }

        private void listBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox6.Image = Image.FromFile(listBox6.Items[listBox6.SelectedIndex].ToString());
        }

        private void listBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox7.Image = Image.FromFile(listBox7.Items[listBox7.SelectedIndex].ToString());
        }

        private void listBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox8.Image = Image.FromFile(listBox8.Items[listBox8.SelectedIndex].ToString());
        }

        private void listBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox9.Image = Image.FromFile(listBox9.Items[listBox9.SelectedIndex].ToString());
        }

        private void rgbLevelSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (rgbLevelSelect.Checked && grayLevelSelect.Checked)
            {
                grayLevelSelect.Checked = false;
            }
            else if (!rgbLevelSelect.Checked && !grayLevelSelect.Checked)
            {
                grayLevelSelect.Checked = true;
            }

        }

        private void grayLevelSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (rgbLevelSelect.Checked && grayLevelSelect.Checked)
            {
                rgbLevelSelect.Checked = false;
            }
            else if (!rgbLevelSelect.Checked && !grayLevelSelect.Checked)
            {
                rgbLevelSelect.Checked = true;
            }
        }

    }
}

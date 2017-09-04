using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mstum.utils;


namespace RailgunRenderer
{
    public partial class ImageExport : Form
    {

        private Bitmap image;
        private string outputLoc;
        private FileStream inputRIF;
        private int outPos = 0;
        private int multiplier;
        private bool isOddRow = false;
        public ImageExport()
        {
            InitializeComponent();
        }

        private void ImageExport_Load(object sender, EventArgs e)
        {

        }

        public void run(int m)
        {
            Debug.WriteLine("MM:" + m);
            multiplier = m;
            prepConvert();
            

        }



        private void prepConvert()
        {
            OpenFileDialog inputFileDialog = new OpenFileDialog();
            string location = "null";
            inputFileDialog.Title = "Select an RIF file";
            inputFileDialog.Filter = "RIF Image(*.RIF)|*.RIF";
            if (inputFileDialog.ShowDialog() == DialogResult.OK)
            {
                location = inputFileDialog.FileName;
            }
            else
            {
                MessageBox.Show("RIF file not selected! Aborting!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;

            }
            inputRIF = File.OpenRead(location);
            //image = new Bitmap(location);
            SaveFileDialog outputFileDialog = new SaveFileDialog();
            outputFileDialog.Title = "Choose where to save converted file";
            outputFileDialog.Filter = "Image Files(*.BMP)|*.BMP";
            outputFileDialog.AddExtension = true;
            if (outputFileDialog.ShowDialog() == DialogResult.OK)
            {
                outputLoc = outputFileDialog.FileName;
            }
            else
            {
                MessageBox.Show("Output location not selected! Aborting!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            
            convert();
        }

        private void convert()
        {
            
            ArrayList imageData = new ArrayList();
            int h = (int)Base36.Decode(readHelper());

            int v = (int)Base36.Decode(readHelper());
            progressBar1.Maximum = (h * v);
            h *= multiplier;
            v *= multiplier;
            progressBar1.Step = 1;
            Debug.WriteLine("X:" + h + " Y:" + v + " M:" + multiplier);
            //outputRIF.AutoFlush = true;
            image = new Bitmap(h,v);
            int i;
            for (i = 0; i < v/multiplier; i++)
            {
                for (int j = 0; j < h/multiplier; j++)
                {
                    String nextVal = readHelper();
                    Color pixel;
                    if (nextVal.Contains('&'))
                    {
                        int idx = (int)Base36.Decode(nextVal.Substring(1));
                        pixel = Color.FromArgb((int) imageData[idx]);
                        
                    }
                    else
                    {
                        int argb;
                        if (nextVal.Contains('+'))
                        {
                            argb = (int)Base36.Decode(nextVal.Substring(1));
                            pixel = Color.FromArgb(argb);

                        }
                        argb = -(int)Base36.Decode(nextVal);
                        imageData.Add(argb);
                        pixel = Color.FromArgb(argb);
                    }
                    Graphics drawer = Graphics.FromImage(image);
                    Pen color = new Pen(pixel);
                    if (!isOddRow)
                    {
                        drawer.DrawRectangle(color, j, i + (multiplier - 1), multiplier, multiplier);
                        isOddRow = true;
                    }
                    else
                    {
                        drawer.DrawRectangle(color, j + (multiplier - 1), i + (multiplier - 1), multiplier, multiplier);
                        isOddRow = false;
                    }
                    
                    progressBar1.PerformStep();
                }
                    
                
            }
            image.Save(outputLoc);
            MessageBox.Show("Image sucessfully created.");
            this.Close();
        }

        /*
         * Helper Methods
         */
        private string readHelper()
        {
            bool foundSeperator = false;
            ArrayList data = new ArrayList();
            while (!foundSeperator)
            {
                byte[] buffer = new byte[1];
                inputRIF.Read(buffer, 0, 1);
                string raw = new ASCIIEncoding().GetString(buffer);
                if (!raw.Equals("|"))
                {
                    data.Add(raw);
                }
                else
                {
                    foundSeperator = true;
                }
            }
            string returnData = null;
            foreach (string v in data)
            {
                returnData += v;
            }
            return returnData;
        }

        private string toHex(int num)
        {
            return num.ToString("X");
        }
        //To go back to int
        //return Convert.ToInt32(hex, 16);
    }
}

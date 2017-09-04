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


namespace RIFTool
{
    public partial class Converter : Form
    {
        
        private Bitmap image;
        private FileStream outputRIF;
        private int outPos = 0;
        public Converter()
        {
            InitializeComponent();
        }

        private void Converter_Load(object sender, EventArgs e)
        {
            
        }

        public void run()
        {

            prepConvert();
            
            
        }



        private void prepConvert()
        {
            OpenFileDialog inputFileDialog = new OpenFileDialog();
            string location = "null";
            inputFileDialog.Title = "Select an image file";
            inputFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
            if (inputFileDialog.ShowDialog() == DialogResult.OK)
            {
                location = inputFileDialog.FileName;
            }
            else
            {
                MessageBox.Show("Image not selected! Aborting!", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;

            }
            image = new Bitmap(location);
            SaveFileDialog outputFileDialog = new SaveFileDialog();
            outputFileDialog.Title = "Choose where to save converted file";
            outputFileDialog.Filter = "RIF Image(*.RIF)|*.RIF";
            outputFileDialog.AddExtension = true;
            if (outputFileDialog.ShowDialog() == DialogResult.OK)
            {
                location = outputFileDialog.FileName;
            }
            else
            {
                MessageBox.Show("Output location not selected! Aborting!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            outputRIF = File.Create(location);
            convert();
        }

        private void convert()
        {
            
            ArrayList imageData = new ArrayList();
            int h = (int) image.Width;
            
            int v = (int) image.Height;
            
            progressBar1.Maximum = (h * v);
            progressBar1.Step = 1;

            //outputRIF.AutoFlush = true;
            
            writeHelper(Base36.Encode(h));
            writeHelper("|");
            writeHelper(Base36.Encode(v));
            writeHelper("|");
            outputRIF.Flush();
            int i;
            for(i = 0; i < v; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Color pixel = image.GetPixel(j, i);
                    if (imageData.Contains(pixel.ToArgb().ToString()))
                    {
                        int idx = imageData.IndexOf(pixel.ToArgb().ToString());
                        writeHelper("&" + Base36.Encode(idx));
                    }
                    else
                    {
                        imageData.Add(pixel.ToArgb().ToString());
                        string sub = pixel.ToArgb().ToString();
                        if (pixel.ToArgb() < 0)
                        {
                            sub = sub.Substring(1);
                            writeHelper(Base36.Encode(int.Parse(sub)));
                        }
                        else
                        {
                            writeHelper("+" + Base36.Encode(int.Parse(sub)));
                        }


                    }
                    writeHelper("|");
                    outputRIF.Flush();
                    progressBar1.PerformStep();
                }
            }
            outputRIF.Close();
            MessageBox.Show("RIF sucessfully created.");
            this.Close();
        }

        /*
         * Helper Methods
         */
        private void writeHelper(string data)
        {
            
            byte[] toWrite = new ASCIIEncoding().GetBytes(data);
            outputRIF.Write(toWrite,0,toWrite.Length);
            //outPos += toWrite.Length-1;
        }

        private string toHex(int num)
        {
            return num.ToString("X");
        }
        //To go back to int
        //return Convert.ToInt32(hex, 16);
    }
}

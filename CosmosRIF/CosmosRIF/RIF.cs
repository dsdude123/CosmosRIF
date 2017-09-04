using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using mstum.utils.cosmos;
using Cosmos.System.Graphics;


namespace CosmosRIF
{
    public class RIF
    {


        private FileStream inputRIF;


        public void DrawImage(string path, Canvas c, int x, int y)
        {
            inputRIF = File.Open(path,FileMode.Open);
            draw(c,x,y);

        }
        private void draw(Canvas c, int x, int y)
        {
            
            List<int> imageData = new List<int>();

            int h = (int)Base36.Decode(readHelper());
            int v = (int)Base36.Decode(readHelper());


            int i;
            for (i = 0; i < v; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    String nextVal = readHelper();
                    Color pixel;
                    if (nextVal.Contains("&"))
                    {
                        int idx = (int)Base36.Decode(nextVal.Substring(1));
                        pixel = Color.FromArgb((int) imageData[idx]);
                        
                    }
                    else
                    {
                        int argb;
                        if (nextVal.Contains("+"))
                        {
                            argb = (int)Base36.Decode(nextVal.Substring(1));
                            pixel = Color.FromArgb(argb);

                        }
                        argb = -(int)Base36.Decode(nextVal);
                        imageData.Add(argb);
                        pixel = Color.FromArgb(argb);
                    }

                    Pen color = new Pen(pixel);
                    c.DrawPoint(color,x+j,y+i);
                }
                    
                
            }
        }

        /*
         * Helper Methods
         */
        private string readHelper()
        {
            bool foundSeperator = false;
            List<string> data = new List<String>();
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

        
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Robot_GUI
{
    public partial class Form1 : Form
    {
        private static TcpClient client;
        private static NetworkStream stream;
        private static byte[] message;
        private int degrees;
        private int distance;

        public Form1()
        {
            InitializeComponent();
            this.degrees = 90;
            this.distance = 60;
            this.label1.Text = "Turn Degrees: " + this.degrees.ToString();
            this.label2.Text = "Distance to Move: " + this.distance.ToString();
            this.chart1.ChartAreas["Chart"].AxisX.Maximum = 160;
            this.chart1.ChartAreas["Chart"].AxisX.Minimum = 0;
            this.chart1.ChartAreas["Chart"].AxisY.Maximum = 80;
            this.chart1.ChartAreas["Chart"].AxisY.Minimum = 0;
            this.chart1.Series["Player"].Points.AddXY(80.0, 0.0);

            try
            {
                client = new TcpClient("192.168.1.1", 42880);
                stream = client.GetStream();
            }
            catch (Exception e)
            {
                Console.Write("Failed" + e);
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        // Forward
        private void button1_Click(object sender, EventArgs e)
        {
            sendCommand("w");

            this.label3.Text = recieveData();
            this.label3.Update();
        }

        // Backward
        private void button2_Click(object sender, EventArgs e)
        {
            sendCommand("s");

            this.label3.Text = recieveData();
            this.label3.Update();
        }

        // Left
        private void button3_Click(object sender, EventArgs e)
        {
            sendCommand("a");

            this.label3.Text = recieveData();
            this.label3.Update();
        }

        // Right
        private void button4_Click(object sender, EventArgs e)
        {
            sendCommand("d");

            this.label3.Text = recieveData();
            this.label3.Update();
        }

        // distance up-i down-k degree up-l down-j

        // + 15 degrees
        private void button5_Click(object sender, EventArgs e)
        {
            this.degrees += 15;
            if (this.degrees > 180)
            {
                this.degrees = 180;
            }
            this.label1.Text = "Turn Degrees: " + this.degrees.ToString();
            this.label1.Update();
            sendCommand("l");
        }

        // - 15 degrees
        private void button6_Click(object sender, EventArgs e)
        {
            this.degrees += -15;
            if (this.degrees < 0)
            {
                this.degrees = 0;
            }
            this.label1.Text = "Turn Degrees: " + this.degrees.ToString();
            this.label1.Update();
            sendCommand("j");
        }

        // Detect Surroundings
        private void button7_Click(object sender, EventArgs e)
        {
            this.chart1.Series["Big"].Points.Clear();
            this.chart1.Series["Little"].Points.Clear();
            sendCommand(" ");

            String[] values = recieveData().Split(' ');
            int i;
            String[] coord;
            double x = 0.0, y = 0.0;

            for(i = 0; i < values.Length; i++)
            {
                if (i % 2 == 0)
                {
                    coord = values[i].Split(',');
                    x = Convert.ToDouble(coord[0]);
                    y = Convert.ToDouble(coord[1]);
                }
                else
                {
                    if (values[i] == "B")
                    {
                        this.chart1.Series["Big"].Points.AddXY(x, y);
                    }
                    else
                    {
                        this.chart1.Series["Little"].Points.AddXY(x, y);
                    }
                }
            }
        }

        // Distance +10 start 60
        private void button8_Click(object sender, EventArgs e)
        {
            this.distance += 10;
            this.label2.Text = "Distance to Move: " + this.distance.ToString();
            this.label2.Update();
            sendCommand("i");
        }

        // Distance -10
        private void button9_Click(object sender, EventArgs e)
        {
            this.distance += -10;
            if (this.distance < 0)
            {
                this.distance = 0;
            }
            this.label2.Text = "Distance to Move: " + this.distance.ToString();
            this.label2.Update();
            sendCommand("k");
        }

        private void sendCommand(String command)
        {
            try
            {
                message = Encoding.ASCII.GetBytes(command);
                stream.Write(message, 0, message.Length);
            }
            catch (Exception e)
            {
                Console.Write("Exception " + e);
            }
        }

        private string recieveData()
        {
            try
            {
                //int length = stream.Read(message, 0, message.Length);
                //return Encoding.ASCII.GetString(message, 0, length);
                byte[] myReadBuffer = new byte[1024];
                StringBuilder myCompleteMessage = new StringBuilder();
                int numberOfBytesRead = 0;

                // Incoming message may be larger than the buffer size.
                do
                {
                    numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);

                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

                }
                while (stream.DataAvailable);

                return myCompleteMessage.ToString();
            }
            catch (Exception e)
            {
                return "Exception " + e.ToString();
            }
        }

        private void adjustPositions()
        {

        }
    }
}

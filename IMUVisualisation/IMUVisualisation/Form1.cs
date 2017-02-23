using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace IMUVisualisation
{
    public partial class Form1 : Form
    {
        int i = 0;
        int time = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Otwórz port")
            {
                

                Int32 predkosc = Int32.Parse(comboBox1.SelectedItem.ToString());
                serialPort1.BaudRate = predkosc;
                // Portszeregowy.BaudRate =  predkosc;
                // Portszeregowy.
                serialPort1.PortName = comboBox2.SelectedItem.ToString();
                serialPort1.Open();
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(otrzymaniedanych);


                if (serialPort1.IsOpen)
                {
                    button2.Enabled = true;
                    button1.Text = "Zakmnij port";

                    MessageBox.Show("otwarto port");
                }
                else
                    MessageBox.Show("Nieudało się otworzyć portu szeregowego");
            }
            else
            {
                serialPort1.Close();
                button2.Enabled = false;
                button1.Text = "Otwórz port";
                MessageBox.Show("Port został zamkniety");

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string wiadomosc = textBox1.Text;
            serialPort1.WriteLine(wiadomosc);
            textBox1.Text = "";

        }

        private void Form1_Load(object sender, EventArgs e)
        {
             string[] ports = SerialPort.GetPortNames();

            string[] bounds = { "2400","9600", "19200","115200","230400"};
            foreach (string port in ports)
            {
                comboBox2.Items.Add(port);

            }

            foreach (string bound in bounds)
            {
                comboBox1.Items.Add(bound);

            }
            comboBox2.SelectedItem = comboBox2.Items[1];
            comboBox1.SelectedItem = comboBox1.Items[1];
        }
        public void newdata_odchylenie(int txt)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<int>(newdata_odchylenie), new object[] { txt });
                return;
            }
            this.chart1.Series["Odchylenie"].Points.AddXY(time, txt);
        }
        public void newdata_przechylenie(int txt)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<int>(newdata_przechylenie), new object[] { txt });
                return;
            }
            this.chart1.Series["Przechylenie"].Points.AddXY(time, txt);
        }
        public void newdata_pochylenie(int txt)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<int>(newdata_pochylenie), new object[] { txt });
                return;
            }
            this.chart1.Series["Pochylenie"].Points.AddXY(time, txt);
        }

        private void otrzymaniedanych(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string message = serialPort1.ReadLine();

            //Console.WriteLine(message);
            time++;
            int i = 0;
            int j = 0;
            int[] pozycja = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (i = 0; i < message.Length; i++)
            {
                if (message[i] == '+' | message[i] == '-')
                {

                    pozycja[j] = i;
                    j = j + 1;
                }

            }

            char odchylenieznak = message[pozycja[0]];
            char odchylenie1 = message[pozycja[0] + 1];
            char odchylenie2 = message[pozycja[0] + 2];
            char odchylenie3 = message[pozycja[0] + 3];
            string odchyleniestring = odchylenie1.ToString() + odchylenie2.ToString() + odchylenie3.ToString();
            int odchylenieint = Int32.Parse(odchyleniestring);
            if (odchylenieznak == '-')
            {

                odchylenieint = odchylenieint * (-1);

            }
            newdata_odchylenie(odchylenieint);

            char pochylenieznak = message[pozycja[1]];
            char pochylenie1 = message[pozycja[1] + 1];
            char pochylenie2 = message[pozycja[1] + 2];
            char pochylenie3 = message[pozycja[1] + 3];
            string pochyleniestring = pochylenie1.ToString() + pochylenie2.ToString() + pochylenie3.ToString();
            int pochylenieint = Int32.Parse(pochyleniestring);
            if (pochylenieznak == '-')
            {

                pochylenieint = pochylenieint * (-1);

            }
            newdata_pochylenie(pochylenieint);

            char przechylenieznak = message[pozycja[2]];
            char przechylenie1 = message[pozycja[2] + 1];
            char przechylenie2 = message[pozycja[2] + 2];
            char przechylenie3 = message[pozycja[2] + 3];
            string przechyleniestring = przechylenie1.ToString() + przechylenie2.ToString() + przechylenie3.ToString();
            int przechylenieint = Int32.Parse(przechyleniestring);
            if (przechylenieznak == '-')
            {

                przechylenieint = przechylenieint * (-1);

            }
            newdata_przechylenie(przechylenieint);

        }
    }
}

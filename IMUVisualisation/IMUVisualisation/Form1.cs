using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;

namespace IMUVisualisation
{
    public partial class Form1 : Form
    {
        int i = 0;
        int time = 0;
        private int[] odchylenieArray = new int[100];
        private int[] przechylenieArray = new int[100];
        private int[] pochylenieArray = new int[100];
        private string[] messageArray = new string[1000];
        private Thread ImuThread;
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
                ImuThread = new Thread(new ThreadStart(this.IMUHandler));
                ImuThread.IsBackground = true;
                ImuThread.Start();


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
        private void IMUHandler()
        {
            
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
        public void newdata_odchylenie()
        {
            chart1.Series["Odchylenie"].Points.Clear();
            for (int i = 0; i < odchylenieArray.Length - 1; ++i)
            {
                this.chart1.Series["Odchylenie"].Points.AddY(odchylenieArray[i]);
            }
        }
        public void newdata_przechylenie()
        {
            chart1.Series["Przechylenie"].Points.Clear();
            for (int i = 0; i < przechylenieArray.Length - 1 ; ++i)
            {
                this.chart1.Series["Przechylenie"].Points.AddY(przechylenieArray[i]);
            }
        }
        public void newdata_pochylenie()
        {
             chart1.Series["Pochylenie"].Points.Clear();
            for (int i = 0; i < pochylenieArray.Length - 1; ++i)
            {
                this.chart1.Series["Pochylenie"].Points.AddY(pochylenieArray[i]);
            }
        }
        
        private void otrzymaniedanych(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            
                string message = serialPort1.ReadLine();

                messageArray[messageArray.Length - 1] = message + "<<<>>>" + DateTime.Now.ToString();  

                Array.Copy(messageArray, 1, messageArray, 0, messageArray.Length - 1);
                

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


                if(button3.Text == "Stop")
                {
                pochylenieArray[pochylenieArray.Length - 1] = pochylenieint;
                Array.Copy(pochylenieArray, 1, pochylenieArray, 0, pochylenieArray.Length - 1);
                if (chart1.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { newdata_pochylenie(); });
                }


                odchylenieArray[odchylenieArray.Length - 1] = odchylenieint;
                Array.Copy(odchylenieArray, 1, odchylenieArray, 0, odchylenieArray.Length - 1);
                if (chart1.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { newdata_odchylenie(); });
                }




                przechylenieArray[przechylenieArray.Length - 1] = przechylenieint;
                Array.Copy(przechylenieArray, 1, przechylenieArray, 0, przechylenieArray.Length - 1);
                if (chart1.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { newdata_przechylenie(); });
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Start")
            {
                button3.Text = "Stop";
            }
            else
            {
                button3.Text = "Start";
            }
        }
        int numerpliku = 1;
        private void button4_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllLines(@"C:\Users\Lunatyk\Desktop\TestFolder\WriteLines" + numerpliku.ToString() + ".txt", messageArray);
            numerpliku++;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rebex.Net;
using Rebex.TerminalEmulation;
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO.Ports;


namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        int att1 = 0;
        int att2 = 0;
        string[] port;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void telnet_notify()

        {
            //TcpClient tc1 = new TcpClient();
            //TcpClient tc2 = new TcpClient();
            NetworkStream netStream1=null;
            NetworkStream netStream2=null;


            int port1 = 0;
            string hostname1 = "";
            Dispatcher.Invoke(() => hostname1 = servernamebox.Text);
            Dispatcher.Invoke(() => port1 = Convert.ToInt32(portbox.Text));
            //tc1 = new TcpClient(hostname1, port1);
            var tc1 = new TcpClient();

            if (!tc1.ConnectAsync(hostname1, port1).Wait(1000))
            {
                att1 = 1;
            }
            else
            {
                att1 = 2;
                netStream1 = tc1.GetStream();
                netStream1.ReadTimeout = 1000;
            }

            
            int port2 = 0;
            string hostname2 = "";
            Dispatcher.Invoke(() => hostname2 = servernamebox2.Text);
            Dispatcher.Invoke(() => port2 = Convert.ToInt32(portbox2.Text));
            //tc2 = new TcpClient(hostname2, port2);

            var tc2 = new TcpClient();
            if (!tc2.ConnectAsync(hostname2, port2).Wait(1000))
            {
                att2 = 1;
            }
            else
            {
                att2 = 2;
                netStream2 = tc2.GetStream();
                netStream2.ReadTimeout = 1000;
            }

            
            byte[] bytes1 = new byte[tc1.ReceiveBufferSize];
            byte[] bytes2 = new byte[tc2.ReceiveBufferSize];

            
            while (true)
            {   if (Dispatcher.Invoke(() => (flag.Content).ToString() == "0")) { break; }


                string comand="";
                double value=0;
                Dispatcher.Invoke(() => value = slider1.Value);
                value = Math.Round(value, 2);
                comand=Convert.ToString(value);
                if (value < 10) { comand = "0" + comand; }
                comand=comand.Replace(",", "");
                if (comand.Length ==3) { comand += "0"; }
                if (comand.Length == 2) { comand += "00"; }
                comand="wv0"+comand+"\n";
                
                Dispatcher.Invoke(() => output_com.Text = comand);
                com.WriteLine(comand);
                string message = com.ReadLine();
                Dispatcher.Invoke(() => output_com.Text = message);
                //comm_DataSend(comand,com);

                if (att1 == 2)
                {
                    try
                    {   
                        netStream1.Read(bytes1, 0, (int)tc1.ReceiveBufferSize);
                        string returndata1 = Encoding.ASCII.GetString(bytes1);
                        Trace.WriteLine("test connected");
                        Dispatcher.Invoke(() => output_textblock.Text = returndata1);
                    }
                    catch (Exception) { Dispatcher.Invoke(() => output_textblock.Text = "no answer"); }
                }

                if (att2 == 2)
                {
                    try
                    {
                        netStream2.Read(bytes2, 0, (int)tc2.ReceiveBufferSize);
                        string returndata2 = Encoding.ASCII.GetString(bytes2);
                        Trace.WriteLine("test connected");
                        Dispatcher.Invoke(() => output_textblock2.Text = returndata2);

                    }
                    catch (Exception) { Dispatcher.Invoke(() => output_textblock2.Text = "no answer"); }
                }
                Task.Delay(1000).Wait();
            }
            tc1.Close();
            tc2.Close();
            Dispatcher.Invoke(() => output_textblock.Text = "not connected");
            Dispatcher.Invoke(() => output_textblock2.Text = "not connected");

        }

        private void att1_value(double value, TcpClient tc)
        {
            string cmd = "";
            NetworkStream netStream = tc.GetStream();
            if (netStream.CanWrite)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd + "\r");
                netStream.Write(sendBytes, 0, sendBytes.Length);
            }
            Thread.Sleep(1000);
        }
        private void att2_value(double value, TcpClient tc)
        {
            string cmd = "";
            NetworkStream netStream = tc.GetStream();
            if (netStream.CanWrite)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes(cmd + "\r");
                netStream.Write(sendBytes, 0, sendBytes.Length);
            }
            Thread.Sleep(1000);
        }

        

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {   if (checkbox1.IsChecked == true) { slider1.Value = slider1.Value - (slider1.Value%0.25); }
            textbox1.Text = (slider1.Value).ToString();

            if (boundcheckbox.IsChecked == true) { equalize1_Click(sender, e); }
            //telnet_notify();
        }

        private void textbox1_TextChanged(object sender, TextChangedEventArgs e)
        {try
            {slider1.Value= Convert.ToDouble(textbox1.Text);}
            catch (Exception){}
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            slider1.SmallChange = 0.1;
        }

        private void CheckBox1_Checked(object sender, RoutedEventArgs e)
        {
            slider1.Value = slider1.Value - (slider1.Value % 0.25);
        }

        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (checkbox2.IsChecked == true) { slider2.Value = slider2.Value - (slider2.Value % 0.25); }
            textbox2.Text = (slider2.Value).ToString();
            if (boundcheckbox.IsChecked == true) { equalize2_Click(sender, e); }
            //telnet_notify();
        }

        private void textbox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {slider2.Value = Convert.ToDouble(textbox2.Text);}
            catch (Exception){}
        }

        private void equalize1_Click(object sender, RoutedEventArgs e)
        {
            if (checkbox1.IsChecked != checkbox2.IsChecked) { checkbox2.IsChecked = checkbox1.IsChecked; }
            slider2.Value = slider1.Value;

        }

        private void CheckBox2_Checked(object sender, RoutedEventArgs e)
        {
            slider2.Value = slider2.Value - (slider2.Value % 0.25);
        }

        private void equalize2_Click(object sender, RoutedEventArgs e)
        {
            if (checkbox1.IsChecked != checkbox2.IsChecked) { checkbox1.IsChecked = checkbox2.IsChecked; }
            slider1.Value = slider2.Value;
        }

        private void slider1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double value = slider1.Value;
            if (checkbox1.IsChecked == true) { 
            slider1.Value += 0.25 * (e.Delta / Math.Abs(e.Delta));
        }
            else
            {
                slider1.Value += 0.1 * (e.Delta / Math.Abs(e.Delta));
            }
        }

        private void slider2_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double value = slider2.Value;
            if (checkbox2.IsChecked == true)
            {
                slider2.Value += 0.25 * (e.Delta / Math.Abs(e.Delta));
            }
            else
            {
                slider2.Value += 0.1 * (e.Delta / Math.Abs(e.Delta));
            }
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            disconnect_button.IsEnabled = true;
            flag.Content = "1";
            this.com.PortName = port[combox1.SelectedIndex];
            this.com.BaudRate = 115200;
            this.com.StopBits = StopBits.One;
            this.com.Parity = Parity.None;
            this.com.DataBits = 8;
            this.com.Open();

            
            att1 = 1;
            att2 = 2;
            var inner = Task.Factory.StartNew(() =>  // вложенная задача
            {

                telnet_notify();

            }, TaskCreationOptions.AttachedToParent);
            connect_button.IsEnabled = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            att2 = 1;
            var inner = Task.Factory.StartNew(() =>  // вложенная задача
            {

                telnet_notify();

            }, TaskCreationOptions.AttachedToParent);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            connect_button.IsEnabled = true;
            com.Close();

            flag.Content = "0";
            output_textblock.Text = "not connected";
            output_textblock2.Text = "not connected";
            disconnect_button.IsEnabled=false;

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void combox1_Initialized(object sender, EventArgs e)
        {
            port = SerialPort.GetPortNames();
            combox1.ItemsSource = port;
        }
        private SerialPort com = new SerialPort();
        public void comm_DataSend(string str, SerialPort com)
        {
            try
            {
                List<byte> byteList = new List<byte>();
                byte[] byteListIn = new byte[128];
                str = str.Replace(" ", "");
                for (int startIndex = 0; startIndex < str.Length; startIndex += 2)
                    byteList.Add(Convert.ToByte(str.Substring(startIndex, 2), 16));

                com.Write(byteList.ToArray(), 0, byteList.Count);

                
            }
            catch (Exception ex)
            {

            }
        }
    }
    
}

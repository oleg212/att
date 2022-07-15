using System;
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
namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int att1 = 0;
        int att2 = 0;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void telnet_notify()

        {
            TcpClient tc1 = new TcpClient();
            TcpClient tc2 = new TcpClient();
            
            //NetworkStream netStream2 = tc2.GetStream();
            int port1 = 0;
            string hostname1 = "";
            Dispatcher.Invoke(() => hostname1 = servernamebox.Text);
            Dispatcher.Invoke(() => port1 = Convert.ToInt32(portbox.Text));
            tc1 = new TcpClient(hostname1, port1);
            NetworkStream netStream1 = tc1.GetStream();
            att1 = 2;
            netStream1 = tc1.GetStream();
            netStream1.ReadTimeout = 1000;

            //if (att1 == 1) {
            //    int port1 = 0;
            //    string hostname1 = "";
            //    Dispatcher.Invoke(() => hostname1 = servernamebox.Text);
            //    Dispatcher.Invoke(() => port1 = Convert.ToInt32(portbox.Text));
            //    tc1 = new TcpClient(hostname1, port1);
            //    att1 = 2;
            //    netStream1 = tc1.GetStream();
            //    try
            //    {
            //        netStream1 = tc1.GetStream();
            //        netStream1.ReadTimeout = 1000;
            //    }
            //    catch (Exception) { }

            //}
            //if (att2 == 1)
            //{
            //    int port2 = 0;
            //    string hostname2 = "";
            //    Dispatcher.Invoke(() => hostname2 = servernamebox2.Text);
            //    Dispatcher.Invoke(() => port2 = Convert.ToInt32(portbox2.Text));
            //    tc2 = new TcpClient(hostname2, port2);
            //    att2 = 2;
            //    netStream2 = tc2.GetStream();
            //    try
            //    {
            //        netStream2 = tc2.GetStream();
            //        netStream2.ReadTimeout = 1000;
            //    }
            //    catch (Exception) { }
            //}


            //Dispatcher.Invoke(() => output_textblock.Text = GetAnswer(tc));





            byte[] bytes1 = new byte[tc1.ReceiveBufferSize];
            //byte[] bytes2 = new byte[tc2.ReceiveBufferSize];
            
            
            while ((tc1.Connected))
            {
                try
                {   
                    netStream1.Read(bytes1, 0, (int)tc1.ReceiveBufferSize);
                }
                catch (Exception) { }
                try
                {
                    //netStream2.Read(bytes2, 0, (int)tc2.ReceiveBufferSize);
                }
                catch (Exception) { }
                string returndata1 = Encoding.ASCII.GetString(bytes1);
                //string returndata2 = Encoding.ASCII.GetString(bytes2);
                Trace.WriteLine("test connected");
                Task.Delay(1000).Wait();
                Dispatcher.Invoke(() => output_textblock.Text = returndata1);
                //Dispatcher.Invoke(() => output_textblock2.Text = returndata2);

            }
            //att1_value(slider1.Value, tc);
            //att2_value(slider2.Value, tc);

            
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
        {
            try
            {   
                
                slider1.Value= Convert.ToDouble(textbox1.Text);
            }
            catch (Exception)
            {

                
            }
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
            {

                slider2.Value = Convert.ToDouble(textbox2.Text);
            }
            catch (Exception)
            {


            }
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
            att1 = 1;
            var inner = Task.Factory.StartNew(() =>  // вложенная задача
            {

                telnet_notify();

            }, TaskCreationOptions.AttachedToParent);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            att2 = 1;
            var inner = Task.Factory.StartNew(() =>  // вложенная задача
            {

                telnet_notify();

            }, TaskCreationOptions.AttachedToParent);
        }
    }
    
}

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
using System.Threading;
using HtmlAgilityPack;
using System.Net;
using System.IO;
namespace BF3_SCAN
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Task Scan_Task;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void bt_scan(object sender, RoutedEventArgs e)
        {
            if (button_scan.Content.Equals("Start scanning"))
            {
                button_scan.Content = "Stop Scanning";
                Console.Out.WriteLine("Server URL" + Server_URL.GetLineText(0));
                Console.WriteLine("Scan_Rate" + Scan_Rate.GetLineText(0));
                string Request_Map_para = "No_set";
                Int32 Scan_Rate_para = Int32.Parse(Scan_Rate.GetLineText(0));
                string Server_URL_para = Server_URL.GetLineText(0);




                if (cb_Kharg.IsChecked == true)
                {

                    Request_Map_para = (string)cb_Kharg.Content;
                    Console.WriteLine(Request_Map_para);

                }

                if (cb_Wake.IsChecked == true)
                {

                    Request_Map_para = (string)cb_Wake.Content;
                    Console.WriteLine(Request_Map_para);

                }

                if (cb_Shargi.IsChecked == true)
                {

                    Request_Map_para = (string)cb_Shargi.Content;
                    Console.WriteLine(Request_Map_para);

                }

                if (cb_Noshahr.IsChecked == true)
                {

                    Request_Map_para = (string)cb_Noshahr.Content;
                    Console.WriteLine(Request_Map_para);

                }

                SCAN_parameters sc_para = new SCAN_parameters(Server_URL_para, Scan_Rate_para, Request_Map_para);

                Scan_Task = Task.Factory.StartNew(() => {
                    GetServerInfo(sc_para);
                });


            }
            else
            {
                button_scan.Content = "Start scanning";
            }



        }

        private void GetServerInfo(object sc_para)
        {
            SCAN_parameters this_sc_para = (SCAN_parameters)sc_para;
            Console.WriteLine("Thread parameter" + (string)this_sc_para.Get_Server_url() + " " + (string)this_sc_para.Get_Requested_Map());
            String this_url = (string)this_sc_para.Get_Server_url();
            string this_requested_map = (string)this_sc_para.Get_Requested_Map();
            Int32 this_serv_freq = this_sc_para.Get_Polling_Freq();
            string map_running = "noset";
            DateTime RoundBeginTime = DateTime.Now;
            while (true)
            {
                // var html = @;
                HtmlWeb web = new HtmlWeb();
                var htmlDoc = web.Load(this_url);
                var server_name = htmlDoc.DocumentNode.SelectNodes("//div/h1");
                var maps = htmlDoc.DocumentNode.SelectSingleNode("//p/strong");
                var modes = htmlDoc.DocumentNode.SelectNodes("//div/p");
                if (!map_running.Equals(maps.InnerHtml))
                {
                    RoundBeginTime = DateTime.Now;
                    map_running = maps.InnerHtml;
                }
                //Console.WriteLine("Mode    :   " + mode[1].InnerHtml);
                string modes_separate = modes[1].InnerHtml;
                string[] words = modes_separate.Split(' ');
                int ptr = 0;
                string mode_str = " ";
                foreach (var word in words)
                {
                    //System.Console.WriteLine($"<{word}>");
                    if (word.Contains("Conquest"))
                    {
                        if (words[ptr + 1].Contains("Large") || words[ptr + 1].Contains("Assault"))
                        {
                            if (words[ptr + 2].Contains("Large"))
                            {
                                mode_str = words[ptr] + " " + words[ptr + 1] + " " + words[ptr + 2];
                            }
                            else
                            {
                                mode_str = words[ptr] + " " + words[ptr + 1];
                            }
                        }
                        //System.Console.WriteLine(words[ptr]);
                        else
                        { mode_str = words[ptr]; }
                    }
                    if (word.Contains("Rush"))
                    {
                        mode_str = words[ptr];
                    }


                    else
                    {
                        ptr++;
                    }
                }
                //   Console.Clear();




                Console.WriteLine("------------SCAN PARAMETER-------------------");
                Console.WriteLine("URL             :");
                Console.WriteLine(this_url);
                Console.WriteLine("REQUESTED MAP   : " + this_requested_map);
                Console.WriteLine("SCAN FREQ [s]   : " + this_serv_freq);

                string last_scan = DateTime.Now.ToString();
                string scan_duration = DateTime.Now.Subtract(RoundBeginTime).ToString();
                Dispatcher.Invoke(() => {

                    Result_Server_name.Text = server_name[1].InnerHtml;
                    Result_running_map.Text = map_running;
                    Result_mode.Text = mode_str;
                    Result_last_scan.Text = last_scan;
                    Result_scan_duration.Text = scan_duration;



                    if (map_running.Contains(this_requested_map))
                    {


                        Result_matching_text.Content = this_requested_map + " now! ";
                        Result_matching_rect.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                        Console.WriteLine("!!!!!!!!!!Your map is running!!!!!!!       :");
                        Console.WriteLine("!!!!!!!!!!Your map is running!!!!!!!       :");
                        Console.WriteLine("!!!!!!!!!!Your map is running!!!!!!!       :");
                        Console.WriteLine("!!!!!!!!!!Your map is running!!!!!!!       :");
                    }

                    else
                    {

                        Result_matching_rect.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);

                    }





                });






                Console.WriteLine("Server          :   " + server_name[1].InnerHtml);


                Console.WriteLine("------------SCAN RESULTS----------------------");
                Console.WriteLine("Last SCAN       :" + DateTime.Now);
                Console.WriteLine("Server          :   " + server_name[1].InnerHtml);
                Console.WriteLine("MAP             :   " + map_running);
                Console.WriteLine("MODE            :   " + mode_str);
                Console.WriteLine("  ");
                Console.WriteLine("Round Duration  :" + DateTime.Now.Subtract(RoundBeginTime));

                if (map_running.Contains(this_requested_map))
                {



                    Console.WriteLine("!!!!!!!!!!Your map is running!!!!!!!       :");
                    Console.WriteLine("!!!!!!!!!!Your map is running!!!!!!!       :");
                    Console.WriteLine("!!!!!!!!!!Your map is running!!!!!!!       :");
                    Console.WriteLine("!!!!!!!!!!Your map is running!!!!!!!       :");
                }


                try
                {
                    Thread.Sleep(1000 * this_serv_freq);
                }
                catch (ThreadInterruptedException) { }
            }
        }


        public class SCAN_parameters
        {
            string Server_url;
            Int32 Polling_Freq;
            string Requested_Map;
            public SCAN_parameters(string Server_url, Int32 Polling_Freq, string Requested_Map)
            {
                this.Polling_Freq = Polling_Freq;
                this.Requested_Map = Requested_Map;
                this.Server_url = Server_url;
            }
            public string Get_Server_url()
            {
                return Server_url;
            }
            public Int32 Get_Polling_Freq()
            {
                return Polling_Freq;
            }
            public string Get_Requested_Map()
            {
                return Requested_Map;
            }
        }


    }
}

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
using System.Configuration;
using System.Globalization;

namespace TSimulater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 60 * 1000;
            pathTextbox.Text = ConfigurationManager.AppSettings["path"];
        }

        Dictionary<string, string> fileList = new Dictionary<string, string>();
        private void startBtn_Click(object sender, EventArgs e)
        {
            ConfigurationManager.AppSettings["path"] = pathTextbox.Text;


            AddLog(startBtn.Text);


            if ( startBtn.Text == "start")
            {
                startBtn.Text = "stop";


                timer1.Start();

                timer1_Tick(null,null);
            }
            else
            {
                startBtn.Text = "start";
                timer1.Stop();
            }

            

        }



        private void eventBtn_Click(object sender, EventArgs e)
        {
            var destRecent = Path.Combine(pathTextbox.Text, @"TeslaCam\RecentClips\");
            var destSave = Path.Combine(pathTextbox.Text, @"TeslaCam\SavedClips\");

            moveFile(destRecent, destSave);
        }


        public async Task CopyFilesAsync(string SourceName, string DestinationName)
        {
            
            {
                using (FileStream sourceStream = File.Open(SourceName, FileMode.Open))
                {
                    using (FileStream destinationStream = File.Create(DestinationName))
                    {
                        await sourceStream.CopyToAsync(destinationStream);
                    }
                }
            }
        }




        private async void timer1_Tick(object sender, EventArgs e)
        {

            AddLog("start write recentclips");

            

            var destRecent = Path.Combine(pathTextbox.Text , @"TeslaCam\RecentClips\");
            //var destSave = Path.Combine(pathTextbox.Text , @"TeslaCam\SavedClips\");

            var now = DateTime.Now;

            Directory.CreateDirectory(destRecent);

            foreach (var item in fileList)
            {
                

                var dest = Path.Combine(destRecent, now.ToString("yyyy-MM-dd_HH-mm-ss-") + item.Key+".mp4");

                AddLog($"write file {dest}");


                try
                {
                    await CopyFilesAsync(item.Value, dest);
                }
                catch (Exception ex)
                {

                    AddLog(ex.Message);

                }
                
            }

            AddLog("end write recentclips");

            AddLog($"wait time({timer1.Interval/1000} sec)");
        }

        private void AddLog(string v)
        {
            listBox1.Items.Insert(0, v);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var file = "sample.mp4";

            fileList.Add("front", file);
            fileList.Add("back", file);
            fileList.Add("left_repeater", file);
            fileList.Add("right_repeater", file);
        }

        private void sentryBtn_Click(object sender, EventArgs e)
        {
            var destRecent = Path.Combine(pathTextbox.Text, @"TeslaCam\RecentClips\");
            var destSave = Path.Combine(pathTextbox.Text, @"TeslaCam\SentryClips\");

            moveFile(destRecent,destSave);
        }


        private void moveFile(string destRecent, string destSave)
        {

            var list = Directory.GetFiles(destRecent, "*.mp4");

            var flist = list.Select(item => new FileInfo(item)).OrderBy(item => item.CreationTime).ToList();

            destSave = Path.Combine(destSave, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));

            Directory.CreateDirectory(destSave);

            for (int i = 0; i < 40 && i < flist.Count; i++)
            {
                var dest = Path.Combine(destSave, flist[i].Name);

                AddLog($"move file {dest}");


                File.Move(flist[i].FullName, dest);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int value))
            {
                timer1.Interval = value * 1000;

                MessageBox.Show("update success");

            }
            else
            {
                MessageBox.Show("update fail");
            }


        }
    }
}

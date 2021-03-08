using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Security8
{
    public partial class Form1 : Form
    {
        
        const int WM_DEVICECHANGE = 0x0219; //see msdn site
        const int DBT_DEVICEARRIVAL = 0x8000;
        const int DBT_DEVICEREMOVALCOMPLETE = 0x8004;
        const int DBT_DEVTYPVOLUME = 0x00000002;
        Cryptor cryptor;
        public Form1()
        {
            InitializeComponent();
            cryptor = new Cryptor();
            refreshDisks();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_DEVICECHANGE)
                refreshDisks();
        }

        async Task<HttpResponseMessage> PostRequest(string url , Dictionary<string, string> values)
        {
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, content);
            
            return response;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void login_Click(object sender, EventArgs e)
        {
            lab.Visible = false;
            resLogin.Text = "";
            string text = "";
            if(selectedDiskForLogin == "")
            {
                MessageBox.Show("Виберіть диск");
                return;
            }

            if (File.Exists($"{selectedDiskForLogin}keys.txt"))
            {
                text = TextReader.getText($"{selectedDiskForLogin}keys.txt");
            }
            else {
                MessageBox.Show("Диск не містить ключа");
                return;
            }



            if (text == "")
            {
                resLogin.Text = "У вас немає ключа";
                return;
            }
            var list = text.Split('\n').ToList();
            string name = list.First();
            Random random = new Random();
            string password = list[random.Next(1, list.Count)];
            var values = new Dictionary<string, string>
            {
                { "name", name },
                { "password", password }
            };
            var response = await PostRequest("http://127.0.0.1:8000/login", values);
            if(response.IsSuccessStatusCode)
            {
                lab.Visible = true;
                resLogin.Text = name;
            }
        }

        private async void register_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && selectedDisk != "")
            {
                var values = new Dictionary<string, string>
            {
                { "name",  textBox1.Text}
            };
                var response = await PostRequest("http://127.0.0.1:8000/register", values);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    string[] arr = JsonConvert.DeserializeObject<string[]>(result);
                    List<string> enc = new List<string>();
                    enc.Add(textBox1.Text);
                    foreach (var a in arr)
                    {
                        enc.Add(cryptor.Encrypt(a));
                    }
                    string fileText = string.Join("\n", enc.ToArray());
                    string filename = $"{selectedDisk}keys.txt";
                    try
                    {
                        TextReader.toFile(filename, fileText);
                        File.SetAttributes(filename, FileAttributes.ReadOnly);
                    }
                    catch
                    {
                        regResult.Text = "Цей флеш токен вже зайнятий";
                    }
                    finally
                    {
                        MessageBox.Show("Успіх");
                    }
                }
            }
            else MessageBox.Show("Вкажіть ім'я або виберіть диск");


        }

        private void refreshDisks()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            var drives = DriveInfo.GetDrives();
            foreach (var d in drives)
            {
                if (d.DriveType == DriveType.Removable)
                {
                    listBox1.Items.Add(d.Name);
                    listBox2.Items.Add(d.Name);
                }
            }
            login.Enabled = listBox2.Items.Count > 0;
            register.Enabled = listBox2.Items.Count > 0;
        }
        string selectedDisk = "";
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDisk = listBox2.SelectedItem.ToString();
        }

        string selectedDiskForLogin = "";
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedDiskForLogin = listBox1.SelectedItem.ToString();
        }
    }
}

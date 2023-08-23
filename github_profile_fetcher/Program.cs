using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Windows.Forms;
using System.Security.Policy;
using Microsoft.VisualBasic.ApplicationServices;

namespace My
{
    static class Program
    {
        /*[System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole(); // Import this function Logger Console*/

        [STAThread]
        static void Main()
        {
            // AllocConsole();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        
        // Public textbox
        private string url = "https://api.github.com/users/";
        string[] order = {"User ID : ","ID : ","Name : ","Type : ","Location : ","Email : ","Bio : ","Followers : ","Following : "};
        TextBox user = new TextBox();
        Label login = new Label();
        Label id = new Label();
        Label name = new Label();
        Label type = new Label();
        Label location = new Label();
        Label email = new Label();
        Label bio = new Label();
        Label followers = new Label();
        Label following = new Label();
        Label[] Textu = new Label[9];

        private HttpClient cli;


        public MainForm()
        {
            Text = "Github Profile Fetcher";
            Size = new System.Drawing.Size(800,550);
            // https://api.github.com/users/mohan-raj
            // Usrname
            cli = new HttpClient();
            // Creating Text Boxes
            Textu[0] = login;
            Textu[1] = id;
            Textu[2] = name;
            Textu[3] = type;
            Textu[4] = location;
            Textu[5] = email;
            Textu[6] = bio;
            Textu[7] = followers;
            Textu[8] = following;
            int y = 20;

            for (int i = 0; i < 9; i++)
            {
                Textu[i].Location = new Point(20, y);
                Textu[i].Size = new Size(560, 30);
                Textu[i].Text = order[i];
                y += 40;
            }


            user.Size = new Size(250, 50);
            user.Location = new Point(20, y);
            user.LostFocus += lost_focus;
            y += 50;
            // Button SIgn ih
            Button btnPrintMessage = new Button();
            btnPrintMessage.Location = new Point(70, y);
            y += 40;
            btnPrintMessage.Text = "Search";
            btnPrintMessage.Click += BtnPrintMessage_Click;
            btnPrintMessage.Size = new Size(140,40);
            Controls.Add(user);
            for (int i = 0; i < 9; i++)
            {
                Controls.Add(Textu[i]);
            }

            Controls.Add(btnPrintMessage);

        }


        public void lost_focus(object sender,EventArgs e){
            if(user.Text==""){
                user.Text="Enter the User to Search";
            }
        }
        private async void BtnPrintMessage_Click(object sender, EventArgs e)
        {
            try
            {
                GitSchema data = await Fetcher(url+user.Text);
                
                Textu[0].Text = key_val(Textu[0].Text,data.login);
                Textu[1].Text = key_val(Textu[1].Text,Convert.ToString(data.id));
                Textu[2].Text = key_val(Textu[2].Text,data.name);
                Textu[3].Text = key_val(Textu[3].Text,data.type);
                Textu[4].Text = key_val(Textu[4].Text,data.location);
                Textu[5].Text = key_val(Textu[5].Text,data.email);
                Textu[6].Text = key_val(Textu[6].Text,data.bio);
                Textu[7].Text = key_val(Textu[7].Text,Convert.ToString(data.following));
                Textu[8].Text = key_val(Textu[8].Text,Convert.ToString(data.followers));
            }
            catch (JsonException ex)
            {
                MessageBox.Show("No Internet :(");
                Console.WriteLine("Error deserializing JSON: " + ex.Message);
            }

        }

        public async Task<GitSchema> Fetcher(string url)
        {
            string resp = await FetchDataFromApiAsync(url);
            return jsoni(resp);
        }
        public async Task<string> FetchDataFromApiAsync(string apiUrl)
        {
            try
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                requestMessage.Headers.Add("User-Agent", "Mozilla"); //need this Header to authenticate as Browser
                HttpResponseMessage response = await cli.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                Console.WriteLine("Respo : " + response.Content);
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                // Handle exceptions here
                Console.WriteLine("Error deserializing JSON: " + ex.Message);
                return "{\"error\": \"123\"}";
            }
        }

        public class GitSchema
        {
            public string message {get; set;}
            public string login { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string location { get; set; }
            public string email { get; set; }
            public string bio { get; set; }
            public int followers { get; set; }
            public int following { get; set; }
            public string error { get; set; }

        }
        public GitSchema jsoni(string resp)
        {
            GitSchema Gitdata = JsonSerializer.Deserialize<GitSchema>(resp);
            return Gitdata;
        }

        private string key_val(string text,string val){
            string[] st = text.Split(":");
            if(val==null){
                val="Not Found";
            }
            return st[0]+": "+val;
        }



    }
}

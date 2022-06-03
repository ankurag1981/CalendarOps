using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Graph;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Identity.Web;
using System.Security;
using SysIO=System.IO;

namespace CalendarOpsDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PopulateEvents(textBox1.Text);
        }

        async void PopulateEvents(string user)
        {
            try
            {
                ICalendarEventsCollectionPage events = await CalendarEventsHelper.UserCalendarEvents(user,int.Parse(textBox2.Text));
                dataGridView1.AutoGenerateColumns = true;
                List<UserEvent> uv = new List<UserEvent>();
                foreach (Event e in events)
                {
                    uv.Add(new UserEvent { Subject = e.Subject, Body = e.Body.Content, Start = e.Start.DateTime, End = e.End.DateTime });
                }
                dataGridView1.DataSource = uv;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    public struct UserEvent
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}

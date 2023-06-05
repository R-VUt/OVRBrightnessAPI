using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private HttpClient cl = new HttpClient();
        
        public Form1()
        {
            InitializeComponent();
            
        }
        private string EndPoint = "http://localhost:13902/";
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            string brightness = (trackBar1.Value/(float)100).ToString();
            try
            {
                cl.PostAsync(EndPoint + "brightness/set", new StringContent(brightness));
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFirewallHelper;

namespace SimpleInternetConnectionCheck
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        bool allow = true;
        private void refreshStatus()
        {
            if (allow == true)
            {
                Console.WriteLine("Checking internet connection...");
                
                allow = false;
                try
                {
                    using (var client = new System.Net.WebClient())
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        label2.ForeColor = Color.Green;
                        Console.WriteLine("Internet Connection Test Successful");
                    }
                }
                catch
                {
                    label2.ForeColor = Color.Red;
                    Console.WriteLine("Internet Connection Test Failed");
                }
                allow = true;
            }
        }

        private static void createFirewallRule()
        {
            var ruleOut = FirewallManager.Instance.CreateApplicationRule(
        FirewallManager.Instance.GetProfile().Type, "InternetChecker",
        FirewallAction.Allow, Application.ExecutablePath);
            ruleOut.Direction = FirewallDirection.Outbound;

            var ruleIn = FirewallManager.Instance.CreateApplicationRule(
        FirewallManager.Instance.GetProfile().Type, "InternetChecker",
        FirewallAction.Allow, Application.ExecutablePath);
            ruleIn.Direction = FirewallDirection.Inbound;

            FirewallManager.Instance.Rules.Add(ruleOut);
            FirewallManager.Instance.Rules.Add(ruleIn);
        }

        private void buttonRefresh(object sender, EventArgs e)
        {
            Thread thread = new Thread(refreshStatus)
            {
                IsBackground = true
            };
            thread.Start();
        }

        public MainForm()
        {
            InitializeComponent();
            var allRules = FirewallManager.Instance.Rules.ToArray();
            bool exists = false;
            foreach (var s in allRules)
            {
                if (s.ToString() == "InternetChecker")
                {
                    exists = true;
                }
            }
            if (exists == false)
            {
                createFirewallRule();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            //refreshStatus();
        }
    }
}

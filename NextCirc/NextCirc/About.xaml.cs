using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace NextCirc.Images
{
    public partial class About : PhoneApplicationPage
    {
        public static double appVersion = 1.0;
        public bool inEmail = false;

        public About()
        {
            InitializeComponent();

            this.versiontext.Text = "Version " + appVersion.ToString("N1");
            this.abouttext.Text = "This app was created to make the Circulator schedule more accessible to students. It is in no way endorsed, " +
                "related to, or even acknowledged by Washington University.\n\n" +
                "If you have feedback, I'd love to hear it - complaints, feature requests, or anything else. Just hit that button down below " +
                "and drop me a line!";
        }

        public void SendEmail(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "NextCirc Feedback (Version " + appVersion + ")";
            emailComposeTask.Body = "";
            emailComposeTask.To = "jefftheman45@gmail.com";

            if (!inEmail)
            {
                inEmail = true;
                emailComposeTask.Show();
            }
        }

    }
}
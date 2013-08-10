using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnSendEmail_Click(object sender, EventArgs e)
    {
        EmailHelper emailHelper = new EmailHelper("test@email.com", "recepient@email.com", null, null, "This email was sent from my asp.net application", string.Format("Email was sent on {0}<br/></b>This is bold text.</b><br/><I>This is italic</i>", DateTime.Now), true, System.Net.Mail.MailPriority.High);
        emailHelper.SendMailMessage();

        Response.Write("Email sent successfully.");
    }
}
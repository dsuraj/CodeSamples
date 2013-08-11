using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CryptSharp;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnEncryptPassword_Click(object sender, EventArgs e)
    {
        string cryptedPassword = Crypter.Blowfish.Crypt(txtPassword.Text.Trim());

        Response.Write(string.Format("Password encrypted successfully. Encrypted password is: {0}", cryptedPassword));
    }
}
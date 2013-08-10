using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// A helper class to send an email
/// </summary>
public class EmailHelper
{
    #region "Properties"

    public string TO { get; set; }

    public string FROM { get; set; }

    public string BCC { get; set; }

    public string CC { get; set; }

    public string Subject { get; set; }

    public string MailBody { get; set; }

    public bool IsBodyHtml { get; set; }

    public MailPriority? EMailPriority { get; set; }

    #endregion "Properties"

    #region "Constructor"

    public EmailHelper()
    {
        //
        // Default Constructor
        //
    }

    /// <summary>
    /// Simple constructor for EmailHelper class
    /// </summary>
    /// <param name="from">A sender address</param>
    /// <param name="to">A recepient address</param>
    /// <param name="bcc">A bcc recepient</param>
    /// <param name="cc">A cc recepient</param>
    /// <param name="subject">Subject of mail message</param>
    /// <param name="mailBody">Body of mail message</param>
    /// <param name="isBodyHtml"></param>
    /// <param name="mailPriority"></param>
    public EmailHelper(string from, string to, string bcc, string cc, string subject, string mailBody, bool isBodyHtml, MailPriority mailPriority)
    {
        this.FROM = from;
        this.TO = to;
        this.BCC = bcc;
        this.CC = cc;
        this.Subject = subject;
        this.MailBody = mailBody;
        this.IsBodyHtml = isBodyHtml;
        this.EMailPriority = mailPriority;
    }

    #endregion "Constructor"

    /// <summary>
    /// Send the Email, if you use parameterised constructor
    /// </summary>
    public void SendMailMessage()
    {
        if (!string.IsNullOrEmpty(FROM) && !string.IsNullOrEmpty(TO) && IsValidEmailAdress(FROM) && IsValidEmailAdress(TO))
        {
            // a new instance of MailMessage
            MailMessage mailMessage = new MailMessage();

            // Set the sender address of the mail message
            mailMessage.From = new MailAddress(FROM);


            // Set the recepient address of the mail message
            mailMessage.To.Add(new MailAddress(TO));

            // Check if the bcc value is null or an empty string
            if (!string.IsNullOrEmpty(BCC) && IsValidEmailAdress(BCC))
            {
                // Set the Bcc address of the mail message
                mailMessage.Bcc.Add(new MailAddress(BCC));
            }

            // Check if the bcc value is null or an empty string
            if (!string.IsNullOrEmpty(CC) && IsValidEmailAdress(CC))
            {
                // Set the cc address of the mail message
                mailMessage.Bcc.Add(new MailAddress(CC));
            }

            // Set the subject of the mail message
            mailMessage.Subject = Subject;

            // Set the body of the mail message
            mailMessage.Body = MailBody;

            // Set the format of the mail message body as HTML
            mailMessage.IsBodyHtml = IsBodyHtml;

            // Set the priority of the mail message to normal if not set
            mailMessage.Priority = EMailPriority.HasValue ? EMailPriority.Value : MailPriority.Normal;

            // Instantiate a new instance of SmtpClient
            SmtpClient smtpClient = new SmtpClient();

            // Send the mail
            smtpClient.Send(mailMessage);
        }
    }

    /// <summary>
    ///  Sends a mail message
    /// </summary>
    /// <param name="from">Sender address</param>
    /// <param name="to">Recepient address list</param>
    /// <param name="bcc">Bcc recepient list</param>
    /// <param name="cc">Cc recepient list</param>
    /// <param name="subject">Subject of mail message</param>
    /// <param name="mailBody">Body of mail message</param>
    /// <param name="isBodyHtml">If set to true, Email message will send as HTML</param>
    /// <param name="mailPriority">Sets the priority of the email</param>
    public void SendMailMessage(string from, List<string> to, List<string> bcc, List<string> cc, string subject, string mailBody, bool isBodyHtml, MailPriority mailPriority)
    {
        // a new instance of MailMessage
        MailMessage mailMessage = new MailMessage();

        // Set the sender address of the mail message
        mailMessage.From = new MailAddress(from);

        foreach (string toPerson in to)
        {
            if (!string.IsNullOrEmpty(toPerson) && IsValidEmailAdress(toPerson))
            {
                // Set the recepient address of the mail message
                mailMessage.To.Add(new MailAddress(toPerson));
            }
        }

        foreach (string toBcc in bcc)
        {
            // Check if the bcc value is null or an empty string
            if (!string.IsNullOrEmpty(toBcc) && IsValidEmailAdress(toBcc))
            {
                // Set the Bcc address of the mail message
                mailMessage.Bcc.Add(new MailAddress(toBcc));
            }
        }

        foreach (string toCc in cc)
        {
            // Check if the cc value is null or an empty value
            if (!string.IsNullOrEmpty(toCc) && IsValidEmailAdress(toCc))
            {
                // Set the CC address of the mail message
                mailMessage.CC.Add(new MailAddress(toCc));
            }
        }

        // Set the subject of the mail message
        mailMessage.Subject = subject;

        // Set the body of the mail message
        mailMessage.Body = mailBody;

        // Set the format of the mail message body as HTML
        mailMessage.IsBodyHtml = isBodyHtml;

        // Set the priority of the mail message
        mailMessage.Priority = mailPriority;

        // Instantiate a new instance of SmtpClient
        SmtpClient smtpClient = new SmtpClient();

        // Send the mail
        smtpClient.Send(mailMessage);
    }

    /// <summary>
    /// Validation for email address 
    /// </summary>
    /// <param name="emailAddress">Email address to validate</param>
    /// <returns>True, if email address is valid else returns false</returns>
    private bool IsValidEmailAdress(string emailAddress)
    {
        if (!string.IsNullOrEmpty(emailAddress))
        {
            var regularExpression = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regularExpression.IsMatch(emailAddress) ? true : false;
        }
        return false;
    }
}

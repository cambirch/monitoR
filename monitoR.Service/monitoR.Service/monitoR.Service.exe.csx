
using System.Net.Mail;
using System.IO;

// Initialize a SMTP client with the setup we want to use
//  Fully configurable in any way that .NET will allow
var smtpClient = new SmtpClient {
	Host = "smtp.gmail.com",
	Port = 587,
	EnableSsl = true,
	UseDefaultCredentials = false,
	Credentials = new System.Net.NetworkCredential("cam.birch@gmail.com", "cxYKuLiGuFfeoMDJD")
};

// Add the actual configuration setting so it can be used in the application itself
Add("smtp", smtpClient);
Add("fromEmail", "cam.birch@gmail.com");
Add("toEmail", "cam.birch@gmail.com");


// Templates
Add("template-stateChange", File.ReadAllText("stateChangeEmail.cshtml"));
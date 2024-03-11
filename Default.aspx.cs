using System;
using System.Net.Mail;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDFGeneration
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_Email_Click1(object sender, EventArgs e)
        {
            string senderEmail = "k.vaishnavi.one@outlook.com";
            //string senderPassword = "Vk1@Outlook";

            string recipientEmail = "sushilpatil@outlook.com";

            //string smtpServer = "smtp-mail.outlook.com";

            //string hostName = Dns.GetHostName();
            //string ipAddresses = Dns.GetHostAddresses(hostName).ToString();

            string ipAddress = "192.168.235.92";

            int smtpPort = 25;
            //bool enableSsl = true;

            using (var smtpClient = new SmtpClient(ipAddress))
            {
                smtpClient.Credentials = new NetworkCredential();
                smtpClient.Port = smtpPort;
                //smtpClient.EnableSsl = enableSsl;

                //smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
                //smtpClient.UseDefaultCredentials = true;

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                var mailMessage = new System.Net.Mail.MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "Test Email",
                    Body = "This is a test email sent from a .NET Core console app using SMTP.",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(new MailAddress(recipientEmail));

                try
                {
                    smtpClient.Send(mailMessage);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }
        }

        protected async void btn_Email_ClickAsync(object sender, EventArgs e)
        {
            string clientId = "afb887bf-1db3-48c2-9fba-f6e7815dcce1";
            string clientSecret = "Vul8Q~HfQmlmYMVKQWRsMxTuW31rkMAeMhsfEaxC";
            string tenantId = "f8cdef31-a31e-4b4a-93e4-5f571e91255a";

            string userEmail = "k.vaishnavi.one@outlook.com";

            string accessToken = await GetAccessToken(clientId, clientSecret, tenantId);

            if (!string.IsNullOrEmpty(accessToken))
            {
                string endpoint = $"https://graph.microsoft.com/v1.0/users/{userEmail}/sendMail";

                string jsonBody = @"{
                ""message"": {
                    ""subject"": ""Test Email"",
                    ""body"": {
                        ""content"": ""This is a test email sent using Microsoft Graph API."",
                        ""contentType"": ""Text""
                    },
                    ""toRecipients"": [
                        {
                            ""emailAddress"": {
                                ""address"": ""sushilpatil@outlook.com""
                            }
                        }
                    ]
                }
            }";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    try
                    {
                        HttpResponseMessage response = await client.PostAsync(endpoint, content);

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Email sent successfully!");
                        }
                        else
                        {
                            MessageBox.Show($"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed to obtain access token");
            }
        }

        static async Task<string> GetAccessToken(string clientId, string clientSecret, string tenantId)
        {
            string authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
            string scope = "https://graph.microsoft.com/.default";

            var confidentialClient = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri(authority))
                .Build();

            var result = await confidentialClient.AcquireTokenForClient(new[] { scope })
                .ExecuteAsync();

            return result.AccessToken;
        }
    }
}
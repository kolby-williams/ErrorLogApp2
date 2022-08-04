using System;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using Console = Colorful.Console;
using MimeKit;
using MailKit.Net.Smtp;
using System.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;

namespace ErrorLogApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Start();
        }

        static void Start()
        {
         int uploadError = 0;
         int unsuccessfulPing = 0;
         int invalidProNum = 0;
         List<String> errorLogList = new List<string>();

            StreamReader reader = File.OpenText("c:/fs3d1/FS_Tray_Application_Log.log");
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split('\n');

                foreach (string item in items)
                {
                    if (item.Contains("ERROR") && item.Contains(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")))
                    {
                        if (item.Contains(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")))
                            Console.WriteLine(item + "\n");
                            errorLogList.Add(item);

                        if (item.Contains("Upload Error"))
                            uploadError++;

                        if (item.Contains("Unsuccessfully Pinged"))
                            unsuccessfulPing++;
                    }
                    if (item.Contains(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")))
                        if (item.Contains("Please Enter Valid Pro Number"))
                            invalidProNum++;
                }
            }
            Console.WriteLine("|-------Error Counts For " + DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy") + "-------|");
            Console.WriteLine("Upload Errors = " + uploadError + "\n---------------------------------");
            Console.WriteLine("Unsuccessful Ping = " + unsuccessfulPing + "\n---------------------------------");
            Console.WriteLine("Invalid Pro Number = " + invalidProNum + "\n---------------------------------");
            Console.WriteAscii("FreightSnap", ColorTranslator.FromHtml("#50C8FF"));

            var subjectLineCompany = ConfigurationManager.AppSettings["emailSubjectLineCompany"];
            var subjectLineLocation = ConfigurationManager.AppSettings["emailSubjectLineLocation"];
            var subjectLineScanner = ConfigurationManager.AppSettings["emailSubjectLineScanner"];
            var customerEmailAddress = ConfigurationManager.AppSettings["emailAddress"];
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("FreightSnap", ""));
            mailMessage.To.Add(new MailboxAddress("Whom It May Concern", customerEmailAddress));
            mailMessage.Subject = subjectLineCompany + " | " + subjectLineLocation + " | " + subjectLineScanner;
            mailMessage.Body = new TextPart("plain")
            {
                Text = "--------- Error Log for " + DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy") + " ---------\n" + "Upload Errors = " + uploadError + "\nUnsuccessful Ping = " + unsuccessfulPing + "\nInvalid Pro Number = " + invalidProNum
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect("smtp.gmail.com", 465, true);
                smtpClient.Authenticate("Kolby.williams@freightsnap.com", "Kjw2002!");
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }

            var url = "https://api.freightsnap.com/LiquidityServices/addErrorLog.php";
            var parameters = $"?company_id={subjectLineCompany}&location_id={subjectLineLocation}&scanner_id={subjectLineScanner}&date={DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}&upload_errors={uploadError}&invalid_pro={invalidProNum}&unsuccessful_ping={unsuccessfulPing}&error_text={errorLogList}";
            var stringContent = new StringContent(parameters);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PostAsync(url,stringContent).Result;

           Console.WriteLine(response);



            //--------------------------------------------------------------------------------------------------------------------------

            while (true)
            {
                TimeSpan time = TimeSpan.FromDays(1);
                Task.Delay(time).Wait();
                Console.Clear();
                errorLogList.Clear();
                uploadError = 0;
                unsuccessfulPing = 0;
                invalidProNum = 0;


                StreamReader reader2 = File.OpenText("c:/fs3d1/FS_Tray_Application_Log.log");
                string line2;
                while ((line2 = reader2.ReadLine()) != null)
                {
                    string[] items2 = line2.Split('\n');

                    foreach (string item2 in items2)
                    {
                        if (item2.Contains("ERROR") && item2.Contains(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")))
                        {
                            if (item2.Contains(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")))
                                Console.WriteLine(item2 + "\n");
                                errorLogList.Add(item2);

                            if (item2.Contains("Upload Error"))
                                uploadError++;

                            if (item2.Contains("Unsuccessfully Pinged"))
                                unsuccessfulPing++;
                        }
                        if (item2.Contains(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")))
                            if (item2.Contains("Please Enter Valid Pro Number"))
                                invalidProNum++;
                    }

                }
                Console.WriteLine("|-------Error Counts For " + DateTime.Now.AddDays(-1).ToString("MM-dd-yyyy") + "-------|");
                Console.WriteLine("Upload Errors = " + uploadError + "\n---------------------------------");
                Console.WriteLine("Unsuccessful Ping = " + unsuccessfulPing + "\n---------------------------------");
                Console.WriteLine("Invalid Pro Number = " + invalidProNum + "\n---------------------------------");
                Console.WriteAscii("FreightSnap", ColorTranslator.FromHtml("#50C8FF"));

                var mailMessage2 = new MimeMessage();
                mailMessage2.From.Add(new MailboxAddress("FreightSnap", ""));
                mailMessage2.To.Add(new MailboxAddress("Whom It May Concern", customerEmailAddress));
                mailMessage.Subject = subjectLineCompany + " | " + subjectLineLocation + " | " + subjectLineScanner;
                mailMessage2.Body = new TextPart("plain")
                {
                    Text = "---------Error Log for " + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "---------\n" + "Upload Errors = " + uploadError + "\nUnsuccessful Ping = " + unsuccessfulPing + "\nInvalid Pro Number = " + invalidProNum
                };

                using (var smtpClient2 = new SmtpClient())
                {
                    smtpClient2.Connect("smtp.gmail.com", 587, true);
                    smtpClient2.Authenticate("user", "password");
                    smtpClient2.Send(mailMessage);
                    smtpClient2.Disconnect(true);
                }

                var url2 = "https://api.freightsnap.com/LiquidityServices/addErrorLog.php";
                var parameters2 = $"?company_id={subjectLineCompany}&location_id={subjectLineLocation}&scanner_id={subjectLineScanner}&date={DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")}&upload_errors={uploadError}&invalid_pro={invalidProNum}&unsuccessful_ping={unsuccessfulPing}&error_text={errorLogList}";
                var stringContent2 = new StringContent(parameters2);
                HttpClient client2 = new HttpClient();
                client2.BaseAddress = new Uri(url2);
                client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response2 = client.PostAsync(url2,stringContent2).Result;

                Console.WriteLine("Upload Successful");
            }
        }

    }
}

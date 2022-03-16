using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProductReading.Models;
using ProductReading.BL;
using System.IO;

namespace ProductReading.Helping_Classes
{
    public class MailSender
    {

        public static bool SendForgotPasswordEmail(string email, string encId, string BaseUrl = "")
        {
            try
            {
                string MailBody = "<html>" +
                    "<head></head>" +
                    "<body>" +
                    "<center>" + "<div> <h1 class='text-center' style='color:#000000'> Password Reset </h1> " +
                    "<p class='text-center' style='color:#000000'> " +
                          "You are Getting this Email Because You Requested To Reset Your Account Password.<br>Click the Button Below To Change Your Password" +
                    " </p>" +
                    "<p style='color:#000000' class='text-center'>" +
                            "If you did not request a password reset, Please Ignore This Email" +
                    "</p>" +
                    "<h3 style='color:#000000'>" + "Thanks" + "</h3>" +
                    "<br/>" +
                    "<button style='background-color: #CE2029; padding:12px 16px; border:1px solid #CE2029; border-radius:3px;'>" +
                            "<a href='" + BaseUrl + "Auth/ResetPassword?encId=" + encId + "&t=" + GeneralPurpose.DateTimeNow().Ticks + "' style='text-decoration:none; font-size:15px; color:white;'> Reset Password </a>" +
                    "</button>" +
                    "<p style='color:#FF0000'>Link will Expire after Date Change.<br>" +
                    "Link will not work in spam. Please move this mail into your inbox.</p>" +
                    "</div>" + "</center>" +
                            "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";


                RestClient client = new RestClient();
                client.BaseUrl = new Uri("https://api.mailgun.net/v3");
                client.Authenticator = new HttpBasicAuthenticator("api", "key-ff43f744db29071a74f3106541e3b925");

                RestRequest request = new RestRequest();
                request.AddParameter("domain", "nodlays.co.uk", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", ProjectVariable.FromEmail);
                request.AddParameter("to", email);
                request.AddParameter("subject", "Product | Password Reset");

                request.AddParameter("html", MailBody);

                request.Method = Method.POST;

                string response = client.Execute(request).Content.ToString();

                if (response.ToLower().Contains("queued"))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
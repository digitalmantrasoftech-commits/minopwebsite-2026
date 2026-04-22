using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Web;
using System.Configuration;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;

namespace PayTimeWebClient.Helper
{
    public static class ErrorHandler
    {

        private static readonly string LogDirectoryPath = HttpContext.Current.Server.MapPath("~/App_Data/ExceptionLog/");
        private const long MaxFileSizeInBytes = 2 * 1024 * 1024; // 2 MB

        public static class ErrorCodes
        {
           

            public const string UNAUTHORIZED_ACCESS_SENDING_ERROR = "UA001";

            // Web-related errors
            public const string JSON_DESERIALIZATION_ERROR = "JDE001"; // Error during JSON deserialization
            public const string API_RESPONSE_ERROR = "ARE001"; // API returned an unexpected response
            public const string HTTP_REQUEST_SENDING_ERROR = "HRSE001"; // Error while sending HTTP request
            public const string HTTP_BAD_REQUEST = "BRE001"; // For Bad request
            public const string HTTP_TOO_MANY_REQUEST = "TMR001"; //For too many request occurred

            // Database-related errors
            public const string DB_CONNECTION_ERROR = "CE001"; // Database connection failure
            public const string DB_QUERY_EXECUTION_ERROR = "QE001"; // Error during query execution
            public const string DB_TRANSACTION_ERROR = "TE003"; // Error during database transaction

            // Application-level errors
            public const string UNEXPECTED_ERROR = "UE001"; // Unexpected or generic application error
            public const string DATA_VALIDATION_ERROR = "DVE002"; // Validation failure for input data
            public const string CONFIGURATION_ERROR = "COE003"; // Configuration issue in the application

            // Authentication and authorization errors
            public const string UNAUTHORIZED_ACCESS_ERROR = "UAE001"; // Unauthorized access attempt
            public const string TOKEN_EXPIRED_ERROR = "TEE001"; // Authentication token expired
            public const string PERMISSION_DENIED_ERROR = "PDE001"; // User lacks the necessary permissions

            // Email-related errors
            public const string EMAIL_SENDING_ERROR = "ESE001"; // Error while sending email
            public const string EMAIL_TEMPLATE_ERROR = "ETE002"; // Issue with the email template

            // File-related errors
            public const string FILE_UPLOAD_ERROR = "FUE001"; // Error during file upload
            public const string FILE_DOWNLOAD_ERROR = "FDU002"; // Error during file download
            public const string FILE_PROCESSING_ERROR = "FPU003"; // Error processing the file

            // Logging-related errors
            public const string LOGGING_ERROR = "LOG001"; // Error while logging data


        }
        // Log the error details into a file
        public static void LogError(string errorCode, string errorType, Exception ex, [CallerMemberName] string methodName = "", string DbName = "", string URL = "", string IPAddress = "")
        {
            try
            {
                // Ensure the log directory exists
                if (!Directory.Exists(LogDirectoryPath))
                {
                    Directory.CreateDirectory(LogDirectoryPath);
                }

                // Determine the log file path with today's date
                string logFileName = $"ExeErrorLog_{DateTime.Now:yyyy-MM-dd}.txt";
                string logFilePath = Path.Combine(LogDirectoryPath, logFileName);

                // Check if the current file exceeds the maximum file size
                if (File.Exists(logFilePath) && new FileInfo(logFilePath).Length > MaxFileSizeInBytes)
                {
                    // Create a new file with a timestamp if the size exceeds the limit
                    string newFileName = $"ExeErrorLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
                    logFilePath = Path.Combine(LogDirectoryPath, newFileName);
                }

                // Prepare the log message
                //string logMessage = $"[{DateTime.Now}] Error Code: {errorCode} | " +
                //                    $"Error Type: {errorType} | Method: {methodName} | " +
                //                    $"Message: {ex.Message}\n{ex.StackTrace}\n";

                string logMessage = $@" Error occurred at: {DateTime.Now:yyyy-MM-dd hh:mm tt}
                                        Method: {methodName}
                                        Database: {DbName}
                                        URI :{URL}                                       
                                        Error Code: {errorCode} 
                                        Subject: {errorType}
                                        Message: {ex.Message} 
                                        RequestIPAddress: {IPAddress} 
                                        Stack Trace:{ex.StackTrace}";

                // Append the log message to the file
                File.AppendAllText(logFilePath, logMessage);
            }
            catch (Exception loggingEx)
            {
                // Handle logging failures gracefully
                // You could add an emergency fallback logging mechanism here if needed
                Console.WriteLine($"Failed to log error: {loggingEx.Message}");
            }
        }

        // Send an error notification email
        public static void SendErrorEmail(string errorCode, string errorType, Exception ex, [CallerMemberName] string methodName = "", string DbName = "", string URL = "", string IPAddress = "")
        {
            SmtpClient smtpClient = new SmtpClient();
            MailMessage message = new MailMessage();

            try
            {
                // Fetch email settings from web.config
                string fromEmail = ConfigurationManager.AppSettings["ExceptionFromEmailZoho"];
                string fromPassword = ConfigurationManager.AppSettings["ExceptionFromPasswordZoho"];
                string smtpHost = ConfigurationManager.AppSettings["ExceptionHostZoho"];
                int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["ExceptionPortZoho"]);
                string toEmail = ConfigurationManager.AppSettings["ExceptionToMailZoho"];
                string ccEmail = ConfigurationManager.AppSettings["ExceptionToCCMailZoho"];
                string bccEmail = ConfigurationManager.AppSettings["EmailBCCZoho"];
                bool _enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSLZoho"]);
                bool _enableMailFlageZoho = Convert.ToBoolean(ConfigurationManager.AppSettings["enableMailFlageZoho"]);

                // Construct the email body         
                //$"<p><b>Stack Trace:</b><br>{ex.StackTrace.Replace("\n", "<br>")}</p>";

                if (_enableMailFlageZoho == true)
                {
                    string body = $@"
                    <p><b>Error occurred at:</b> {DateTime.Now:yyyy-MM-dd hh:mm tt}</p>
                    <p><b>Method:  </b> {methodName}</p>  
                    <p><b>Database: </b> {DbName}</p>
                    <p><b>URI: </b> {URL}</p>
                    <p><b>Error Code: </b> {errorCode.Replace("\n", "<br>")}</p>
                    <p><b>Error Type: </b> {errorType}</p>
                    <p><b>Message: </b> {ex.Message}</p>
                    <p><b>IPAddress: </b> {IPAddress}</p>
                    <p><b>Stack Trace:</b><br>{ex.StackTrace.Replace("\n", "<br>")}</p>";

                    // Configure the MailMessage object
                    message.From = new MailAddress(fromEmail, "Error Notification");
                    message.To.Add(toEmail);

                    if (!string.IsNullOrEmpty(ccEmail))
                    {
                        message.CC.Add(ccEmail);
                    }
                    if (!string.IsNullOrEmpty(bccEmail))
                    {
                        message.Bcc.Add(bccEmail);
                    }
                    //message.Subject = $"Error Notification:{methodName}-{errorCode} - {errorType}";
                    message.Subject = $"Error Notification:{methodName}";
                    message.Body = body;
                    message.IsBodyHtml = true;
                    smtpClient.Port = smtpPort;
                    smtpClient.Host = smtpHost;
                    // Configure the SMTP client
                    smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                    smtpClient.EnableSsl = _enableSSL;
                    // Send the email
                    smtpClient.Send(message);
                }
            }
            catch (Exception emailEx)
            {
                LogError("EMAIL_SENDING_ERROR", " Email Sending Error", emailEx, "SendErrorEmail", URL);
            }
            finally
            {
                // Dispose of resources
                message.Dispose();
                smtpClient.Dispose();
            }
        }
        //Centralized method to handle errors
        public static void HandleException(string errorCode, string errorType, Exception ex, [CallerMemberName] string methodName = "", string DbName = "", string URL = "")
        {
            string clientIp = GetClientIpAddress();
            LogError(errorCode, errorType, ex, methodName, DbName, URL, clientIp);
            SendErrorEmail(errorCode, errorType, ex, methodName, DbName, URL, clientIp);
        }
        public static string GetClientIpAddress()
        {
            try
            {
                var context = HttpContext.Current;

                if (context == null || context.Request == null)
                    return "IP Not Available";

                var request = context.Request;

                string ipAddress = null;

                // 1️⃣ Check X-Forwarded-For (Proxy / Load Balancer)
                string forwardedFor = request.Headers["X-Forwarded-For"];

                if (!string.IsNullOrWhiteSpace(forwardedFor))
                {
                    // Multiple IPs possible: client, proxy1, proxy2
                    string[] addresses = forwardedFor.Split(',');

                    if (addresses.Length > 0)
                    {
                        ipAddress = addresses[0].Trim(); // First one = real client
                    }
                }

                // 2️⃣ If not found, use UserHostAddress
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    ipAddress = request.UserHostAddress;
                }

                // 3️⃣ Final validation
                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                    return "IP Not Available";
                }

                return ipAddress;
            }
            catch (Exception ex)
            {                
                return "IP Not Available";
            }
        }

        //Dynamic handler based on exception type
        //public static void HandleException(Exception ex, string methodName, string dbName, string uri, string customMessage = "An error occurred.")
        //{

        //    string errorCode = ErrorHandler.ErrorCodes.UNEXPECTED_ERROR;
        //    // Determine the error code based on exception type
        //    // Determine the error code based on the exception type
        //    if (ex is UnauthorizedAccessException)
        //    {
        //        errorCode = ErrorHandler.ErrorCodes.UNAUTHORIZED_ACCESS_ERROR;
        //        customMessage = "Unauthorized access detected.";
        //    }
        //    else if (ex is HttpRequestException)
        //    {
        //        errorCode = ErrorHandler.ErrorCodes.HTTP_REQUEST_SENDING_ERROR;
        //        customMessage = "HTTP request failed.";
        //    }
        //    else
        //    {
        //        errorCode = ErrorHandler.ErrorCodes.UNEXPECTED_ERROR;
        //        customMessage = "An unexpected error occurred.";
        //    }

        //    // Delegate to the primary handler
        //    HandleException(
        //        errorCode.ToString(),
        //        customMessage,
        //        ex,
        //        methodName,
        //        dbName,
        //        uri
        //    );
        //}
        public static void HandleException(Exception ex, string methodName, string dbName, string uri, string customMessage = "An error occurred.")
        {
            string errorCode = ErrorCodes.UNEXPECTED_ERROR;

            // Check if exception is an HttpRequestException and extract status code
            if (ex is HttpRequestException httpEx)
            {
                string errorMessage = httpEx.Message;

                if (errorMessage.Contains("401"))
                {
                    errorCode = ErrorCodes.UNAUTHORIZED_ACCESS_ERROR;
                    customMessage = "Unauthorized access detected.";
                }
                else if (errorMessage.Contains("403"))
                {
                    errorCode = ErrorCodes.PERMISSION_DENIED_ERROR;
                    customMessage = "You do not have permission to access this resource.";
                }
                else if (errorMessage.Contains("404"))
                {
                    errorCode = ErrorCodes.API_RESPONSE_ERROR;
                    customMessage = "HTTP request failed.";
                }
                else if (errorMessage.Contains("500"))
                {
                    errorCode =ErrorCodes.HTTP_REQUEST_SENDING_ERROR;
                    customMessage = "A problem occurred on the server.";
                }
                else if (errorMessage.Contains("408"))
                {
                    errorCode = ErrorCodes.HTTP_REQUEST_SENDING_ERROR;
                    customMessage = "Request Time Out Occured.";
                }
                else if (errorMessage.Contains("414") || errorMessage.Contains("411") || errorMessage.Contains("413") || errorMessage.Contains("416"))
                {
                    errorCode = ErrorCodes.JSON_DESERIALIZATION_ERROR;
                    customMessage = "A Url Too Long.";
                }
                else if (errorMessage.Contains("400"))
                {
                    errorCode = ErrorCodes.HTTP_BAD_REQUEST;
                    customMessage = "A Bad Request Happened.";
                }
                else if (errorMessage.Contains("429"))
                {
                    errorCode = ErrorCodes.HTTP_TOO_MANY_REQUEST;
                    customMessage = "Too Many Request Occured On Server.";
                }
                else
                {
                    errorCode = ErrorCodes.HTTP_REQUEST_SENDING_ERROR;
                    customMessage = $"HTTP Error: {errorMessage}";
                }
            }
            else if (ex is UnauthorizedAccessException)
            {
                errorCode =ErrorCodes.UNAUTHORIZED_ACCESS_ERROR;
                customMessage = "Unauthorized access detected.";
            }
            else
            {
                errorCode = ErrorCodes.UNEXPECTED_ERROR;
                customMessage = $"Unexpected error: {ex.Message}";
            }

            // Delegate to the primary handler
            HandleException(
                errorCode.ToString(),
                customMessage,
                ex,
                methodName,
                dbName,
                uri
            );
        }

        public class ExceptionResponse
        {
            public string Message { get; set; }
            public string ErrorCode { get; set; }
            public string MethodName { get; set; }
            public string DbName { get; set; }
            public string RequestUri { get; set; }
        }


      
    }

    
    
}
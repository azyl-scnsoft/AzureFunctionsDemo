#r "Twilio.Api"
#load "..\OCRShared\TaskDetails.csx"
#load "..\OCRShared\TaskStatus.csx"

using System;
using System.Threading.Tasks;
using Twilio;

public static void Run(TaskDetails task, out SMSMessage message, TraceWriter log)
{
    string body = task.GetStatus() == TaskStatus.Completed
        ? string.Format("Text recognition for image {0} has finished successfully.", task.ImageFileName)
        : string.Format("Text recognition for image {0} has failed.", task.ImageFileName);
        
    message = string.IsNullOrWhiteSpace(task.UserPhone)
    ? null
    : new SMSMessage
    {
        To = task.UserPhone,
        Body = body
    };
}
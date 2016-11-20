#r "Twilio.Api"
#load "..\OCRShared\TaskDetails.csx"
#load "..\OCRShared\TaskStatus.csx"

using System;
using Twilio;

public static SMSMessage Run(TaskDetails task, TraceWriter log)
{
    string body = task.GetStatus() == TaskStatus.Completed
        ? string.Format("Text recognition for image {0} has finished successfully.", task.ImageFileName)
        : string.Format("Text recognition for image {0} has failed.", task.ImageFileName);
        
    return string.IsNullOrWhiteSpace(task.UserPhone)
    ? null
    : new SMSMessage
    {
        To = task.UserPhone,
        Body = body
    };
}
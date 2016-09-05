#load "..\OCRShared\TaskDetails.csx"
#load "..\OCRShared\TaskStatus.csx"
#r "SendGridMail"

using System;
using SendGrid;
using Microsoft.WindowsAzure.Storage.Blob;

public static void Run(TaskDetails task, out SendGridMessage message, TraceWriter log)
{
    if(string.IsNullOrWhiteSpace(task.UserEmail))
    {
        message = null;
        return;
    }
    
    string subject = "";
    string text = "";

    var taskStatus = task.GetStatus();
    if (taskStatus == TaskStatus.Completed)
    {
        subject = string.Format("Text recognition for image {0} has finished!", task.ImageFileName);
        text = string.Format("Text recognition for your image {0} has finished successfully! Please visit the next link to get the results: {1}", task.ImageFileName, task.ResultUri);
    }
    else if (taskStatus == TaskStatus.Failed)
    {
        subject = string.Format("Text recognition for image {0} has failed!", task.ImageFileName);
        text = string.Format("Sorry, we could not process your image {0}. Please, try again later.", task.ImageFileName);
    }
    else
    {
        message = null;
        return;
    }
    message = new SendGridMessage()
    {
        Subject = subject,
        Text = text
    };
    message.AddTo(task.UserEmail);
}
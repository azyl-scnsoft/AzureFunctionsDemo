#load "..\OCRShared\TaskDetails.csx"
#load "..\OCRShared\TaskStatus.csx"
#r "SendGrid"

using System;
using SendGrid.Helpers.Mail;

public static Mail Run(TaskDetails task, TraceWriter log)
{
    if(string.IsNullOrWhiteSpace(task.UserEmail))
    {
        return null;
    }

    var message = new Mail();
    Content content = new Content
    {
        Type = "text/plain"
    };

    string subject = "";
    string text = "";

    var taskStatus = task.GetStatus();
    if (taskStatus == TaskStatus.Completed)
    {
        message.Subject = string.Format("Text recognition for image {0} has finished!", task.ImageFileName);
        content.Value = string.Format("Text recognition for your image {0} has finished successfully! Please visit the next link to get the results: {1}", task.ImageFileName, task.ResultUri);
    }
    else if (taskStatus == TaskStatus.Failed)
    {
        message.Subject = string.Format("Text recognition for image {0} has failed!", task.ImageFileName);
        content.Value = string.Format("Sorry, we could not process your image {0}. Please, try again later.", task.ImageFileName);
    }
    else
    {
        return null;
    }

    var personalization = new Personalization();
    personalization.AddTo(new Email(task.UserEmail));
    message.AddPersonalization(personalization);
    message.AddContent(content);

    return message;
}
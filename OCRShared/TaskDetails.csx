#r "Microsoft.WindowsAzure.Storage"
#load "TaskStatus.csx"

using System;
using Microsoft.WindowsAzure.Storage.Table;

public class TaskDetails : TableEntity
{
    public TaskDetails()
    {
        Status = TaskStatus.Pending.ToString();
    }

    public TaskDetails(string id) : base(id, id)
    {
        Id = id;
        Status = TaskStatus.Pending.ToString();
    }

    public string Id { get; set; }
    public string Status { get; set; }
    public string UserEmail { get; set; }
    public string UserPhone { get; set; }
    public string ImageFileName { get; set; }
    public DateTimeOffset CreateTime { get; set; }
    public DateTimeOffset UpdateTime { get; set; }
    public string ResultUri { get; set; }

    public TaskStatus GetStatus()
    {
        return (TaskStatus)Enum.Parse(typeof(TaskStatus), Status);
    }
}
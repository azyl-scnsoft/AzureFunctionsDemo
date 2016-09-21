#r "Microsoft.WindowsAzure.Storage"
#load "..\OCRShared\TaskDetails.csx"

using System;
using System.Net;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage.Table;

public static HttpResponseMessage Run(TaskId taskId,  HttpRequestMessage req, TaskDetails task, TraceWriter log)
{
    return task == null
        ? req.CreateResponse(HttpStatusCode.NotFound)
        : req.CreateResponse(HttpStatusCode.OK, new TaskInfo {
            Id = task.Id,
            Status = task.Status,
            ResultUri = task.ResultUri
        });
}

public class TaskId
{
    public string Id { get; set; }
}

public class TaskInfo
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string ResultUri { get; set; }
}
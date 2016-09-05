#load "..\OCRShared\TaskDetails.csx"
#load "..\OCRShared\TaskStatus.csx"

#r "Microsoft.WindowsAzure.Storage"

using System;
using Microsoft.WindowsAzure.Storage.Table;

public static void Run(TaskDetails task, CloudTable taskTable, out TaskDetails taskMessage, TraceWriter log)
{
    task.Status = TaskStatus.Failed.ToString();
    task.UpdateTime = DateTimeOffset.UtcNow;

    taskMessage = task;

    task.ETag = "*";
    var operation = TableOperation.Replace(task);
    taskTable.Execute(operation);
}
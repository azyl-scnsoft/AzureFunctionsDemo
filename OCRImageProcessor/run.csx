#load "..\OCRShared\TaskDetails.csx"
#load "..\OCRShared\TaskStatus.csx"

#r "Microsoft.WindowsAzure.Storage"
#r "Newtonsoft.Json"

using System;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

public static async Task<TaskDetails> Run(
    TaskDetails task,
    Stream image,
    CloudTable tasksTable,
    ICloudBlob taskResult,
    TraceWriter log)
{
    var apiResult = await RecognizeAsync(image, task);

    if (apiResult.OCRExitCode == 3 || apiResult.OCRExitCode == 4)
    {
        task.Status = TaskStatus.Failed.ToString();
    }
    else
    {
        task.ResultUri = await WriteResultAsync(apiResult, taskResult);

        task.Status = TaskStatus.Completed.ToString();
    }

    task.ETag = "*";
    var operation = TableOperation.Replace(task);
    await tasksTable.ExecuteAsync(operation);

    return task;
}

private static async Task<Rootobject> RecognizeAsync(Stream image, TaskDetails task)
{
    var apiKey = System.Configuration.ConfigurationManager.AppSettings["OCR.Space.Api.Key"];

    var imageData = await GetImageBinariesAsync(image);

    using (var client = new HttpClient())
    {
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(apiKey), "apikey");
        form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", task.ImageFileName);

        var response = await client.PostAsync("https://api.ocr.space/Parse/Image", form);

        var strContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Rootobject>(strContent);
    }
}

private static async Task<byte[]> GetImageBinariesAsync(Stream image)
{
    byte[] imageData;
    using (MemoryStream ms = new MemoryStream())
    {
        await image.CopyToAsync(ms);
        imageData = ms.ToArray();
    }

    return imageData;
}

private static async Task<string> WriteResultAsync(Rootobject apiResult, ICloudBlob blob)
{
    using (var mem = new MemoryStream())
    {
        var sw = new StreamWriter(mem, new UnicodeEncoding());

        foreach (var parsedResult in apiResult.ParsedResults)
        {
            await sw.WriteAsync(parsedResult.ParsedText);
        }

        await sw.FlushAsync();
        mem.Seek(0, SeekOrigin.Begin);

        await blob.UploadFromStreamAsync(mem);

        sw.Dispose();
        
        var token = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy
        {
            Permissions = SharedAccessBlobPermissions.Read,
            SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddDays(7)
        });
        
        return blob.Uri.AbsoluteUri + token;
    }
}

public class Rootobject
{
    public Parsedresult[] ParsedResults { get; set; }
    public int OCRExitCode { get; set; }
    public bool IsErroredOnProcessing { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorDetails { get; set; }
}

public class Parsedresult
{
    public object FileParseExitCode { get; set; }
    public string ParsedText { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorDetails { get; set; }
}
#load "..\OCRShared\TaskDetails.csx"
#load "..\OCRShared\TaskStatus.csx"
#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Net;
using Microsoft.WindowsAzure.Storage.Blob;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req,
    IAsyncCollector<TaskDetails> tasks,
    CloudBlobContainer tasksBlobContainer,
    IAsyncCollector<TaskDetails> taskMessages,
    TraceWriter log)
{
    var image = await req.Content.ReadAsStreamAsync();
    var email = GetFromHeaders(req, "email");
    var fileName = GetFromHeaders(req, "filename");
    var phone = GetFromHeaders(req, "phone");

    if (image == null || image.Length == 0 || string.IsNullOrWhiteSpace(fileName))
    {
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }

    var id = Guid.NewGuid().ToString();

    var task = new TaskDetails(id)
    {
        ImageFileName = fileName,
        UserEmail = email.ToLowerInvariant(),
        UserPhone = phone,
        CreateTime = DateTimeOffset.UtcNow,
        UpdateTime = DateTimeOffset.UtcNow
    };

    await tasks.AddAsync(task);

    image.Position = 0;
    var blob = tasksBlobContainer.GetBlockBlobReference(id);
    await tasksBlobContainer.CreateIfNotExistsAsync();
    await blob.UploadFromStreamAsync(image);

    await taskMessages.AddAsync(task);

    return req.CreateResponse(HttpStatusCode.OK, new { Id = id });
}

private static string GetFromHeaders(HttpRequestMessage req, string key)
{
    IEnumerable<string> headers;
    return !req.Headers.TryGetValues(key, out headers) ? null : headers.FirstOrDefault();
}
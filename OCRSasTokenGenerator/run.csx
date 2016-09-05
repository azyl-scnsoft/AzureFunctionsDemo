#r "Microsoft.WindowsAzure.Storage"

using System;
using System.Net;
using Microsoft.WindowsAzure.Storage.Blob;

public static HttpResponseMessage Run(HttpRequestMessage req, CloudBlobContainer imageBlobContainer, TraceWriter log)
{
    var token = imageBlobContainer.GetSharedAccessSignature(new SharedAccessBlobPolicy
    {
        SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(30)
    });

    var response = req.CreateResponse(HttpStatusCode.OK, new
    {
        Token = token
    });
    
    return response;
}
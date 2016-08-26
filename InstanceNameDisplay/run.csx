using System;
using System.Threading;

public static async Task Run(string myQueueItem, TraceWriter log)
{
    log.Info($"C# manually triggered function called with input: {myQueueItem}");
	
	await Task.Delay(10000);
	
	log.Info(GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
}

public static string GetEnvironmentVariable(string name)
{
    return name + ": " + 
        System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}
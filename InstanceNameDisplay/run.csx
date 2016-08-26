using System;
using System.Threading;

public static void Run(string myQueueItem, TraceWriter log)
{
    log.Info($"C# manually triggered function called with input: {myQueueItem}");
	
	log.Info(GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
}

public static string GetEnvironmentVariable(string name)
{
    return name + ": " + 
        System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}
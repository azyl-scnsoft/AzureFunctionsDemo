using System;
using System.Threading;

public static void Run(string input, TraceWriter log)
{
    log.Info($"C# manually triggered function called with input: {input}");
	
	log.Info(GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));
	
	Thread.Sleep(Int.MaxValue);
}

public static string GetEnvironmentVariable(string name)
{
    return name + ": " + 
        System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}
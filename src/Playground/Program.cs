using System.Diagnostics;

foreach (var p in Process.GetProcesses()) {
    Console.WriteLine(p.ProcessName);
}
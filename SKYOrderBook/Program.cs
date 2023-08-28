using SKYOrderBook;
using System.Diagnostics;

try
{
    var filePath = InputManager.PromptForInputFilePath();
    var outputFilePath = InputManager.PromptForOutputFilePapth();
    var records = InputFileReader.GetAllCsvRecords(filePath);
    Stopwatch stopwatch = new Stopwatch();
    
    stopwatch.Start();

    var ticketBuilder = new TicketBuilder();
    var ticket = ticketBuilder.Build(records);

    stopwatch.Stop();

    var elapsed = stopwatch.Elapsed;

    Console.WriteLine($"Program executed in {elapsed.TotalSeconds:F2} {(elapsed.TotalSeconds > 1 ? "seconds" : "second")}.");

    OutputFileWriter.CreateFileFromTicket(ticket, outputFilePath);
}
catch (Exception ex)
{
    Console.WriteLine("An exception occurred: " + ex.Message);
}
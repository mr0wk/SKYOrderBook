using SKYOrderBook;

try
{
    var filePath = InputManager.PromptForFilePath();
    var records = InputFileReader.GetAllCsvRecords(filePath);
    var ticket = TicketBuilder.Build(records);

    OutputFileWriter.CreateFileFromTicket(ticket);
}
catch (Exception ex)
{
    Console.WriteLine("An exception occurred: " + ex.Message);
}
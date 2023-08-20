namespace SKYOrderBook
{
    public static class InputManager
    {
        public static string PromptForFilePath()
        {
            Console.WriteLine("Enter the path to the CSV file:");

            string filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist. Path is invalid.");

                return PromptForFilePath();
            }

            Console.WriteLine("File exists. Path is valid.");

            return filePath;
        }
    }
}

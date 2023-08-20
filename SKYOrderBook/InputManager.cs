namespace SKYOrderBook
{
    public static class InputManager
    {
        public static string PromptForInputFilePath()
        {
            Console.WriteLine("Enter the path to the CSV file:");

            string filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist. Path is invalid.");

                return PromptForInputFilePath();
            }

            Console.WriteLine("File exists. Path is valid.");

            return filePath;
        }

        public static string PromptForOutputFilePapth()
        {
            Console.WriteLine("Enter the path to the output CSV file:");

            return Console.ReadLine();
        }
    }
}

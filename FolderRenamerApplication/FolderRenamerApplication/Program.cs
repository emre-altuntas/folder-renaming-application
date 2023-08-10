using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json")
                        .Build();


Console.WriteLine("FOLDER RENAMING APPLICATION");
Console.WriteLine("Would you like to run the application? (Y/N)");

string? choice = Console.ReadLine();

int totalLines = 0;
int totalChangedLines = 0;

while (true)
{
    if (choice == "Y" || choice == "y")
    {
        // This is the path where the log file will be written. It is retrieved from the appsettings.json.
        StreamWriter sw = new StreamWriter($"{configuration.GetSection("Paths:LogFilePath").Value}");
        Console.SetOut(sw); 
      
        try
        {
            // Here, the path of the folders to be renamed is retrieved from the appsettings.json.
            string? folderPath = configuration.GetSection("Paths:FolderPath").Value;
            // Here, the .txt file containing the old names of folders and their corresponding new names is retrieved from the appsettings.json.
            string? textFilePath = configuration.GetSection("Paths:FolderRenamerTextFilePath").Value;

            // We are checking whether the file path is retrieved or not.
            if (folderPath == null || textFilePath == null)
            {
                Console.WriteLine($"{DateTime.UtcNow} \t ERROR: File path could not be obtained. Please check.");
            }

            else
            {
                // We are retrieving each line from the text file.
                string[] lines = File.ReadAllLines(textFilePath);

                foreach (string line in lines)
                {
                    totalLines++;

                    // We are separating the old and new names in each line."
                    string[] parts = line.Split('\t');

                    if (parts.Length == 2)
                    {
                        string oldFolderName = parts[0];
                        // During the renaming, if the new name of one folder already exists in folder that gives an error. We append an '_' character to the end. This will be removed later when it is read again. 
                        string newFolderName = parts[1] + "_";

                        string oldFolderPath = Path.Combine(folderPath, oldFolderName);
                        string newFolderPath = Path.Combine(folderPath, newFolderName);

                        if (Directory.Exists(oldFolderPath))
                        {
                            Directory.Move(oldFolderPath, newFolderPath);

                            totalChangedLines++;

                            Console.WriteLine($"{DateTime.UtcNow} \t INFO: {oldFolderName} has been renamed to {newFolderName.TrimEnd('_')}.");
                        }
                        else
                        {
                            Console.WriteLine($"{DateTime.UtcNow} \t INFO: No folder found with the name {oldFolderName}.");
                        }
                    }

                    // If the number of separated names in a line is greater than 2, it indicates a formatting issue (such as having only one name or more than two names).
                    else
                    {
                        Console.WriteLine($"{DateTime.UtcNow} \t ERROR: Invalid line format in your text file. -> {line}");
                    }
                }

                string[] folders = Directory.GetDirectories(folderPath);

                foreach (string folder in folders)
                {
                    string folderName = Path.GetFileName(folder);

                    if (folderName.EndsWith("_"))
                    {
                        // We are removing the trailing '_' character from the folders to which we added the character.
                        string newFolderName = folderName.Substring(0, folderName.Length - 1);
                        string newFolderPath = Path.Combine(folderPath, newFolderName);

                        if (Directory.Exists(newFolderPath))
                        {
                            Console.WriteLine($"{DateTime.UtcNow} \t ERROR: {newFolderName} could not be added because a folder named {newFolderPath} already exists!");
                        }
                        else
                        {
                            Directory.Move(folder, newFolderPath);
                        }
                    }
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine($"{DateTime.UtcNow} \t An error has occurred: {ex.Message}");
        }

        Console.WriteLine($"{DateTime.UtcNow} \t Total lines in your text file: {totalLines}, number of folders with changed names: {totalChangedLines}.");

        sw.Close();

        break;
    }

    else if (choice == "N" || choice == "n")
    {
        Console.WriteLine("Exiting the application...");
        break;
    }

    else
    {
        Console.WriteLine("Please enter a valid command. Type 'Y' or 'y' for yes, 'N' or 'n' for no.");
        choice = Console.ReadLine();
    }
}

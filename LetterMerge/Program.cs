using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using LetterMerge;


// Assumptions:
// 1. Archive Folder also has the two subfolders, namely, Scholarship and Admission;

namespace LetterProcessing
{
    class Program
    {
        // input: root folder name and (optional) date string in the "yyyyMMdd" format.
        // If no date string found, assume the date is today.
        static void Main(string[] args) 
        {
            // get the root folder, and create
            string rootFolder = args[0];
            string inputDirectory = Path.Combine(rootFolder, "Input");
            string archiveDirectory = Path.Combine(rootFolder, "Archive");
            string outputDir = Path.Combine(rootFolder, "Output");

            string date = DateTime.Today.ToString("yyyyMMdd");
            if (args.Length > 1)
            {
                date = args[1];
            }

            // Initialization: function returns the above strings --> unit test  & exception handling
            InitialCheck initialCheck = new InitialCheck();
            string message = initialCheck.RunChecks(outputDir, date);
            if (message != "")
            {
                Console.WriteLine(message);
                return;
            }
            
            //Create the directory with the dates
            string inputAdmissionDateDir = Path.Combine(inputDirectory, "Admission", date);
            string inputScholarshipDateDir = Path.Combine(inputDirectory, "Scholarship", date);

            LetterService letterService = new LetterService();

            //Archive files
            letterService.ArchiveLetters(inputAdmissionDateDir, archiveDirectory, "Admission", date);
            letterService.ArchiveLetters(inputScholarshipDateDir, archiveDirectory, "Scholarship", date);
            Console.WriteLine("Archive complete");

            //Find students with two letters
            List<string> doubleStudents = letterService.FindIdsWithBoth(inputAdmissionDateDir, inputScholarshipDateDir);
            Console.WriteLine($"The number of students with two letters is {doubleStudents.Count}");

            // create output directory and write report
            letterService.WriteReport(outputDir, date, doubleStudents);
            Console.WriteLine("Report generated");

            // Combine Letters
            string destinationDirectory = Path.Combine(outputDir, date);

            if (doubleStudents.Count > 0)
            {

                foreach (string studentId in doubleStudents)
                {
                    string AdmissionFile = Path.Combine(inputAdmissionDateDir, $"admission-{studentId}.txt");
                    string ScholarshipFile = Path.Combine(inputScholarshipDateDir, $"scholarship-{studentId}.txt");
                    string resultFile = Path.Combine(destinationDirectory, $"funded-{studentId}.txt");
                    letterService.CombineTwoLetters(AdmissionFile, ScholarshipFile, resultFile);
                }
            }

        }

    }
}


/*1.We can find the "id: admission/scholar" key-value pair on-the-fly and store them into a hashmap. 
 * However, in this version I choose to use another function to traverse all the files again because:
 *  1.1 Maintainence; 
 *  1.2 Readbility of the code
 *  1.3 Should I combine the two
 *2. Modules
 *  2.1 Declare functions as static versus use instances of a class
 *  2.2 Single page versus class managerment
 *3. Exception handling
 *  3.1 What if the user starts the program
 *      3.1.1 earlier
 *      3.1.2 later
 *  3.2 what if someone starts it after it has been run?
 *  3.3 Is hour a factor to consider?
 *4. 
 *5. Other questions
 * 5.0 During archive, should I paste or copy?
 * 5.1 Reports to show more statistics
 * 5.2 Should the archive follow the same pattern as the input folder (i.e., with two subfolders each for admission and scolarship?)
 * 5.3 Does scholarship entail admission? 
 *6. 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetterMerge
{
    public interface IletterService  
    {
        /// Archive letters from the Input/Admission/date or Input/Scholarship/date files to the Archive/Admission/date or Archive/Scholarship/date files
        /// <param name="inputDir">File path for the input directory.</param>
        /// <param name="archiveDir">File path for the archive directory.</param>
        /// <param name="date">date for the opeation</param>
        void ArchiveLetters(string inputDir, string archiveDir, string category, string date);

        /// Find student ids with both letters; 
        /// Returns a list of the student ids as strings;
        /// <param name="admissionDir">directory for the admission folder.</param>
        /// <param name="scholarshipDir">directory for the scholarship folder.</param>
        List<string> FindIdsWithBoth(string admissionDir, string scholarshipDir);

        /// Write the final Report which includes date and the ids of the students of both letters
        /// <param name="outputDir">directory for the output folder.</param>
        /// <param name="date">date for the operation.</param>
        /// <param name="doubleStudents">a string of the students ids with both letters.</param>
        public void WriteReport(string outputDir, string date, List<string> doubleStudents);

        /// Combine two letter files into one file.
        /// </summary>
        /// <param name="inputFile1">File path for the first letter.</param>
        /// <param name="inputFile2">File path for the second letter.</param>
        /// <param name="resultFile">File path for the combined letter.</param>
        void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile);
    }
    public class LetterService : IletterService
    {
        public void ArchiveLetters(string inputAdmissionDateDir, string archiveDir, string category, string date)
        {
            // if sourceFolder do not exist, return
            Console.WriteLine($"inputadmissiondir is {inputAdmissionDateDir}");
            if (!Directory.Exists(inputAdmissionDateDir))
            {
                Console.WriteLine($"Make sure you are not running before 10 am. If after 10am, it means No inocming {category} files for the date {date}");
                return;
            }

            // if exists, create the category folder in the Archive directory 
            Console.WriteLine($"Creating {category} files for the date {date}");
            string destinationDirectory = Path.Combine(archiveDir, category, date);
            Directory.CreateDirectory(destinationDirectory);

            //Archive files
            foreach (var file in Directory.GetFiles(inputAdmissionDateDir))
            {
                string FileDestination = Path.Combine(destinationDirectory, Path.GetFileName(file));
                File.Copy(file, FileDestination);
            }
        }
        public List<string> FindIdsWithBoth(string admissionDir, string scholarshipDir)
        {
            //Setup a dictionary to store all the key-value pairs as "id": "category" (for instance, such a pair can be "11223344": "admission")
            Dictionary<string, string> fileMap = new Dictionary<string, string>();

            //Setup a list to store all the students who have two letters
            List<string> doubleStudents = new List<string>();

            // Find all files in the admissionDir and store the key-value pairs in a dictionary
            foreach (string file in Directory.GetFiles(admissionDir, "*.txt"))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                fileMap.Add(fileName.Split('-')[1], fileName.Split('-')[0]);
            }

            // find all files in the scholarDir; if the id is already stored, then store the student's id into the student list
            foreach (string file in Directory.GetFiles(scholarshipDir, "*.txt"))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string idPart = fileName.Split('-')[1];

                if (fileMap.ContainsKey(idPart))
                {
                    Console.WriteLine($"top student found with id {idPart}");
                    doubleStudents.Add(idPart);
                }
            }

            return doubleStudents;
        }
        public void WriteReport(string outputDir, string date, List<string> doubleStudents)
        {
            // create a subfolder with date as name in the outputDir
            string destinationDirectory = Path.Combine(outputDir, date);
            Directory.CreateDirectory(destinationDirectory);
            Console.WriteLine($"outputdir {destinationDirectory} created");
            int count = doubleStudents.Count;

            // write the report
            string reportFile = Path.Combine(destinationDirectory, date + "Report.txt");
            using (var writer = new StreamWriter(reportFile))
            {
                string date_MMddyyyyFormat = DateTime.ParseExact(date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
                writer.WriteLine($"Processing date: {date_MMddyyyyFormat}\tReport");
                writer.WriteLine(new string('-', 20));
                writer.WriteLine($"Number of combined letters: {count}");
                foreach (string studentId in doubleStudents)
                {
                    writer.WriteLine(studentId);
                }
            }
        }
        public void CombineTwoLetters(string inputFile1, string inputile2, string resultFile)
        {
            // Read the contents of the first letter file
            var file1Contents = File.ReadAllText(inputFile1);

            var file2Contents = File.ReadAllText(inputile2);

            // Combine the contents of the two files
            var combinedContents = file1Contents + Environment.NewLine + file2Contents;

            // Write the combined contents to the result file
            File.WriteAllText(resultFile, combinedContents);
        }

    }
}

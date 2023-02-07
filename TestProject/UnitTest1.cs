using LetterMerge;
using NUnit.Framework;
using NUnit.Framework.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestProject
{
    public class Tests
    {
        private InitialCheck initialCheck;
        private LetterService letterService;

        [SetUp]
        public void Setup()
        {
            initialCheck = new InitialCheck();
            letterService = new LetterService();
        }

        // test the ISWeekday method to see if it can correctly determine if a date is workday or not
        [Test]
        public void TestIsWeekday()
        {
            // Test if it's a weekday
            string date = "20230206";
            bool isWeekday = initialCheck.IsWeekday(date);
            Assert.IsTrue(isWeekday);

            // Test if it's not a weekday
            date = "20230205";
            isWeekday = initialCheck.IsWeekday(date);
            Assert.IsFalse(isWeekday);
        }

        // test the IsOutPutGenerated method to see if it can detect a outputDateFolder;
        // the method is used to pre-check if the results have been generated or not
        [Test]
        public void TestIsOutPutGenerated() 
        {
            // Create a test folder 
            string testFolder = "TestFolder0";
            Directory.CreateDirectory(testFolder);
            // create output folder with date
            string date = "20230206";
            string outputFolder = Path.Combine(testFolder, "Output");
            Directory.CreateDirectory(outputFolder);

            string outputDateFolder = Path.Combine(outputFolder, date);
            Directory.CreateDirectory(outputDateFolder);

            //evoke the isoutputgenerated method
            Assert.IsTrue(initialCheck.IsOutputGenerated(outputFolder, date) == true);

        }

        // test the ArchiveLetter method when there are letters to move
        [Test]
        public void TestArchiveLettersCorrectDate()
        {
            // Create a test folder 
            string testFolder = "TestFolder1";
            Directory.CreateDirectory(testFolder);
            // Create input and archive folders
            string inputFolder = Path.Combine(testFolder, "Input");
            Directory.CreateDirectory(inputFolder);
            string archiveFolder = Path.Combine(testFolder, "Archive");
            Directory.CreateDirectory(archiveFolder);

            // Create Admission folders for both input and archive
            string inputAdmissionFolder = Path.Combine(inputFolder, "Admission");
            Directory.CreateDirectory(inputAdmissionFolder);
            string archiveAdmissionFolder = Path.Combine(archiveFolder, "Admission");
            Directory.CreateDirectory(archiveAdmissionFolder);

            // create a date folder within the admission folder
            string date = "20230206";
            string inputAdmissionDateFolder = Path.Combine(inputAdmissionFolder, date);
            Directory.CreateDirectory(inputAdmissionDateFolder);

            // create a empty txt file in the admission Folder
            File.Create(Path.Combine(inputAdmissionDateFolder, "file1.txt")).Close();
            
            // Invoke the ArchiveLetters method
            letterService.ArchiveLetters(inputAdmissionDateFolder, archiveFolder, "Admission", date);

            // Verify that the files have been moved to the archive folder
            string archiveAdmissionDateFolder = Path.Combine(archiveAdmissionFolder, date);
            Assert.IsTrue(Directory.Exists(archiveAdmissionDateFolder));
            Assert.IsTrue(File.Exists(Path.Combine(archiveAdmissionDateFolder, "file1.txt")));
            Assert.IsFalse(File.Exists(Path.Combine(archiveAdmissionDateFolder, "file2.txt")));
            
        }

        // test the ArchiveLetter method when there is no inputAdmissionDate folder
        // Assumption: no directory of the date is equal to no file.
        //             Meaning that if there are no files for the day for "Admission", then no such date file will be generated.
        [Test]
        public void TestArchiveLettersIncorrectDate()
        {
            // Create folders
            string testFolder = "TestFolder2";
            Directory.CreateDirectory(testFolder);
            // Create input and archive folders
            string inputFolder = Path.Combine(testFolder, "Input");
            Directory.CreateDirectory(inputFolder);
            string archiveFolder = Path.Combine(testFolder, "Archive");
            Directory.CreateDirectory(archiveFolder);

            // Create Admission folders for both input and archive
            string inputAdmissionFolder = Path.Combine(inputFolder, "Admission");
            Directory.CreateDirectory(inputAdmissionFolder);
            string archiveAdmissionFolder = Path.Combine(archiveFolder, "Admission");
            Directory.CreateDirectory(archiveAdmissionFolder);

            // create a date folder within the admission folder
            string date = "20230205";
            string inputAdmissionDateFolder = Path.Combine(inputAdmissionFolder, date);
            // Directory.CreateDirectory(inputAdmissionDateFolder);

            // Invoke the ArchiveLetters method
            letterService.ArchiveLetters(inputAdmissionDateFolder, archiveFolder, "Admission", date);

            // Verify that there are no paths
            string archiveAdmissionDateFolder = Path.Combine(archiveAdmissionFolder, date);
            Assert.IsFalse(Directory.Exists(archiveAdmissionDateFolder));


        }

        // Test the findIdsWithBoth method
        [Test]
        public void TestFindIdsWithBoth()
        {

            // Create folders
            string testFolder = "TestFolder3";
            Directory.CreateDirectory(testFolder);
            // Create input folder
            string inputFolder = Path.Combine(testFolder, "Input");
            Directory.CreateDirectory(inputFolder);

            //// Create Admission folders for both input 
            string inputAdmissionFolder = Path.Combine(inputFolder, "Admission");
            Directory.CreateDirectory(inputAdmissionFolder);
            string inputScholarshipFolder = Path.Combine(inputFolder, "Scholarship");
            Directory.CreateDirectory(inputScholarshipFolder);

            ////  create date input folders with two letters each in the Admission.They have the ids "12341234" and "00000000".
            ////  Do the same thing for the Scholarship folder
            string date = "20230206";
            string inputAdmissionDateFolder = Path.Combine(inputAdmissionFolder, date);
            Directory.CreateDirectory(inputAdmissionDateFolder);
            File.Create(Path.Combine(inputAdmissionDateFolder, "admission-12341234.txt")).Close();
            File.Create(Path.Combine(inputAdmissionDateFolder, "admission-00000000.txt")).Close();

            string inputScholarshipDateFolder = Path.Combine(inputScholarshipFolder, date);
            Directory.CreateDirectory(inputScholarshipDateFolder);
            File.Create(Path.Combine(inputScholarshipDateFolder, "admission-12341234.txt")).Close();
            File.Create(Path.Combine(inputScholarshipDateFolder, "admission-00000000.txt")).Close();

            //  Evoke the FindIdsWithBoth method to check if two ids are found
            Assert.IsTrue(letterService.FindIdsWithBoth(inputAdmissionDateFolder, inputScholarshipDateFolder).Count == 2);

        }

        // Test the WriteReport method
        [Test]
        public void TestWriteReport()
        {
            // Create folder
            string testFolder = "TestFolder4";
            Directory.CreateDirectory(testFolder);
            // add date and a list of strings of ids
            string date = "20230206";
            List<string> ids = new List<string>() { "11", "22" };

            //evoke the WriteReport method
            letterService.WriteReport(testFolder, date, ids);
            string filename = date + "Report.txt";

            // Test if the dated subfolder is created
            Assert.IsTrue(Path.Exists(Path.Combine(testFolder, date)));

            // Test if the file exists
            Assert.IsTrue(File.Exists(Path.Combine(testFolder, date, filename)));

            string contents = File.ReadAllText(Path.Combine(testFolder, date, filename));
            // Test if the file includes the ids and the date in the MM/dd/yyyy format
            string date_MMddyyyyFormat = DateTime.ParseExact(date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("MM/dd/yyyy");
            Assert.IsTrue(contents.Contains(date_MMddyyyyFormat));
            Assert.IsTrue(contents.Contains("11"));
            Assert.IsTrue(contents.Contains("22"));
        }

        // Test the CombineTwoLetters method
        [Test]
        public void TestCombineTwoLetters()
        {
            // Create folder
            string testFolder = "TestFolder5";
            Directory.CreateDirectory(testFolder);
            // add files
            File.Create(Path.Combine(testFolder, "admission-12341234.txt")).Close();
            File.Create(Path.Combine(testFolder, "scholarship-12341234.txt")).Close();
            string string1 = "test1";
            string string2 = "test2";
            File.WriteAllText(Path.Combine(testFolder, "admission-12341234.txt"), string1);
            File.WriteAllText(Path.Combine(testFolder, "scholarship-12341234.txt"), string2);

            string AdmissionFile = Path.Combine(testFolder, "admission-12341234.txt");
            string ScholarshipFile = Path.Combine(testFolder, "scholarship-12341234.txt");
            string resultFile = Path.Combine(testFolder, "funded-12341234.txt");

            //evoke the comebine method
            letterService.CombineTwoLetters(AdmissionFile, ScholarshipFile, resultFile);

            Assert.IsTrue(File.Exists(Path.Combine(testFolder, "funded-12341234.txt")));

            string contents = File.ReadAllText(Path.Combine(testFolder, "funded-12341234.txt"));
            Assert.IsTrue(contents.Contains(string1));
            Assert.IsTrue(contents.Contains(string2));
        }

        // Delete all the folders used
        [TearDown]
        public void TearDown()
        {
            //Delete the test folders and all its contents
            List<string> testFolders = new List<string>() { "TestFolder0", "TestFolder1", "TestFolder2", "TestFolder3", "TestFolder4", "TestFolder5" };

            foreach (string folder in testFolders)
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
            }
        }


    }

}

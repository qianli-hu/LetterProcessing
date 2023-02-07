using LetterProcessing;
using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LetterMerge
{
    public class InitialCheck
    {
        public bool IsWeekday(string date)
        {
            DateTime currentDate = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            return currentDate.DayOfWeek == DayOfWeek.Monday || currentDate.DayOfWeek == DayOfWeek.Tuesday ||
                   currentDate.DayOfWeek == DayOfWeek.Wednesday || currentDate.DayOfWeek == DayOfWeek.Thursday ||
                   currentDate.DayOfWeek == DayOfWeek.Friday;
        }

        public bool IsOutputGenerated(string outputdir, string date)
        {
            string destinationPath = Path.Combine(outputdir, date);
            return Path.Exists(destinationPath);
        }

        public string RunChecks(string outputDir, string date)
        {
            {
                string message = "";
                // Check if the date is not a weekday
                if (!IsWeekday(date))
                {
                    message = "Please run the program on the next workday after 10 a.m.";
                    return message;
                }

                // Check if the report has already been generated for the current date
                if (IsOutputGenerated(outputDir, date))
                {
                    //Console.WriteLine($"Output and archive already done for today");
                    message = "Output and archiving already done for today";
                    return message;
                }

                return message;
            }
        }
    }
}
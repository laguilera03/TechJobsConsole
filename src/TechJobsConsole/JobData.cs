using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace TechJobsConsole
{
    class JobData
    {
        static List<Dictionary<string, string>> AllJobs = new List<Dictionary<string, string>>();
        static bool IsDataLoaded = false;

        public static List<Dictionary<string, string>> FindAll()
        {
            LoadData();
            return AllJobs;
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. 
         */
        public static List<string> FindAll(string column)
        {
            LoadData();

            List<string> values = new List<string>();

            foreach (Dictionary<string, string> job in AllJobs)
            {
                string aValue = job[column];

                if (!values.Contains(aValue))
                {
                    values.Add(aValue);
                }
            }
            return values;
        }

        /*
         * Gets the column choice and returns a set of columns
         */
        public static List<Dictionary<string, string>> FindByColumnAndValue(string column, string value)
        {
            // load data, if not already loaded
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();
            
            foreach (Dictionary<string, string> row in AllJobs)
            {
                string aValue = row[column].ToLower();

                if (aValue.Contains(value.ToLower()))
                {
                    jobs.Add(row);
                }
            }
            return jobs;
        }

        /*
         * Load and parse data from job_data.csv
         */
        private static void LoadData()
        {

            if (IsDataLoaded)
            {
                return;
            }

            List<string[]> rows = new List<string[]>();

            using (StreamReader reader = File.OpenText("job_data.csv"))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    string[] rowArrray = CSVRowToStringArray(line);
                    if (rowArrray.Length > 0)
                    {
                        rows.Add(rowArrray);
                    }
                }
            }

            string[] headers = rows[0];
            rows.Remove(headers);

            // Parse each row array into a more friendly Dictionary
            foreach (string[] row in rows)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length; i++)
                {
                    rowDict.Add(headers[i], row[i]);
                }
                AllJobs.Add(rowDict);
            }

            IsDataLoaded = true;
        }

        /*
         * Parse a single line of a CSV file into a string array
         */
        private static string[] CSVRowToStringArray(string row, char fieldSeparator = ',', char stringSeparator = '\"')
        {
            bool isBetweenQuotes = false;
            StringBuilder valueBuilder = new StringBuilder();
            List<string> rowValues = new List<string>();

            // Loop through the row string one char at a time
            foreach (char c in row.ToCharArray())
            {
                if ((c == fieldSeparator && !isBetweenQuotes))
                {
                    rowValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                }
                else
                {
                    if (c == stringSeparator)
                    {
                        isBetweenQuotes = !isBetweenQuotes;
                    }
                    else
                    {
                        valueBuilder.Append(c);
                    }
                }
            }

            // Add the final value
            rowValues.Add(valueBuilder.ToString());
            valueBuilder.Clear();

            return rowValues.ToArray();
        }

        /*
         * Searches for each column and returns all rows of the list that includes the value
         */
        public static List<Dictionary<string, string>> FindByValue(string value, Dictionary<string, string> choices)
        {
            // load data, if not already loaded
            LoadData();

            // Initialize the search
            List<Dictionary<string, string>> search = new List<Dictionary<string, string>>();

            // Make the search list for all columns to exclude all in order to decrease column search redundancy
            List<string> allChoices = new List<string>(); //all choices except all (new ones can be added too)
            foreach (KeyValuePair<string, string> choice in choices)
            {
                if (choice.Key != "all")
                {
                    allChoices.Add(choice.Key);
                }
            }

            // iterate all choices and build the dictionary
            foreach (Dictionary<string, string> job in AllJobs)
            {
                for (int i = 0; i < allChoices.Count; i++)
                {
                    string find = job[allChoices[i]].ToLower();//case insensitive
                    if (find.Contains(value.ToLower()))//case insensitive
                    {
                        search.Add(job);
                        break;//stops from having to iterate each line
                    }
                }
            }

            // Return the search list with dictionaries that include the value
            return search;
        }
    }
}

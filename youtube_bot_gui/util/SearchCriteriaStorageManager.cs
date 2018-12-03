using System;
using System.Collections.Generic;
using System.IO;

namespace youtube_bot_gui.util
{
    public  class SearchCriteriaStorageManager
    {
        public static IList<string> getSearchCriterias()
        {
            try
            {
                IList<string> list = new List<string>();
                TextReader reader = new StreamReader("search_criterias.txt");
                string line = reader.ReadLine();
                while (line != null && !line.Equals(""))
                {
                    list.Add(line.Replace("\n", "").Replace("\r", ""));
                    line = reader.ReadLine();
                }
                reader.Close();
                return list;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public static void addSearchCriteria(string str)
        {
            try
            {
                TextWriter writer = new StreamWriter("search_criterias.txt", true);
                writer.WriteLine(str);
                writer.Close();

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public static void resetFileAndSaveList(IList<string> list)
        {
            try
            {
                TextWriter writer = new StreamWriter("search_criterias.txt");
                foreach (string s in list)
                {
                    writer.WriteLine(s);                    
                }                
                writer.Close();
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
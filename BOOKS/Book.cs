using System;
using System.Collections.Generic;
using System.IO;
namespace BOOKS
{
    /*интерфейс, который будет использован в качестве базового для наследования*/
    internal interface IPaper : ICloneable
    {
        void DeleteFile();
        void CopyFileToDirectory(string directory, string fileName);
        string ConvertToTable();
    }

    /*базовый класс*/
    internal abstract class Record
    {
        public readonly int? cost;
        public readonly string name;
        public readonly DateTime? date;
        public long ID;
        protected static long nextID = 0;

        public Record(string PapersName, string costStr, string PapersYearOfMaden = null)
        {
            if (PapersYearOfMaden != null && PapersYearOfMaden != "")
            {
                if (DateTime.TryParse(PapersYearOfMaden, out DateTime tmpr))
                    date = tmpr;
            }
            if (PapersName == null)
                throw new Exception("MadenOfPaper get null name");
            if (int.TryParse(costStr, out int tmp))
                cost = tmp;
            name = PapersName;
            ID = nextID++;
        }


        /*сортировочные предикаты*/
        public sealed class SortByYear : IComparer<Record>
        {
            public SortByYear(bool inversion = false)
            { inversed = inversion == true ? -1 : 1; }

            private static int inversed;
            public int Compare(Record a, Record b)
            {
                if (!a.date.HasValue && !b.date.HasValue) return 0;
                if (!b.date.HasValue) return 1 * inversed;
                if (!a.date.HasValue || a.date < b.date) return -1 * inversed;
                if (a.date == b.date) return 0;
                return 1 * inversed;
            }
        }
        public sealed class SortByName : IComparer<Record>
        {
            public SortByName(bool inversion = false)
            { inversed = inversion == true ? -1 : 1; }

            private static int inversed;
            public int Compare(Record a, Record b)
            {
                return a.name.CompareTo(b.name) * inversed;
            }
        }
        public sealed class SortByCost : IComparer<Record>
        {
            public SortByCost(bool inversion = false)
            { inversed = inversion == true ? -1 : 1; }

            private static int inversed;
            public int Compare(Record a, Record b)
            {
                if (a.cost.HasValue && b.cost.HasValue)
                {
                    if (a.cost < b.cost)
                        return -1 * inversed;
                    else if (a.cost == b.cost)
                        return 0;
                    else
                        return 1 * inversed;
                }
                else
                {
                    if (!a.cost.HasValue && !b.cost.HasValue)
                    {
                        return 0;
                    }
                    else
                    {
                        if (a.cost.HasValue)
                        { return 1 * inversed; }
                        else { return -1 * inversed; }
                    }
                }
            }
        }
    }

    /*наследование от базового класса и интерфейса*/
    internal class Book : Record, IPaper
    {
        public readonly string[] autors;
        public readonly string file;

        public Book(string[] SetAutors, string SetName, string cost, string SetFile, string SetYear)
            : base(SetName, cost, SetYear)
        {
            autors = SetAutors;
            foreach (var person in autors)
            {
                if (person == null || person == "")
                    throw new Exception("Book autor got null Name");
            }
            if (!File.Exists(SetFile))
                throw new Exception("File Don't exist");
            file = SetFile;
        }
        public void DeleteFile()
        {
            FileInfo a = new FileInfo(file);
            if (a.Exists)
                a.Delete();
        }
        public object Clone()
        {
            return new Book(autors, name, Convert.ToString(cost.Value), file, Convert.ToString(date));
        }
        public void CopyFileToDirectory(string directory, string newName = null)
        {
            if (!Directory.Exists(directory))
                throw new Exception("No Such Directory");
            if (Environment.CurrentDirectory == directory && newName == name)
                newName = file.Insert(file.LastIndexOf('.'), "_new");
            if (newName == null)
                newName = file.Insert(file.LastIndexOf('.'), "_new");
            File.Copy(file, directory + newName);
        }
        public string ConvertToTable()
        {
            const int paddingLength = 17;
            string answer = autors[0].PadLeft(paddingLength) + name.PadLeft(paddingLength);
            if (cost.HasValue)
                answer += cost.Value.ToString().PadLeft(paddingLength);
            else
            {
                string tmp = "";
                answer += tmp.PadLeft(paddingLength);
            }
            if (date.HasValue)
            {
                string tmp = date.ToString().Substring(0, 10);
                while (tmp.Contains(".0"))
                    tmp = tmp.Replace(".0", ".");
                while (tmp[0] == '0')
                    tmp = tmp.Remove(0, 1);
                answer += tmp.PadLeft(paddingLength);
            }
            return answer + '\n';
        }
        public override string ToString()
        {
            string outcost = "";
            string outdate = "";
            if (cost.HasValue)
                outcost = Convert.ToString(cost.Value);
            if (date.HasValue)
                outdate = Convert.ToString(date.Value).Substring(0, 10);
            return string.Join(" ",
                new string[] { autors[0], name, outcost, file, outdate }
                );
        }

        /*сортировочный предикат*/
        public sealed class SortByAutor : IComparer<Book>
        {
            public SortByAutor(bool inversion = false)
            { inversed = inversion == true ? -1 : 1; }

            private static int inversed;
            public int Compare(Book a, Book b)
            {
                return a.autors[0].CompareTo(b.autors[0]) * inversed;
            }
        }
    }
}

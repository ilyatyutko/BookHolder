using System.Collections.Generic;

namespace BOOKS
{
    internal static class Data
    {
        public static List<Book> bookList = new List<Book>();
        public static List<string> deleteList = new List<string>();
        public static bool sortingInversionName = false;
        public static bool sortingInversionAutor = false;
        public static bool sortingInversionYear = false;
        public static bool sortingInversionCost = false;
    }
}

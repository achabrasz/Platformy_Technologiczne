using System.Runtime.Serialization.Formatters.Binary;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", true);
            if (args.Length == 0)
            {
                Console.WriteLine("Nie podano ścieżki do katalogu.");
                return;
            }

            string directoryPath = args[0];

            Console.WriteLine($"Podana ścieżka: {directoryPath}");

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Podana ścieżka jest nieprawidłowa.");
                return;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            Console.WriteLine("Zawartość katalogu:");
            DisplayDirectoryContents(directoryInfo, 0);

            DateTime oldestDate = directoryInfo.GetOldestItemDate();
            Console.WriteLine($"Najstarszy plik: {oldestDate}");

            SortedDictionary<string, long> directoryItems = LoadDirectoryItems(directoryInfo);

            Serialize(directoryItems, "directoryItems.bin");

            SortedDictionary<string, long> deserializedItems = Deserialize<SortedDictionary<string, long>>("directoryItems.bin");
            Console.WriteLine("Zdeserializowana kolekcja:");
            foreach (var item in deserializedItems)
            {
                Console.WriteLine($"{item.Key} -> {item.Value}");
            }
        }

        static void DisplayDirectoryContents(DirectoryInfo directory, int indent)
        {
            foreach (var file in directory.GetFiles())
            {
                Console.WriteLine($"{new string(' ', indent)}{file.Name} {file.Length} bajtów {file.GetDosAttributes()}");
            }
            foreach (var subDirectory in directory.GetDirectories())
            {
                Console.WriteLine($"{new string(' ', indent)}{subDirectory.Name} ({subDirectory.GetFiles().Length}) ----");
                DisplayDirectoryContents(subDirectory, indent + 2);
            }
        }

        static SortedDictionary<string, long> LoadDirectoryItems(DirectoryInfo directory)
        {
            var comparer = new CustomComparer();
            SortedDictionary<string, long> items = new SortedDictionary<string, long>(comparer);

            foreach (var file in directory.GetFiles())
            {
                items[file.Name] = file.Length;
            }
            foreach (var subDirectory in directory.GetDirectories())
            {
                items[subDirectory.Name] = subDirectory.GetFiles().Length;
            }

            return items;
        }

        static void Serialize<T>(T obj, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, obj);
            }
        }

        static T Deserialize<T>(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(fs);
            }
        }
    }

    static class Extensions
    {
        public static string GetDosAttributes(this FileSystemInfo fileSystemInfo)
        {
            string attributes = "";
            if ((fileSystemInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                attributes += "r";
            else
                attributes += "-";
            if ((fileSystemInfo.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
                attributes += "a";
            else
                attributes += "-";
            if ((fileSystemInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                attributes += "h";
            else
                attributes += "-";
            if ((fileSystemInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                attributes += "s";
            else
                attributes += "-";
            return attributes;
        }

        public static DateTime GetOldestItemDate(this DirectoryInfo directoryInfo)
        {
            DateTime oldestDate = DateTime.MaxValue;

            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.CreationTime < oldestDate)
                    oldestDate = file.CreationTime;
            }
            foreach (var subDirectory in directoryInfo.GetDirectories())
            {
                DateTime subOldestDate = subDirectory.GetOldestItemDate();
                if (subOldestDate < oldestDate)
                    oldestDate = subOldestDate;
            }

            return oldestDate;
        }
    }

    [Serializable]
    class CustomComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == null || y == null)
            {
                throw new ArgumentNullException();
            }

            int lengthComparison = x.Length.CompareTo(y.Length);
            if (lengthComparison != 0)
                return lengthComparison;
            else
                return x.CompareTo(y);
        }
    }
}

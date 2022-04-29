namespace TestExAppricot
{
    public class FileSystemEntity : IFileSystemEntity
    {
        private float size;
        public bool IsDir { get; set; }
        public string Name { get; set; } = "";
        public string Deep { get; set; }
        public List<IFileSystemEntity> SubEntites { get; set; }
        public float Size
        {
            get
            {
                float result = 0;

                if (IsDir)
                {
                    if (SubEntites?.Count != 0)
                    {
                        foreach (FileSystemEntity ent in SubEntites)
                        {
                            result += ent.Size;
                        }
                    }
                    size = result;
                    return result;
                }
                else
                {
                    return size;
                }
            }
            set
            { size = value; }
        }

        public FileSystemEntity(FileInfo file, string deep)
        {
            IsDir = false;
            Name = file.Name;
            Size = file.Length;
            Deep = deep;
            SubEntites = new List<IFileSystemEntity>();
        }

        public FileSystemEntity(DirectoryInfo dir, string deep, bool isDir)
        {
            Name = dir.Name;
            Deep = deep;
            IsDir = isDir;
            SubEntites = new List<IFileSystemEntity>();

            try
            {
                FileInfo[]? files = dir.GetFiles("*.*");
                if (files != null)
                {
                    foreach (FileInfo fi in files)
                    {
                        SubEntites.Add(new FileSystemEntity(fi, deep + "-"));
                    }
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                DirectoryInfo[]? subDirs = dir.GetDirectories("*.*");
                if (subDirs.Length > 0)
                {
                    for (int i = 0; i < subDirs.Length; i++)
                    {
                        SubEntites.Add(new FileSystemEntity(subDirs[i], deep + "-", true));
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Output(string[] args, StreamWriter sw)
        {
            if (args.Contains("-q") || args.Contains("--quite"))
            {
                if (args.Contains("-h") || args.Contains("--humanread"))
                {
                    sw.Write($"{Deep + "-"} {Name} ({FormatBytes(Size)}) \n");
                    if (SubEntites.Count > 0)
                    {
                        foreach (FileSystemEntity s in SubEntites)
                        {
                            s.Output(args, sw);
                        }
                    }
                }
                else
                {
                    sw.Write($"{Deep + "-"} {Name} ({Size} B) \n");
                    if (SubEntites.Count > 0)
                    {
                        foreach (FileSystemEntity s in SubEntites)
                        {
                            s.Output(args, sw);
                        }
                    }
                }
            }
            else
            {
                if (args.Contains("-h") || args.Contains("--humanread"))
                {
                    Console.WriteLine($"{Deep + "-"} {Name} ({FormatBytes(Size)})");
                    sw.Write($"{Deep + "-"} {Name} ({FormatBytes(size)}) \n");
                    if (SubEntites.Count > 0)
                    {
                        foreach (FileSystemEntity s in SubEntites)
                        {
                            s.Output(args, sw);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"{Deep + "-"} {Name} ({Size} B)");
                    sw.Write($"{Deep + "-"} {Name} ({size} B) \n");
                    if (SubEntites.Count > 0)
                    {
                        foreach (FileSystemEntity s in SubEntites)
                        {
                            s.Output(args, sw);
                        }
                    }
                }
            }
        }

        private static string FormatBytes(float bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }
            return string.Format($"{dblSByte:0.##} {Suffix[i]}");
        }

        public static void Options(string[] args, out string outputFileName, out string? directoryToWriteResult, out FileSystemEntity myDir)
        {
            string? rootDir;

            outputFileName = $@"sizes-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt";
            directoryToWriteResult = null;
            if (args.Contains("-p") || args.Contains("--path"))
            {
                rootDir = args[Array.IndexOf(args, "-p") + 1];
                if (rootDir == "" || !Directory.Exists(rootDir))
                {
                    Console.WriteLine($"Некорректный путь в параметре -p (--path). Выбрана текущая папка. \n");
                    rootDir = Directory.GetCurrentDirectory();
                }
            }
            else
            {
                rootDir = Directory.GetCurrentDirectory();
            }

            if (args.Contains("-o") || args.Contains("--output"))
            {
                if (Array.IndexOf(args, "-o") + 1 < args.Length)
                    directoryToWriteResult = args[Array.IndexOf(args, "-o") + 1];

                if (directoryToWriteResult == "" || !Directory.Exists(directoryToWriteResult))
                {
                    Console.WriteLine($"Некорректный путь в параметре -o (--output). Выбрана текущая папка. \n");
                    directoryToWriteResult = Directory.GetCurrentDirectory();
                }
            }
            myDir = new FileSystemEntity(new DirectoryInfo(rootDir), "", true);
        }
    }
}

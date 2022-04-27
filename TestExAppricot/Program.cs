
// See https://aka.ms/new-console-template for more information

partial class Program
{
    public class FileSystemEntity : IFileSystemEntity
    {
        private float size;
        public bool IsDir { get; set; }
        public string Name { get; set; } = "";
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

        public string Deep { get; set; }
        public List<IFileSystemEntity> SubEntites { get; set; }
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
                //создаём объект для каждого файла и добавляем в список
                if (files != null)
                {

                    //выводим имена файлов в список

                    foreach (FileInfo fi in files)
                    {
                        // Console.WriteLine($"{deep + "-"} {fi.Name} ");
                        SubEntites.Add(new FileSystemEntity(fi, deep + "-"));
                    }
                }

            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            //catch (IOException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}


            try
            {
                DirectoryInfo[]? subDirs = dir.GetDirectories("*.*");

                //создаём объект для каждого подкаталога и добавляем в список
                if (subDirs.Length > 0)
                {
                    for (int i = 0; i < subDirs.Length; i++)
                    {
                        // Console.WriteLine($"{deep + "-"} {subDirs[i].Name} ");
                        SubEntites.Add(new FileSystemEntity(subDirs[i], deep + "-", true));
                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            //catch(IOException ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }

        public void Output(string[] args, StreamWriter sw)
        {
            if (args.Contains("-q") || args.Contains("--quite"))
            {
                // Console.WriteLine("-q (--quite) - признак вывода сообщений в стандартный поток вывода (если указана, то не выводить лог в консоль. Только в файл);");
                //if (Directory.Exists(Name))
                //{
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
                //  }
            }
            else
            {
                //if (Directory.Exists(Name))
                //{
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
                // }
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
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.Unicode;
        Options(args, out string outputFileName, out string? directoryToWriteResult, out FileSystemEntity myDir);

        using StreamWriter sw = new StreamWriter(Path.Combine(directoryToWriteResult, outputFileName));

        myDir.Output(args, sw);
    }

    private static void Options(string[] args, out string outputFileName, out string? directoryToWriteResult, out FileSystemEntity myDir)
    {
        string? rootDir;

        outputFileName = $@"sizes-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt";
        directoryToWriteResult = Directory.GetCurrentDirectory();
        if (args.Contains("-p") || args.Contains("--path"))
        {
            Console.WriteLine("-p (--path) - путь к папке для обхода");
            rootDir = Console.ReadLine();
            if (rootDir == "" || !Directory.Exists(rootDir))
            {
                Console.WriteLine($"Некорректный путь. Выбрана стандартная папка. \n");
                rootDir = Directory.GetCurrentDirectory();
            }
        }
        else
        {
            rootDir = Directory.GetCurrentDirectory();
        }

        if (args.Contains("-o") || args.Contains("--output"))
        {
            Console.WriteLine("-o (--output) - путь к тестовому файлу, куда записать результаты выполнения расчёта (по-умолчанию файл sizes-YYYY-MM-DD.txt в текущей папке вызова программы);");
            directoryToWriteResult = Console.ReadLine();

        }

        myDir = new FileSystemEntity(new DirectoryInfo(rootDir), "", true);
    }
}

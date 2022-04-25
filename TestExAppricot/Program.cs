
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
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Output(string[] args, StreamWriter sw)
        {
            if (SubEntites.Count > 0)
            {
                if (args.Contains("-q") || args.Contains("--quite"))
                {
                    Console.WriteLine("-q (--quite) - признак вывода сообщений в стандартный поток вывода (если указана, то не выводить лог в консоль. Только в файл);");
                    foreach (FileSystemEntity s in SubEntites)
                    {
                        sw.Write($"{s.Deep + "-"} {s.Name} ({s.size} bytes) \n");

                        s.Output(args, sw);
                    }
                }
                else
                {
                    Console.WriteLine($"{Deep + "-"} {Name} {Size}");
                    sw.Write($"{Deep + "-"} {Name} {Size} \n");
                    foreach (FileSystemEntity s in SubEntites)
                    {
                        // sw.Write(s + "\n");

                        if (!s.IsDir)
                        {
                            sw.Write($"{s.Deep + "-"} {s.Name} ({s.size} bytes) \n");
                            Console.WriteLine($"{s.Deep + "-"} {s.Name} ({s.size} bytes)");
                        }
                        s.Output(args, sw);
                    }

                }

            }
        }

        //public override string ToString()
        //{
        //    return $"{Deep} {Name} {size}"; // Тут все нужные поля вписываете, которые должны в файле быть
        //}
    }

    static void Main(string[] args)
    {
        //Console.Write("> ");

        string? rootDir;

        string outputFileName = $@"sizes-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt"; // имя файла с результатом
        string? directoryToWriteResult = Directory.GetCurrentDirectory();


        if (args.Contains("-p") || args.Contains("--path"))
        {
            Console.WriteLine("-p (--path) - путь к папке для обхода");
            rootDir = Console.ReadLine();
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

        var myDir = new FileSystemEntity(new DirectoryInfo(rootDir), "", true);


        //  Console.WriteLine($"{myDir.Name} ({myDir.Size} bytes)");

        using StreamWriter sw = new StreamWriter(Path.Combine(directoryToWriteResult, outputFileName));
        myDir.Output(args, sw);
    }

}

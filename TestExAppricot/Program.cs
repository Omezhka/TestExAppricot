
// See https://aka.ms/new-console-template for more information

class Program
{
    static void Main(string[] args)
    {
        Console.Write("> ");
        //задаем папку для обхода
        string? rootDir = Console.ReadLine(); /*@"C:\Users\Наталья\source\repos\ConsoleApp3";*/
        List<string> listWalk = new();
        List<float> folderSize = new();

       // folderSize = CalculateFolderSize(rootDir);
        try
        {
            Walk(new DirectoryInfo(rootDir), listWalk);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            Console.WriteLine("Повторите ввод");
            Main(args);
        }
        foreach (string dir in listWalk)
        {
            char[] charsToTrim = { '-', ' ' };
            //folderSize.Add(CalculateFolderSize(dir.Trim(charsToTrim)));
           // foreach (float size in folderSize)
            Console.WriteLine(dir +" "+ CalculateFolderSize(dir.Trim(charsToTrim)) +  " main");
        }
    }

    static void Walk(DirectoryInfo root, List<string> listWalk, string tire = "-")
    {
        
        FileInfo[]? files = null;
        DirectoryInfo[]? subDirs;
        double catalogSize;

        // Получаем все файлы в текущем каталоге
        try
        {
            files = root.GetFiles("*.*");
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        if (files != null)
        {
            //получаем все подкаталоги
            subDirs = root.GetDirectories();

            //получаем размер каталога
          //  catalogSize = CalculateFolderSize(root.ToString());

            //  Console.WriteLine($"{tire} {root.FullName} ({catalogSize} bytes) ");
            listWalk.Add($"{tire} {root.FullName}");
            //выводим имена файлов в консоль
            foreach (FileInfo fi in files)
            {
                listWalk.Add($"{tire + "-"} {fi.Name} ({fi.Length} bytes)");
               // Console.WriteLine($"{tire + "-"} {fi.Name} ({fi.Length} bytes)");
            }

            //проходим по каждому подкаталогу
            foreach (DirectoryInfo dirInfo in subDirs)
            {
                //РЕКУРСИЯ
                Walk(dirInfo, listWalk, tire + "-");
               
            }
           
        }
        
    }

    protected static float CalculateFolderSize(string folder)
    {
        float folderSize = 0.0f;
        
        try
        {
            //Checks if the path is valid or not
            if (!Directory.Exists(folder))
                return folderSize;
            else
            {
                try
            {
                foreach (string file in Directory.GetFiles(folder))
                {
                    if (File.Exists(file))
                    {
                        FileInfo finfo = new FileInfo(file);
                        folderSize += finfo.Length;
                        
                    }
                }

                foreach (string dir in Directory.GetDirectories(folder))
                    folderSize += CalculateFolderSize(dir);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
            }
             }
        }
        catch (UnauthorizedAccessException e)
        {
            Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
        }
        return folderSize;
    }
}

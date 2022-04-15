// See https://aka.ms/new-console-template for more information

class Program
{
    //static StringCollection log = new StringCollection();
    static void Main(string[] args)
    {
        //задаем папку для обхода
        string rootDir = @"C:\Users\Наталья\source\repos\ConsoleApp3";
       
        //вызываем рекурсивный метод
        Walk(new DirectoryInfo(rootDir));
    }
    static void Walk(DirectoryInfo root, string tire = "-")
    {
        FileInfo[] files = null;
        DirectoryInfo[] subDirs = null;
        double catalogSize = 0;
      
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
            catalogSize += CalculateFolderSize(root.ToString());
             Console.WriteLine($"{tire} {root.FullName} ({catalogSize} bytes) ");
            //проходим по каждому подкаталогу

            //выводим имена файлов в консоль
            foreach (FileInfo fi in files)
            {
                Console.WriteLine($" {tire + "-"} {fi.Name} ({fi.Length} bytes)");
            }

            foreach (DirectoryInfo dirInfo in subDirs)
            {  
                //РЕКУРСИЯ
                Walk(dirInfo, tire + "-");
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

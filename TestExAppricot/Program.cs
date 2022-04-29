
// See https://aka.ms/new-console-template for more information
namespace TestExAppricot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            FileSystemEntity.Options(args, out string outputFileName, out string? directoryToWriteResult, out FileSystemEntity myDir);

            using StreamWriter sw = new StreamWriter(Path.Combine(directoryToWriteResult, outputFileName));

            myDir.Output(args, sw);
        }
    }
}

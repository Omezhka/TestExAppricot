namespace TestExAppricot
{
    public interface IFileSystemEntity
    {
        public bool IsDir { get; set; }
        public string Name { get; set; }
        public float Size { get; set; }
        public string Deep { get; set; }
        public void Output(string[] args, StreamWriter sw);
        public List<IFileSystemEntity> SubEntites { get; set; }
    }
}

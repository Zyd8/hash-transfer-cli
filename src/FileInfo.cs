class FileInfo
{
    public int Key { get; set; }
    public string FilePath { get; set; }
    public string HashValue { get; set; }

    public FileInfo(string filePath, int key)
    {
        FilePath = filePath;
        Key = key;
        HashValue = "";
    }
}
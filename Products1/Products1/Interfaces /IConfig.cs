namespace Products1.Interfaces
{
    using SQLite.Net.Interop;

    public interface IConfig
    {
        string DirectoryDB { get; }

        ISQLitePlatform Platform { get; }
    }
}

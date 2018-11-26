using SQLite;

namespace XF.LocalDB.Data
{
    public interface IDependencyServiceSQLite
    {
        SQLiteConnection GetConexao();
    }
}

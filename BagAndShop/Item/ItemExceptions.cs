using SimpleSQLiteORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BagAndShop.Item
{
    public class ItemDataNotFound : Exception
    {
        public SqliteDataBase? DataBase;
        public ItemDataNotFound()
        { }
        public ItemDataNotFound(SqliteDataBase? dataBase)
        {
            DataBase = dataBase;
        }
    }
}

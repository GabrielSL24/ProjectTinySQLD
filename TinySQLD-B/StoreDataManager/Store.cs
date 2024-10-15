using Entities;
using StoreDataManager.Checks;

namespace StoreDataManager
{
    public sealed class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();
        private static string NameDB;
        private static int idTB = 0;
        private static int idDB = 0;
        private TableManager tableManager;

        public static Store GetInstance()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = new Store();
                }
                return instance;
            }
        }

        private const string DatabaseBasePath = @"C:\TinySql\";
        private const string DataPath = $@"{DatabaseBasePath}\Data";
        private const string SystemCatalogPath = $@"{DataPath}\SystemCatalog";
        private const string SystemDatabasesFile = $@"{SystemCatalogPath}\SystemDatabases.Table";
        private const string SystemTablesFile = $@"{SystemCatalogPath}\SystemTables.Table";
        private const string SystemColumnFile = $@"{SystemCatalogPath}\SystemColumns.Table";

        public Store()
        {
            this.InitializeSystemCatalog();
            tableManager = new TableManager(); // Inicializa el TableManager
        }

        private void InitializeSystemCatalog()
        {
            // Always make sure that the system catalog and above folder
            // exist when initializing
            Directory.CreateDirectory(SystemCatalogPath);
        }

        public OperationStatus CreateTable(string NameTable, params (object value, ColumnType type)[] fields)
        {
            return tableManager.CreateTable(NameDB, idDB, NameTable, fields); // Llama al método de TableManager
        }


        public OperationStatus CreateDataBase(string NameDATABASE)
        {
            Directory.CreateDirectory($@"{DataPath}\{NameDATABASE}");
            var DBpath = SystemDatabasesFile;

            using (FileStream stream = new FileStream(DBpath, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new(stream))
            {
                idDB++;
                string Database = NameDATABASE.PadRight(15);

                writer.Write(idDB);
                writer.Write(Database.ToCharArray());
            }
            Console.WriteLine($"Base de datos {NameDATABASE} creada con ID: {idDB}");
            //NameDB = NameDATABASE;       //Esto ya no deberia de hacerse asi debido a que para eso esta el SET 
            return OperationStatus.Success;
        }

        public OperationStatus CreateColumn()
        {
            return OperationStatus.Success;
        }

        public OperationStatus DropTable(string Table)
        {
            ////Verifica si la tabla existe
            //if (CheckAll.Check(SystemTablesFile, Table) == true)
            //{
            //    //File.Delete(Table);
            //    //Console.WriteLine($"Se elimino {Table}");
            //    //Llama la función para eliminar la tabla
            //    CheckTables.RemoveTable(SystemDatabasesFile ,SystemTablesFile, Table);
            //    return OperationStatus.Success;
            //}
            CheckTables.RemoveTable(SystemDatabasesFile, SystemTablesFile, Table);
            return OperationStatus.Success;

            //return OperationStatus.Error;
        }

        public OperationStatus Select()
        {
            // Creates a default Table called ESTUDIANTES
            var tablePath = $@"{DataPath}\Universidad\Funcionarios.Table";
            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryReader reader = new(stream))
            {
                // Print the values as a I know exactly the types, but this needs to be done right11
                Console.WriteLine(reader.ReadInt32());
                Console.WriteLine(reader.ReadString());
                Console.WriteLine(reader.ReadString());
                return OperationStatus.Success;
            }
        }

        public OperationStatus SetDataBase(string NameDATABASE)
        {
            if (CheckAll.Check(SystemDatabasesFile, NameDATABASE))
            {
                NameDB = NameDATABASE;
                Console.WriteLine($"Base de datos '{NameDATABASE}' seleccionada.");
                return OperationStatus.Success;
            }
            else
            {
                Console.WriteLine($"La base de datos '{NameDATABASE}' no fue encontrada.");
                return OperationStatus.Warning;
            }

            //bool databaseExists = false;

            //try
            //{
            //    using (FileStream stream = new FileStream(SystemDatabasesFile, FileMode.Open, FileAccess.Read))
            //    using (BinaryReader reader = new BinaryReader(stream))
            //    {
            //        long recordSize = sizeof(int) + 15 * sizeof(char); // ID + nombre de la base de datos

            //        while (stream.Position + recordSize <= stream.Length)
            //        {
            //            int id = reader.ReadInt32(); // Leer el ID de la base de datos
            //            string dbName = new string(reader.ReadChars(15)).Trim(); // Leer el nombre de la base de datos

            //            // Asegúrate de que la comparación no sea sensible a mayúsculas/minúsculas
            //            if (dbName.Equals(NameDATABASE, StringComparison.OrdinalIgnoreCase))
            //            {
            //                databaseExists = true;
            //                break;
            //            }
            //        }
            //    }
            //}
            //catch (EndOfStreamException ex)
            //{
            //    Console.WriteLine($"Error al leer el archivo: {ex.Message}");
            //    return OperationStatus.Error;
            //}

            //if (databaseExists)
            //{
            //    NameDB = NameDATABASE;
            //    Console.WriteLine($"Base de datos '{NameDATABASE}' seleccionada.");
            //    return OperationStatus.Success;
            //}
            //else
            //{
            //    Console.WriteLine($"La base de datos '{NameDATABASE}' no fue encontrada.");
            //    return OperationStatus.Warning;
            //}
        }


    }

}

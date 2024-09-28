using Entities;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace StoreDataManager
{
    public sealed class Store
    {
        private static Store? instance = null;
        private static readonly object _lock = new object();
        private static string NameDB;
        private static int idTB = 0;
        private static int idDB = 0;
               
        public static Store GetInstance()
        {
            lock(_lock)
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
            
        }

        private void InitializeSystemCatalog()
        {
            // Always make sure that the system catalog and above folder
            // exist when initializing
            Directory.CreateDirectory(SystemCatalogPath);
        }

        public OperationStatus CreateTable(string NameTable, params(object value, ColumnType type)[] fields)
        {

            // Creates a default Table
            var tablePath = $@"{DataPath}\{NameDB}\{NameTable}.Table";

            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryWriter writer = new (stream))
            {
                
                
                // Create an object with a hardcoded.
                // First field is an int, second field is a string of size 30,
                // third is a string of 50
                idTB += 1;
                foreach (var field in fields)
                {
                    switch(field.type)
                    {
                        case ColumnType.Integer:
                            if (field.value is int intvalue)
                            {
                                writer.Write(intvalue);
                            }
                            else
                            {
                                throw new InvalidOperationException("El valor no coincide con en tipo Integer");
                            }
                            break;
                        case ColumnType.DateTime:
                            if (field.value is DateTime dtvalue)
                            {
                                writer.Write(dtvalue.ToBinary());
                            }
                            else
                            {
                                throw new InvalidOperationException("El valor no coincide con el tipo DateTime.");
                            }
                            break;
                        case ColumnType.Double:
                            if (field.value is double doublevalue)
                            {
                                writer.Write(doublevalue);
                            }
                            else
                            {
                                throw new InvalidOperationException("El valor no coincide con el tipo Double.");
                            }
                            break;
                        case ColumnType.Varchar:
                            if (field.value is string strValue)
                            {
                                writer.Write(strValue.ToCharArray());
                            }
                            else
                            {
                                throw new InvalidOperationException("El valor no coincide con el tipo Varchar.");
                            }
                            break;
                        default:
                            throw new InvalidOperationException($"Tipo no soportado: {field.type}");
                    }
                }

                //string nombre = "Isaac".PadRight(30); // Pad to make the size of the string fixed
                //string apellido = "Ramirez".PadRight(50);

                //writer.Write(idTB);
                //writer.Write(nombre);
                //writer.Write(apellido);
            }
            // Directory.CreateDirectory($@"{SystemTablesFile}\{NameTable}");s
            var TBpath = SystemTablesFile;
            using (FileStream stream = new FileStream(TBpath, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new (stream))
            {
                int id = idTB;
                string Table = NameTable.PadRight(15);

                writer.Write(id);
                writer.Write(Table);
            }
            return OperationStatus.Success;
        }

        public OperationStatus CreateDataBase (string NameDATABASE)
        {
            NameDB = NameDATABASE;
            Directory.CreateDirectory($@"{DataPath}\{NameDATABASE}");
            var DBpath = SystemDatabasesFile;
            using (FileStream stream = new FileStream(DBpath, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new(stream))
            {
                idDB =+ 1;
                string Database = NameDATABASE.PadRight(15);

                writer.Write(idDB);
                writer.Write(Database);
            }
            return OperationStatus.Success;
        }

        public OperationStatus CreateColumn()
        {
            return OperationStatus.Success;
        }

        public OperationStatus Select()
        {
            // Creates a default Table called ESTUDIANTES
            var tablePath = $@"{DataPath}\Universidad\Funcionarios.Table";
            using (FileStream stream = File.Open(tablePath, FileMode.OpenOrCreate))
            using (BinaryReader reader = new (stream))
            {
                // Print the values as a I know exactly the types, but this needs to be done right11
                Console.WriteLine(reader.ReadInt32());
                Console.WriteLine(reader.ReadString());
                Console.WriteLine(reader.ReadString());
                return OperationStatus.Success;
            }
        }
    }
}

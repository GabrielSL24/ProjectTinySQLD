using System;
using System.IO;
using Entities;
using StoreDataManager.Checks;

namespace StoreDataManager
{
    public class TableManager
    {
        private const string DataPath = @"C:\TinySql\Data";
        private const string SystemTablesFile = $@"{DataPath}\SystemCatalog\SystemTables.Table";
        private const string SystemColumnsFile = $@"{DataPath}\SystemCatalog\SystemColumns.Table";

        public OperationStatus CreateTable(string nameDB, int idDatabase, string nameTable, params (object value, ColumnType type, string columnName, int extra)[] fields)
        {
            // Verifica si hay una base de datos establecida
            if (string.IsNullOrEmpty(nameDB))
            {
                Console.WriteLine("No hay ninguna base de datos seleccionada.");
                return OperationStatus.Error;
            }
            // Asegúrate de que la carpeta de la base de datos exista
            if (!Directory.Exists($@"{DataPath}\{nameDB}"))
            {
                Console.WriteLine($"La base de datos '{nameDB}' no existe.");
                return OperationStatus.Error;
            }
            //Lláma a la función para que retorne el ID de la última tabla creada en su respectivo Database
            int idTable = CheckAll.CheckID(idDatabase, SystemTablesFile);
            idTable++;
            var tablePath = $@"{DataPath}\{nameDB}\{nameTable}.Table";

            // Añadir la tabla al archivo de tablas
            AddTableToSystem(nameTable, idDatabase, idTable);
            Console.WriteLine($"Tabla {nameTable}, con ID {idTable} creada en la base de datos '{nameDB}'.");

            //Agrega la columna a SystemColumns con sus datos
            using (FileStream stream = File.Open(SystemColumnsFile, FileMode.Append))
            using (BinaryWriter writer = new(stream))
            {
                foreach (var field in fields)
                {
                    WriteField(idTable, writer, field);
                }
            }
            return OperationStatus.Success;
        }
        //Método para escribir en SystemColumns el registro de la columna.
        private void WriteField(int idTB,BinaryWriter writer, (object value, ColumnType type, string columnName, int extra) field)
        {
            string columnName = field.columnName.PadRight(15);
            string type = field.type.ToString();
            
            writer.Write(idTB);
            writer.Write(columnName.ToCharArray());
            writer.Write(type.PadRight(10).ToCharArray());
            writer.Write(field.extra);
        }
        //Método para agregar una tabla en SystemTablesFile
        private void AddTableToSystem(string nameTable, int IdDB, int idTB)
        {
            var TBpath = SystemTablesFile;
            using (FileStream stream = new FileStream(TBpath, FileMode.Append, FileAccess.Write))
            using (BinaryWriter writer = new(stream))
            {
                int IdDatabase = IdDB;
                int idTable = idTB;
                string table = nameTable.PadRight(15);

                writer.Write(IdDatabase);
                writer.Write(idTable);
                writer.Write(table.ToCharArray());
            }
        }
    }
}

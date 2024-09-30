using Antlr4.Runtime;
using System;

//public class SQLParser
//{
//    // Método que parsea una sentencia SQL usando ANTLR
//    public bool Parse(string sqlQuery)
//    {
//        // Crear un input stream a partir de la consulta SQL
//        AntlrInputStream inputStream = new AntlrInputStream(sqlQuery);

//        // Crear el lexer (asume que ya tienes el lexer generado por ANTLR)
//        MySQLLexer lexer = new MySQLLexer(inputStream);

//        // Crear el token stream
//        CommonTokenStream tokenStream = new CommonTokenStream(lexer);

//        // Crear el parser (asume que ya tienes el parser generado por ANTLR)
//        MySQLParser parser = new MySQLParser(tokenStream);

//        try
//        {
//            // Este es el punto de entrada, que parsea la consulta SQL.
//            // Elige el método apropiado según la gramática (ej: sqlStatement)
//            parser.sqlStatement();

//            // Si llega hasta aquí, la sentencia SQL es válida
//            Console.WriteLine("Consulta SQL válida.");
//            return true;
//        }
//        catch (Exception ex)
//        {
//            // Si hay un error de sintaxis, lo capturamos aquí
//            Console.WriteLine($"Error de sintaxis en la consulta SQL: {ex.Message}");
//            return false;
//        }
//    }
//}

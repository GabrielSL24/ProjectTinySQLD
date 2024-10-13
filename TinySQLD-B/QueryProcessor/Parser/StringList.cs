using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryProcessor.Parser
{
    public class StringList
    {
        private string[] items;
        private int count;

        public StringList()
        {
            items = new string[10];
            count = 0;
        }

        // Método para añadir un nuevo elemento a la lista
        public void Add(string item)
        {
            if (count == items.Length)
            {
                Resize();
            }
            items[count] = item;
            count++;
        }

        // Método para obtener el elemento en una posición específica
        public string Get(int index)
        {
            if (index < 0 || index >= count)
            {
                throw new IndexOutOfRangeException("Índice fuera de rango.");
            }
            return items[index];
        }

        // Método que devuelve el número de elementos en la lista
        public int Count()
        {
            return count;
        }

        // Método para redimensionar el arreglo cuando está lleno
        private void Resize()
        {
            string[] newArray = new string[items.Length * 2];
            for (int i = 0; i < items.Length; i++)
            {
                newArray[i] = items[i];
            }
            items = newArray;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SMTest
{
    public static class ObservableCollectionExtentions
    {
        /// <summary>
        /// Метод расширения - добавляет в ObservableCollection заданную коллекцию реализующуй интерфейс IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Исходная коллекция ObservableCollection</param>
        /// <param name="insertData">Коллекция для вставки</param>
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> insertData)
        {
            foreach (var item in insertData)
            {
                collection.Add(item);
            }
        }
    }
}

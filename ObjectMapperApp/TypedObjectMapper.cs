using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace ObjectMapperApp
{
    /// <summary>
    /// Performs Manual Object Relational Mapping using Reflection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypedObjectMapper<T>
    {
        private DbDataReader reader;
        private PropertyInfo[] pi;
        public TypedObjectMapper(DbDataReader reader)
        {
            this.reader = reader;
            pi = typeof(T).GetProperties();
        }
        
        public void Map(out List<T> objList)
        {
            objList = new List<T>();
            while (reader.Read())
            {
                T temp = (T)Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo p in pi)
                {
                    for(int i = 0; i < reader.FieldCount; i++)
                    {
                        if(reader.GetName(i).Equals(p.Name))
                        {
                           if(!reader.IsDBNull(i)) p.SetValue(temp, reader[i]);
                            break;
                        }
                    }
                }
                objList.Add(temp);
            }
        }
    }
}
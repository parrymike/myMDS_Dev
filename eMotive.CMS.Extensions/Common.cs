using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using ServiceStack.Text;

namespace eMotive.CMS.Extensions
{
    public static class Common
    {
        /// <summary>
        /// Produces a key value pair collection from selected fields of an IEnumerable list of objects.
        /// </summary>
        /// <typeparam name="T">The type of objected contained within the IEnumerable list.</typeparam>
        /// <param name="enumerable">A list of objects.</param>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairList<T>(this IEnumerable<T> enumerable, Func<T, string> value, Func<T, string> text)
        {
            return enumerable.Select(f => new KeyValuePair<string, string>(value(f), text(f)));
        }

        /*   public static TModel DoMap<T, TModel>(this T entity) where TModel : class
           {
               return (TModel)Mapper.Map(entity, entity.GetType(), typeof(TModel));
               // return (TModel)Mapper.Map(entity, TModel);
           }*/

        /// <summary>
        /// Will appand a 's' to a string depending if count is singualr or plural.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string SingularOrPlural(this string term, int count)
        {
            return count == 1 ? term : term + "s";
        }


        /// <summary>
        /// Checks if a list is not null and contains objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool HasContent<T>(this IList<T> collection)
        {
            return collection != null && collection.Any();
        }

        public static bool HasContent<T>(this IEnumerable<T> collection)
        {
            //TODO: Will this cause performance issues???
            return collection != null && collection.Skip(0).Any();
        }

        public static bool HasContent<T, T1>(this IDictionary<T, T1> collection)
        {
            return collection != null && collection.Any();
        }




        public static bool IsEmpty<T>(this IList<T> collection)
        {
            return !(collection != null && collection.Any());
        }

        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            //TODO: Will this cause performance issues???
            return !(collection != null && collection.Skip(0).Any());
        }

        public static bool IsEmpty<T, T1>(this IDictionary<T, T1> collection)
        {
            return !(collection != null && collection.Any());
        }





        /// <summary>
        /// Serialises an object to Json using the ServiceStack Json serialiser.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonSerializer.SerializeToString(obj);
        }

        public static T FromJson<T>(this string obj)
        {
            return JsonSerializer.DeserializeFromString<T>(obj);
        }

        /// <summary>
        /// Clones an object. Useful for utilising shared objects stored within cache which need to be modified only for a particualr request.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DeepClone(this object obj)
        {

            if (obj == null)
                return null;

            // Create a "deep" clone of 
            // an object. That is, copy not only
            // the object and its pointers
            // to other objects, but create 
            // copies of all the subsidiary 
            // objects as well. This code even 
            // handles recursive relationships.

            object objResult;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                // Rewind back to the beginning 
                // of the memory stream. 
                // Deserialize the data, then
                // close the memory stream.
                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }
            return objResult;
        }

        public static T DeepClone<T>(this object obj)
        {

            if (obj == null)
                return default(T);

            // Create a "deep" clone of 
            // an object. That is, copy not only
            // the object and its pointers
            // to other objects, but create 
            // copies of all the subsidiary 
            // objects as well. This code even 
            // handles recursive relationships.

            object objResult;
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                // Rewind back to the beginning 
                // of the memory stream. 
                // Deserialize the data, then
                // close the memory stream.
                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }

            return (T)objResult;
        }

        /// <summary>
        /// Checks to see if the entered value is an integer
        /// </summary>
        /// <param name="obj">Value to check</param>
        /// <returns>bool</returns>
        public static bool IsNumeric(this object obj)
        {
            if (obj == null)
                return false;

            double retNum;
            var isNum = Double.TryParse(Convert.ToString(obj), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
    }
}

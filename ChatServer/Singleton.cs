using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChatServer
{
    internal abstract class Singleton<T> where T : class, new()
    {
        private static readonly T s_instance = new T();

        public static T Instance => s_instance;

        protected Singleton() { }
    }
}

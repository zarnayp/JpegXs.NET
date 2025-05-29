using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegXs.NET
{
    public static class EngineFactory
    {
        static public T? CreateEngine<T>() 
        {
            object? engine = null;

            if (typeof(T) == typeof(ISvtEngine))
            {
                engine = new SvtEngine();
            }         

            return (T?)engine;
        }
    }
}

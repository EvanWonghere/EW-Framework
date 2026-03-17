using System;
using System.Reflection;

namespace EW_Framework.Core.Singleton.Base
{
    /// <summary>
    /// Pure C# singleton base class (not MonoBehaviour)
    /// Extremely lightweight, suitable for pure data management or algorithm classes
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        // Use Lazy<T> to ensure thread safety and perfect lazy loading
        private static readonly Lazy<T> _instance = new Lazy<T>(CreateInstanceOfT, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        public static T Instance => _instance.Value;

        // Force subclasses to not have a public constructor, ensuring the absolute purity of the singleton
        protected Singleton() { }

        // Call the private constructor through reflection to prevent external direct new T()
        private static T CreateInstanceOfT()
        {
            var constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException($"[Singleton] {typeof(T).Name} must contain a private parameterless constructor!");
            }
            return (T)constructors[0].Invoke(null);
        }
    }
}
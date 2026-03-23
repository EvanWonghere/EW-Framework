namespace EW_Framework.Core.ObjectPool.Base
{
    /// <summary>
    /// Interface of Reference Pool
    /// Any pure C# class that wants to be managed by the reference pool must implement this interface
    /// </summary>
    public interface IReference<T>
    {
        /// <summary>
        /// Called when the object is returned to the pool, must clear all references and reset all values
        /// To completely prevent dirty data and memory leaks
        /// </summary>
        void OnReturnPool();
    }
}

using System.Threading.Tasks;

namespace RunningWater.Raspberry.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        Task<TValue> GetValueAsync<TValue>(string key, TValue defaultValue = default(TValue));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetValueAsync<TValue>(string key, TValue value);
    }
}

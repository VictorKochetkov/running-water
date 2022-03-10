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
        TValue GetValue<TValue>(string key, TValue defaultValue = default(TValue));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        void SetValue(string key, object value);
    }
}

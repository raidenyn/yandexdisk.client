namespace System.Threading.Tasks
{
    /// <summary>
    /// Set of polyfils for <see cref="Task"/> from .NET 4.5
    /// </summary>
    public static class TaskPf
    {
        /// <summary>
        /// Polyfil for <see cref="Task.CompletedTask"/>
        /// </summary>
        public static Task CompletedTask
        {
            get { return FromResult<object>(null); }
        }

        /// <summary>
        /// Polyfil for <see cref="Task.FromResult"/>
        /// </summary>
        public static Task<T> FromResult<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }
    }
}

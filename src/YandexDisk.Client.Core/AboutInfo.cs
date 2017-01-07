using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace YandexDisk.Client
{
    /// <summary>
    /// Class provided assymbly and file description
    /// </summary>
    [PublicAPI]
    public class AboutInfo
    {
        private readonly Assembly _assembly;
        private string _productTitle;
        private static string _version;

        /// <param name="assembly">Assembly for information providing</param>
        public AboutInfo(Assembly assembly)
        {
            _assembly = assembly;
        }

        /// <summary>
        /// Return product title from AssemblyTitleAttribute
        /// </summary>
        [PublicAPI, NotNull]
        public string ProductTitle => _productTitle ?? (_productTitle = GetAttribute<AssemblyTitleAttribute>().Title);

        /// <summary>
        /// Return version of assembly
        /// </summary>
        [PublicAPI, NotNull]
        public string Version => _version ?? (_version = _assembly.GetName().Version.ToString());

        private TAttr GetAttribute<TAttr>()
            where TAttr: System.Attribute
        {
            return (TAttr)_assembly.GetCustomAttributes(typeof(TAttr)).First();
        }

        /// <summary>
        /// Default Info for IDiskApi
        /// </summary>
        [PublicAPI, NotNull]
        public static readonly AboutInfo Client = new AboutInfo(typeof(IDiskApi).GetTypeInfo().Assembly);
    }
}
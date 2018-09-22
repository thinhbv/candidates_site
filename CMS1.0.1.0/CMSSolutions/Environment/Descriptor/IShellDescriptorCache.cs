using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Castle.Core.Logging;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.FileSystems.AppData;
using CMSSolutions.Localization;

namespace CMSSolutions.Environment.Descriptor
{
    /// <summary>
    /// Single service instance registered at the host level. Provides storage
    /// and recall of shell descriptor information. Default implementation uses
    /// app_data, but configured replacements could use other per-host writable location.
    /// </summary>
    public interface IShellDescriptorCache
    {
        /// <summary>
        /// Recreate the named configuration information. Used at startup.
        /// Returns null on cache-miss.
        /// </summary>
        ShellDescriptor Fetch(string shellName);

        /// <summary>
        /// Commit named configuration to reasonable persistent storage.
        /// This storage is scoped to the current-server and current-webapp.
        /// Loss of storage is expected.
        /// </summary>
        void Store(string shellName, ShellDescriptor descriptor);
    }

    public class ShellDescriptorCache : IShellDescriptorCache
    {
        private readonly IAppDataFolder appDataFolder;
        private const string DescriptorCacheFileName = "cache.dat";

        public ShellDescriptorCache(IAppDataFolder appDataFolder)
        {
            this.appDataFolder = appDataFolder;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public Localizer T { get; set; }

        #region Implementation of IShellDescriptorCache

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "StringReader closed by XmlReader.")]
        public ShellDescriptor Fetch(string name)
        {
            VerifyCacheFile();
            var text = appDataFolder.ReadFile(DescriptorCacheFileName);
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(text);
            XmlNode rootNode = xmlDocument.DocumentElement;
            if (rootNode != null)
            {
                foreach (XmlNode tenantNode in rootNode.ChildNodes)
                {
                    if (String.Equals(tenantNode.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return GetShellDecriptorForCacheText(tenantNode.InnerText);
                    }
                }
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "writer closed by xmlWriter.")]
        public void Store(string name, ShellDescriptor descriptor)
        {
            VerifyCacheFile();
            var text = appDataFolder.ReadFile(DescriptorCacheFileName);
            bool tenantCacheUpdated = false;
            var saveWriter = new StringWriter();
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(text);
            XmlNode rootNode = xmlDocument.DocumentElement;
            if (rootNode != null)
            {
                foreach (XmlNode tenantNode in rootNode.ChildNodes)
                {
                    if (String.Equals(tenantNode.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        tenantNode.InnerText = GetCacheTextForShellDescriptor(descriptor);
                        tenantCacheUpdated = true;
                        break;
                    }
                }
                if (!tenantCacheUpdated)
                {
                    XmlElement newTenant = xmlDocument.CreateElement(name);
                    newTenant.InnerText = GetCacheTextForShellDescriptor(descriptor);
                    rootNode.AppendChild(newTenant);
                }
            }

            xmlDocument.Save(saveWriter);
            appDataFolder.CreateFile(DescriptorCacheFileName, saveWriter.ToString());
        }

        #endregion Implementation of IShellDescriptorCache

        private static string GetCacheTextForShellDescriptor(ShellDescriptor descriptor)
        {
            var sb = new StringBuilder();
            sb.Append(descriptor.SerialNumber + "|");
            foreach (var feature in descriptor.Features)
            {
                sb.Append(feature.Name + ";");
            }

            return sb.ToString();
        }

        private static ShellDescriptor GetShellDecriptorForCacheText(string p)
        {
            string[] fields = p.Trim().Split(new[] { "|" }, StringSplitOptions.None);
            var shellDescriptor = new ShellDescriptor { SerialNumber = Convert.ToInt32(fields[0]) };
            string[] features = fields[1].Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            shellDescriptor.Features = features.Select(feature => new ShellFeature { Name = feature }).ToList();

            return shellDescriptor;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void VerifyCacheFile()
        {
            if (!appDataFolder.FileExists(DescriptorCacheFileName))
            {
                var writer = new StringWriter();
                using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("Tenants");
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                }
                appDataFolder.CreateFile(DescriptorCacheFileName, writer.ToString());
            }
        }
    }
}
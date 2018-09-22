using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.FileSystems.AppData;
using CMSSolutions.FileSystems.LockFile;
using CMSSolutions.Indexing.Models;

namespace CMSSolutions.Indexing.Services
{
    [Feature(Constants.Areas.Indexing)]
    public class IndexingTaskExecutor : IIndexingTaskExecutor, IIndexStatisticsProvider
    {
        private readonly ILockFileManager _lockFileManager;
        private readonly IIndexManager _indexManager;
        private readonly IAppDataFolder _appDataFolder;
        private readonly ShellSettings _shellSettings;
        private IIndexProvider _indexProvider;
        private readonly IEnumerable<IIndexingContentProvider> contentProviders;
        private IndexingStatus _indexingStatus = IndexingStatus.Idle;

        public IndexingTaskExecutor(ILockFileManager lockFileManager, IIndexManager indexManager, IAppDataFolder appDataFolder, ShellSettings shellSettings, IEnumerable<IIndexingContentProvider> contentProviders)
        {
            _lockFileManager = lockFileManager;
            _indexManager = indexManager;
            _appDataFolder = appDataFolder;
            _shellSettings = shellSettings;
            this.contentProviders = contentProviders;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool DeleteIndex(string indexName)
        {
            ILockFile lockFile = null;
            var settingsFilename = GetSettingsFileName(indexName);
            var lockFilename = settingsFilename + ".lock";

            // acquire a lock file on the index
            if (!_lockFileManager.TryAcquireLock(lockFilename, ref lockFile))
            {
                Logger.Info("Could not delete the index. Already in use.");
                return false;
            }

            using (lockFile)
            {
                if (!_indexManager.HasIndexProvider())
                {
                    return false;
                }

                var searchProvider = _indexManager.GetSearchIndexProvider();
                if (searchProvider.Exists(indexName))
                {
                    searchProvider.DeleteIndex(indexName);
                }

                DeleteSettings(indexName);
            }

            return true;
        }

        public bool UpdateIndex(string indexName)
        {
            ILockFile lockFile = null;
            var settingsFilename = GetSettingsFileName(indexName);
            var lockFilename = settingsFilename + ".lock";

            // acquire a lock file on the index
            if (!_lockFileManager.TryAcquireLock(lockFilename, ref lockFile))
            {
                Logger.Info("Index was requested but is already running");
                return false;
            }

            using (lockFile)
            {
                if (!_indexManager.HasIndexProvider())
                {
                    return false;
                }

                // load index settings to know what is the current state of indexing
                var indexSettings = LoadSettings(indexName);

                _indexProvider = _indexManager.GetSearchIndexProvider();
                _indexProvider.CreateIndex(indexName);

                return UpdateIndex(indexName, settingsFilename, indexSettings);
            }
        }

        private bool UpdateIndex(string indexName, string settingsFilename, IndexSettings indexSettings)
        {
            var addToIndex = new List<IDocumentIndex>();

            // Rebuilding the inde
            Logger.Info("Rebuilding index");
            _indexingStatus = IndexingStatus.Rebuilding;

            foreach (var contentProvider in contentProviders)
            {
                addToIndex.AddRange(contentProvider.GetDocuments(id => _indexProvider.New(id)));
            }

            // save current state of the index
            indexSettings.LastIndexedUtc = DateTime.UtcNow;
            _appDataFolder.CreateFile(settingsFilename, indexSettings.ToXml());

            if (addToIndex.Count == 0)
            {
                // nothing more to do
                _indexingStatus = IndexingStatus.Idle;
                return false;
            }

            // save new and updated documents to the index
            _indexProvider.Store(indexName, addToIndex);
            Logger.InfoFormat("Added documents to index: {0}", addToIndex.Count);

            return true;
        }

        public DateTime GetLastIndexedUtc(string indexName)
        {
            var indexSettings = LoadSettings(indexName);
            return indexSettings.LastIndexedUtc;
        }

        public IndexingStatus GetIndexingStatus(string indexName)
        {
            return _indexingStatus;
        }

        private string GetSettingsFileName(string indexName)
        {
            return _appDataFolder.Combine("Sites", _shellSettings.Name, indexName + ".settings.xml");
        }

        /// <summary>
        /// Loads the settings file or create a new default one if it doesn't exist
        /// </summary>
        private IndexSettings LoadSettings(string indexName)
        {
            var indexSettings = new IndexSettings();
            var settingsFilename = GetSettingsFileName(indexName);
            if (_appDataFolder.FileExists(settingsFilename))
            {
                var content = _appDataFolder.ReadFile(settingsFilename);
                indexSettings = IndexSettings.Parse(content);
            }

            return indexSettings;
        }

        /// <summary>
        /// Deletes the settings file
        /// </summary>
        private void DeleteSettings(string indexName)
        {
            var settingsFilename = GetSettingsFileName(indexName);
            if (_appDataFolder.FileExists(settingsFilename))
            {
                _appDataFolder.DeleteFile(settingsFilename);
            }
        }
    }
}
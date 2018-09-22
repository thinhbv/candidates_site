using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Web.Compilation;
using Castle.Core.Logging;
using CMSSolutions.FileSystems.VirtualPath;

namespace CMSSolutions.Environment
{
    public interface IViewsBackgroundCompilation
    {
        void Start();

        void Stop();
    }

    public class ViewsBackgroundCompilation : IViewsBackgroundCompilation
    {
        private readonly IVirtualPathProvider virtualPathProvider;
        private volatile bool stopping;

        public ViewsBackgroundCompilation(IVirtualPathProvider virtualPathProvider)
        {
            this.virtualPathProvider = virtualPathProvider;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void Start()
        {
            stopping = false;
            var timer = new Timer();
            timer.Elapsed += CompileViews;
            timer.Interval = TimeSpan.FromMilliseconds(1500).TotalMilliseconds;
            timer.AutoReset = false;
            timer.Start();
        }

        public void Stop()
        {
            stopping = true;
        }

        public class CompilationContext
        {
            public IEnumerable<string> DirectoriesToBrowse { get; set; }

            public IEnumerable<string> FileExtensionsToCompile { get; set; }

            public HashSet<string> ProcessedDirectories { get; set; }
        }

        private void CompileViews(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var totalTime = new Stopwatch();
            totalTime.Start();
            Logger.Info("Starting background compilation of views");
            ((Timer)sender).Stop();

            // Hard-coded context based on current system profile
            var context = new CompilationContext
            {
                // Put most frequently used directories first in the list
                DirectoriesToBrowse = new[] {
                    // Setup
                    "~/Modules/Orchard.Setup/Views",
                    "~/Themes/SafeMode/Views",

                    // Common
                    "~/Core/Contents/Views",
                    "~/Core/Common/Views",
                    "~/Core/Settings/Views",

                    // Dashboard
                    "~/Core/Dashboard/Views",
                    "~/Themes/TheAdmin/Views",

                    // Content Items
                    "~/Modules/Orchard.PublishLater/Views",

                    // Content Types
                    "~/Modules/Orchard.ContentTypes/Views",

                    // "Edit" homepage
                    "~/Modules/TinyMce/Views",
                    "~/Modules/Orchard.Tags/Views",
                    "~/Core/Navigation/Views",

                    // Various other admin pages
                    "~/Core/Settings/Views",
                    "~/Core/Containers/Views",
                    "~/Modules/Orchard.Widgets/Views",
                    "~/Modules/Orchard.Users/Views",
                    "~/Modules/Orchard.Media/Views",
                    "~/Modules/Orchard.Comments/Views",

                    // Leave these at end (as a best effort)
                    "~/Core", "~/Modules", "~/Themes"
                },
                FileExtensionsToCompile = new[] { ".cshtml", ".acsx", ".aspx" },
                ProcessedDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            };

            var directories = context
                .DirectoriesToBrowse
                .SelectMany(folder => GetViewDirectories(folder, context.FileExtensionsToCompile));

            int directoryCount = 0;
            foreach (var viewDirectory in directories)
            {
                if (stopping)
                {
                    if (Logger.IsInfoEnabled)
                    {
                        var leftOvers = directories.Except(context.ProcessedDirectories).ToList();
                        Logger.InfoFormat("Background compilation stopped before all directories were processed ({0} directories left)", leftOvers.Count);
                        foreach (var directory in leftOvers)
                        {
                            Logger.InfoFormat("Directory not processed: '{0}'", directory);
                        }
                    }
                    break;
                }

                CompileDirectory(context, viewDirectory);
                directoryCount++;
            }

            totalTime.Stop();
            Logger.InfoFormat("Ending background compilation of views, {0} directories processed in {1} sec", directoryCount, totalTime.Elapsed.TotalSeconds);
        }

        private void CompileDirectory(CompilationContext context, string viewDirectory)
        {
            // Prevent processing of the same directories multiple times (sligh performance optimization,
            // as the build manager second call to compile a view is essentially a "no-op".
            if (context.ProcessedDirectories.Contains(viewDirectory))
                return;
            context.ProcessedDirectories.Add(viewDirectory);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var firstFile = virtualPathProvider
                    .ListFiles(viewDirectory).FirstOrDefault(f => context.FileExtensionsToCompile.Any(e => f.EndsWith(e, StringComparison.OrdinalIgnoreCase)));

                if (firstFile != null)
                    BuildManager.GetCompiledAssembly(firstFile);
            }
            catch (Exception e)
            {
                // Some views might not compile, this is ok and harmless in this
                // context of pre-compiling views.
                Logger.InfoFormat(e, "Compilation of directory '{0}' skipped", viewDirectory);
            }
            stopwatch.Stop();
            Logger.InfoFormat("Directory '{0}' compiled in {1} msec", viewDirectory, stopwatch.ElapsedMilliseconds);
        }

        private IEnumerable<string> GetViewDirectories(string directory, IEnumerable<string> extensions)
        {
            var result = new List<string>();
            GetViewDirectories(virtualPathProvider, directory, extensions, result);
            return result;
        }

        private static void GetViewDirectories(IVirtualPathProvider vpp, string directory, IEnumerable<string> extensions, ICollection<string> files)
        {
            if (vpp.ListFiles(directory).Any(f => extensions.Any(e => f.EndsWith(e, StringComparison.OrdinalIgnoreCase))))
            {
                files.Add(directory);
            }

            foreach (var childDirectory in vpp.ListDirectories(directory).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
            {
                GetViewDirectories(vpp, childDirectory, extensions, files);
            }
        }
    }
}
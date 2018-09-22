﻿using System;
using System.Threading;
using System.Web.Razor.Generator;
using System.Web.WebPages.Razor;
using CMSSolutions.Environment;

namespace CMSSolutions.Web.Mvc.ViewEngines.Razor
{
    public class RazorCompilationEventsShim : IShim
    {
        private static int initialized;

        private RazorCompilationEventsShim()
        {
            HostContainerRegistry.RegisterShim(this);
            RazorBuildProvider.CodeGenerationStarted += RazorBuildProviderCodeGenerationStarted;
            RazorBuildProvider.CodeGenerationCompleted += RazorBuildProviderCodeGenerationCompleted;
        }

        public IHostContainer HostContainer { get; set; }

        private void RazorBuildProviderCodeGenerationStarted(object sender, EventArgs e)
        {
            var provider = (RazorBuildProvider)sender;
            HostContainer.Resolve<IRazorCompilationEvents>().CodeGenerationStarted(provider);
        }

        private void RazorBuildProviderCodeGenerationCompleted(object sender, CodeGenerationCompleteEventArgs e)
        {
            var provider = (RazorBuildProvider)sender;
            HostContainer.Resolve<IRazorCompilationEvents>().CodeGenerationCompleted(provider, e);
        }

        public static void EnsureInitialized()
        {
            var uninitialized = Interlocked.CompareExchange(ref initialized, 1, 0) == 0;
            if (uninitialized)
                new RazorCompilationEventsShim();
        }
    }
}
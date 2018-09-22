using System;
using System.Collections.Generic;

namespace CMSSolutions.Web.UI.Grid
{
    /// <summary>
    /// Represents a Grid Row
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridRow<T>
    {
        private Func<GridRowViewData<T>, IDictionary<string, object>> attributes = x => new Dictionary<string, object>();
        private Func<GridRowViewData<T>, RenderingContext, bool> startSectionRenderer = (x, y) => false;
        private Func<GridRowViewData<T>, RenderingContext, bool> endSectionRenderer = (x, y) => false;

        /// <summary>
        /// Invokes the custom renderer defined (if any) for the start of the row.
        /// Returns TRUE if custom rendering occurred (indicating that further rendering should stop) otherwise FALSE.
        /// </summary>
        public Func<GridRowViewData<T>, RenderingContext, bool> StartSectionRenderer
        {
            get { return startSectionRenderer; }
            set { startSectionRenderer = value; }
        }

        /// <summary>
        /// Invokes the custom renderer defined (if any) for the start of the row.
        /// Returns TRUE if custom rendering occurred (indicating that further rendering should stop) otherwise FALSE.
        /// </summary>
        public Func<GridRowViewData<T>, RenderingContext, bool> EndSectionRenderer
        {
            get { return endSectionRenderer; }
            set { endSectionRenderer = value; }
        }

        /// <summary>
        /// Returns custom attributes for the row.
        /// </summary>
        public Func<GridRowViewData<T>, IDictionary<string, object>> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
    }
}
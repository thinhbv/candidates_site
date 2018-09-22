using System;
using CMSSolutions.Configuration;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Indexing.Services;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Indexing.Models
{
    [Feature(Constants.Areas.Indexing)]
    public class SearchSettings : ISettings
    {
        public SearchSettings()
        {
            SearchedFields = new[] { "title", "body" };
        }

        string ISettings.Name { get { return "Search Settings"; } }

        bool ISettings.Hidden { get { return false; } }

        void ISettings.OnEditing(ControlFormResult<ISettings> controlForm, WorkContext workContext)
        {
            var indexingService = workContext.Resolve<IIndexingService>();
            IndexEntry indexEntry;

            try
            {
                indexEntry = indexingService.GetIndexEntry("Search");
            }
            catch (Exception)
            {
                indexEntry = null;
            }

            if (indexEntry == null)
            {
                controlForm.RegisterExternalDataSource("SearchedFields", "title", "body");
            }
            else
            {
                controlForm.RegisterExternalDataSource("SearchedFields", indexEntry.Fields);
            }
        }

        [ControlChoice(ControlChoice.CheckBoxList, Columns = 2, LabelText = "Searched Fields")]
        public string[] SearchedFields { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.ContentManagement.Menus.Domain;
using CMSSolutions.ContentManagement.Menus.Services;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Menus
{
    [Feature(Constants.Areas.Menus)]
    public class MenuWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Menu Widget"; }
        }

        public override bool HasTitle
        {
            get { return false; }
        }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Menu", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public int MenuId { get; set; }

        [ControlNumeric(Required = true, LabelText = "Levels", MinimumValue = "0", MaximumValue = "9999", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public int Levels { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Show From Current", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public bool ShowFromCurrent { get; set; }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (MenuId == 0)
            {
                return;
            }

            var workContext = workContextAccessor.GetContext(viewContext.HttpContext);
            var menuService = workContext.Resolve<IMenuService>();
            var menu = menuService.GetById(MenuId);
            if (menu == null)
            {
                return;
            }

            var menuItemService = workContext.Resolve<IMenuItemService>();
            var menuItems = menuItemService.GetMenuItems(MenuId, true);
            if (menuItems.Count == 0)
            {
                return;
            }

            // ReSharper disable PossibleNullReferenceException
            var slug = viewContext.HttpContext.Request.Url.LocalPath.Trim('/');
            // ReSharper restore PossibleNullReferenceException

            var shellSettings = workContext.Resolve<ShellSettings>();
            if (!string.IsNullOrEmpty(shellSettings.RequestUrlPrefix) && slug.StartsWith(shellSettings.RequestUrlPrefix))
            {
                slug = slug.Substring(shellSettings.RequestUrlPrefix.Length);
            }

            var viewContent = ViewContent(viewContext, "MenuWidget", new { MenuItems = menuItems, Levels, Slug = slug});
            if (!string.IsNullOrEmpty(viewContent))
            {
                writer.Write(viewContent);
                return;
            }

            if (!string.IsNullOrEmpty(CssClass))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "nav navbar-nav");
            }

            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            var urlHelper = new UrlHelper(viewContext.RequestContext);

            if (ShowFromCurrent)
            {
                if (!string.IsNullOrEmpty(slug))
                {
                    var currentMenuItem = menuItems.FirstOrDefault(x => x.Url.Equals(slug, StringComparison.InvariantCultureIgnoreCase));
                    if (currentMenuItem == null)
                    {
                        return;
                    }

                    foreach (var menuItem in menuItems.Where(x => x.ParentId == currentMenuItem.ParentId).OrderBy(x => x.Position).ThenBy(x => x.Text))
                    {
                        BuildMenuItems(writer, menuItem, menuItems, 1, slug, urlHelper);
                    }
                }
            }
            else
            {
                foreach (var menuItem in menuItems.Where(x => x.ParentId == null).OrderBy(x => x.Position).ThenBy(x => x.Text))
                {
                    BuildMenuItems(writer, menuItem, menuItems, 1, slug, urlHelper);
                }
            }

            writer.RenderEndTag(); // ul
        }

        private void BuildMenuItems(HtmlTextWriter writer, MenuItem menuItem, IList<MenuItem> menuItems, int currentLevel, string slug, UrlHelper urlHelper)
        {
            var isCurrent = menuItem.Url.Equals(slug);

            if (Levels == 0 || currentLevel < Levels)
            {
                var childItems = menuItems.Where(x => x.ParentId == menuItem.Id).OrderBy(x => x.Position).ThenBy(x => x.Text).ToList();
                if (childItems.Any())
                {
                    var url = menuItem.IsExternalUrl ? menuItem.Url : urlHelper.Content("~/" + menuItem.Url);
                    if (string.IsNullOrEmpty(menuItem.CssClass))
                    {
                        writer.Write("<li class=\"dropdown\"><a href=\"{1}\" data-toggle=\"dropdown\"><span>{0}</span>&nbsp;<b class=\"caret\"></b></a>", menuItem.Text, url);
                    }
                    else
                    {
                        writer.Write("<li class=\"dropdown {1}\"><a href=\"{2}\" data-toggle=\"dropdown\"><span>{0}</span>&nbsp;<b class=\"caret\"></b></a>", menuItem.Text, menuItem.CssClass, url);
                    }

                    writer.Write("<ul class=\"dropdown-menu\">");
                    foreach (var item in childItems)
                    {
                        BuildMenuItems(writer, item, menuItems, currentLevel + 1, slug, urlHelper);
                    }
                    writer.Write("</ul></li>");
                }
                else
                {
                    var cssClass = (menuItem.CssClass + (isCurrent ? " current" : "")).Trim();
                    var url = menuItem.IsExternalUrl ? menuItem.Url : urlHelper.Content("~/" + menuItem.Url);

                    if (string.IsNullOrEmpty(cssClass))
                    {
                        writer.Write("<li><a href=\"{0}\"><span>{1}</span>{2}</a></li>",
                        url, menuItem.Text,
                        string.IsNullOrEmpty(menuItem.Description) ? null : "<p>" + menuItem.Description + "</p>");
                    }
                    else
                    {
                        writer.Write("<li class=\"{2}\"><a href=\"{0}\"><span>{1}</span>{3}</a></li>",
                            url, menuItem.Text,
                            cssClass,
                            string.IsNullOrEmpty(menuItem.Description) ? null : "<p>" + menuItem.Description + "</p>");
                    }
                }
            }
            else
            {
                var cssClass = (menuItem.CssClass + (isCurrent ? " current" : "")).Trim();
                var url = menuItem.IsExternalUrl ? menuItem.Url : urlHelper.Content("~/" + menuItem.Url);

                if (string.IsNullOrEmpty(cssClass))
                {
                    writer.Write("<li><a href=\"{0}\"><span>{1}</span>{2}</a>",
                        url, menuItem.Text,
                        string.IsNullOrEmpty(menuItem.Description) ? null : "<p>" + menuItem.Description + "</p>");
                }
                else
                {
                    writer.Write("<li class=\"{2}\"><a href=\"{0}\"><span>{1}</span>{3}</a>",
                    url, menuItem.Text,
                    cssClass,
                    string.IsNullOrEmpty(menuItem.Description) ? null : "<p>" + menuItem.Description + "</p>");
                }
            }
        }

        public override ActionResult BuildEditor(Controller controller, WorkContext workContext, ControlFormResult<IWidget> form)
        {
            var menuService = workContext.Resolve<IMenuService>();
            var menus = menuService.GetRecords().OrderBy(x => x.Name).ToDictionary(x => x.Id, x => x.Name);
            form.RegisterExternalDataSource("MenuId", menus);

            return form;
        }
    }
}
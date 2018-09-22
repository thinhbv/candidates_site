using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CMSSolutions.DisplayManagement;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Mvc.Filters;
using IFilterProvider = CMSSolutions.Web.Mvc.Filters.IFilterProvider;

namespace CMSSolutions.Exceptions.Filters
{
    public class UnhandledExceptionFilter : FilterProvider, IActionFilter
    {
        private readonly IExceptionPolicy exceptionPolicy;
        private readonly dynamic shapeFactory;
        private readonly Lazy<IEnumerable<IFilterProvider>> filterProviders;

        public UnhandledExceptionFilter(IExceptionPolicy exceptionPolicy, IShapeFactory shapeFactory, Lazy<IEnumerable<IFilterProvider>> filterProviders)
        {
            this.exceptionPolicy = exceptionPolicy;
            this.shapeFactory = shapeFactory;
            this.filterProviders = filterProviders;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // don't provide custom errors if the action has some custom code to handle exceptions
            if (!filterContext.ActionDescriptor.GetCustomAttributes(typeof(HandleErrorAttribute), false).Any())
            {
                if (!filterContext.ExceptionHandled && filterContext.Exception != null)
                {
                    if (exceptionPolicy.HandleException(this, filterContext.Exception))
                    {
                        filterContext.ExceptionHandled = true;

                        // inform exception filters of the exception that was suppressed
                        var filterInfo = new FilterInfo();
                        foreach (var filterProvider in filterProviders.Value)
                        {
                            filterProvider.AddFilters(filterInfo);
                        }

                        var exceptionContext = new ExceptionContext(filterContext.Controller.ControllerContext, filterContext.Exception);
                        foreach (var exceptionFilter in filterInfo.ExceptionFilters)
                        {
                            exceptionFilter.OnException(exceptionContext);
                        }

                        if (exceptionContext.ExceptionHandled)
                        {
                            filterContext.Result = exceptionContext.Result;
                        }
                        else
                        {
                            if (filterContext.HttpContext.Request.HttpMethod == "POST" && filterContext.HttpContext.Request.IsAjaxRequest())
                            {
                                filterContext.Result = new AjaxResult().Alert(filterContext.Exception.GetBaseException().Message);
                            }
                            else
                            {
                                var shape = shapeFactory.ErrorPage();
                                shape.Message = filterContext.Exception.Message;
                                shape.Exception = filterContext.Exception;
                                filterContext.Result = new ShapeResult(filterContext.Controller, shape);
                                filterContext.RequestContext.HttpContext.Response.StatusCode = 500;

                                // prevent IIS 7.0 classic mode from handling the 404/500 itself
                                filterContext.RequestContext.HttpContext.Response.TrySkipIisCustomErrors = true;    
                            }
                        }
                    }
                }
            }

            if (filterContext.Result is HttpNotFoundResult)
            {
                var model = shapeFactory.NotFound();
                var request = filterContext.RequestContext.HttpContext.Request;
                var url = request.RawUrl;

                // If the url is relative then replace with Requested path
                // ReSharper disable PossibleNullReferenceException
                model.RequestedUrl = request.Url.OriginalString.Contains(url) & request.Url.OriginalString != url ? request.Url.OriginalString : url;
                // ReSharper restore PossibleNullReferenceException

                // Dont get the user stuck in a 'retry loop' by
                // allowing the Referrer to be the same as the Request
                model.ReferrerUrl = request.UrlReferrer != null &&
                    request.UrlReferrer.OriginalString != model.RequestedUrl ?
                    request.UrlReferrer.OriginalString : null;

                filterContext.Result = new ShapeResult(filterContext.Controller, model);
                filterContext.RequestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

                // prevent IIS 7.0 classic mode from handling the 404/500 itself
                filterContext.RequestContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}

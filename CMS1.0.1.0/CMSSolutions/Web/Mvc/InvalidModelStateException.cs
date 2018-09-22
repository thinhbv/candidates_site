using System;
using System.Linq;
using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc
{
    public class InvalidModelStateException : Exception
    {
        private readonly ModelStateDictionary modelState;

        public InvalidModelStateException(ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
            {
                throw new ArgumentException("ModelState is valid.");
            }
            this.modelState = modelState;
        }

        public override string Message
        {
            get { return modelState.Values.SelectMany(x => x.Errors).First().ErrorMessage; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace CMSSolutions.Web.UI.JQueryBuilder
{
    public class JQuery
    {
        private readonly IList<JQuery> childActions;
        private readonly object selector;
        private readonly object parentSelector;
        private readonly bool endStatement;

        protected JQuery()
        {
            childActions = new List<JQuery>();
        }

        public JQuery(object selector, bool endStatement = true)
            : this(selector, null, endStatement)
        {
        }

        public JQuery(object selector, object parentSelector, bool endStatement = true)
        {
            this.selector = selector;
            this.parentSelector = parentSelector;
            childActions = new List<JQuery>();
            this.endStatement = endStatement;
        }

        protected virtual bool ReturnJQuery
        {
            get { return true; }
        }

        private void ValidateBeforeCall()
        {
            if (childActions.Count > 0)
            {
                if (!childActions[childActions.Count - 1].ReturnJQuery)
                {
                    throw new ArgumentException("Cannot call this method on non jQuery object.");
                }
            }
        }

        public virtual string Build()
        {
            if (parentSelector == null)
            {
                if (selector is string)
                {
                    return string.Format("jQuery(\"{0}\")", selector);
                }
                return string.Format("jQuery({0})", selector);
            }

            if (selector is string)
            {
                return string.Format("jQuery(\"{0}\", {1})", selector, parentSelector);
            }

            return string.Format("jQuery({0}, {1})", selector, parentSelector);
        }

        public sealed override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Build());

            foreach (var childAction in childActions)
            {
                sb.Append(childAction.Build());
            }

            if (endStatement)
            {
                sb.Append(";");
            }

            return sb.ToString();
        }

        #region Manipulation

        public JQuery AddClass(string className)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryAddClass(className));
            return this;
        }

        public JQuery Value()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryValue());
            return this;
        }

        public JQuery Data(string name, object value = null)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryData(name, value));
            return this;
        }

        public JQuery Value(object value)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryValue(value));
            return this;
        }

        public JQuery After(object content)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryAfter(content));
            return this;
        }

        public JQuery Append(object content)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryAppend(content));
            return this;
        }

        public JQuery AppendTo(object target)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryAppendTo(target));
            return this;
        }

        public JQuery Attr(string attributeName)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryAttr(attributeName));
            return this;
        }

        public JQuery Attr(string attributeName, object value)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryAttr(attributeName, value));
            return this;
        }

        public JQuery Before()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryBefore());
            return this;
        }

        public JQuery Before(object content)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryBefore(content));
            return this;
        }

        public JQuery Clone()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryClone());
            return this;
        }

        public JQuery Clone(bool withDataAndEvents)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryClone(withDataAndEvents));
            return this;
        }

        public JQuery Css(string propertyName)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryCss(propertyName));
            return this;
        }

        public JQuery Css(string propertyName, string value)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryCss(propertyName, value));
            return this;
        }

        public JQuery Detach()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryDetach());
            return this;
        }

        public JQuery Empty()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryEmpty());
            return this;
        }

        public JQuery HasClass(string className)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryHasClass(className));
            return this;
        }

        public JQuery Height()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryHeight());
            return this;
        }

        public JQuery Height(object value)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryHeight(value));
            return this;
        }

        public JQuery Html()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryHtml());
            return this;
        }

        public JQuery Html(object htmlString)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryHtml(htmlString));
            return this;
        }

        public JQuery InnerHeight()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryInnerHeight());
            return this;
        }

        public JQuery InnerWidth()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryInnerWidth());
            return this;
        }

        public JQuery InsertAfter(object target)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryInsertAfter(target));
            return this;
        }

        public JQuery InsertBefore(object target)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryInsertBefore(target));
            return this;
        }

        public JQuery Offset()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryOffset());
            return this;
        }

        public JQuery Offset(object coordinates)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryOffset(coordinates));
            return this;
        }

        public JQuery OuterHeight()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryOuterHeight());
            return this;
        }

        public JQuery OuterHeight(object includeMargin)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryOuterHeight(includeMargin));
            return this;
        }

        public JQuery OuterWidth()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryOuterWidth());
            return this;
        }

        public JQuery OuterWidth(object includeMargin)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryOuterWidth(includeMargin));
            return this;
        }

        public JQuery Position()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryPosition());
            return this;
        }

        public JQuery Prepend()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryPrepend());
            return this;
        }

        public JQuery Prepend(object content)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryPrepend(content));
            return this;
        }

        public JQuery PrependTo(object target)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryPrependTo(target));
            return this;
        }

        public JQuery Property(string propertyName)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryProperty(propertyName));
            return this;
        }

        public JQuery Property(string propertyName, object value)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryProperty(propertyName, value));
            return this;
        }

        public JQuery Remove()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryRemove());
            return this;
        }

        public JQuery Remove(object selectors)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryRemove(selectors));
            return this;
        }

        public JQuery RemoveAttr(string attributeName)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryRemoveAttr(attributeName));
            return this;
        }

        public JQuery RemoveClass(object className)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryRemoveClass(className));
            return this;
        }

        public JQuery RemoveProp(string propertyName)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryRemoveProperty(propertyName));
            return this;
        }

        public JQuery ReplaceAll(object target)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryReplaceAll(target));
            return this;
        }

        public JQuery ReplaceWith(object newContent)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryReplaceWith(newContent));
            return this;
        }

        public JQuery ScrollLeft()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryScrollLeft());
            return this;
        }

        public JQuery ScrollLeft(object value)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryScrollLeft(value));
            return this;
        }

        public JQuery ScrollTop()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryScrollTop());
            return this;
        }

        public JQuery ScrollTop(object value)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryScrollTop(value));
            return this;
        }

        public JQuery Text()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryText());
            return this;
        }

        public JQuery Text(object textString)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryText(textString));
            return this;
        }

        public JQuery ToggleClass(string className, bool switchs)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryToggleClass(className, switchs));
            return this;
        }

        public JQuery Unwrap()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryUnwrap());
            return this;
        }

        public JQuery Width()
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryWidth());
            return this;
        }

        public JQuery Width(object value)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryWidth(value));
            return this;
        }

        public JQuery Wrap(object wrappingElement)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryWrap(wrappingElement));
            return this;
        }

        public JQuery WrapAll(object wrappingElement)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryWrapAll(wrappingElement));
            return this;
        }

        public JQuery WrapInner(object wrappingElement)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryWrapInner(wrappingElement));
            return this;
        }

        public JQuery Serialize()
        {
            ValidateBeforeCall();
            childActions.Add(new JQuerySerialize());
            return this;
        }

        public JQuery Each(params JQuery[] handlers)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryEach(handlers));
            return this;
        }

        #endregion Manipulation

        #region Validate

        public JQuery Validate(params JQuery[] validHandlers)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryValidate(validHandlers));
            return this;
        }

        #endregion Validate

        #region Events

        public JQuery Click(params JQuery[] handlers)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryClickEvent(handlers));
            return this;
        }

        public JQuery Submit(params JQuery[] handlers)
        {
            ValidateBeforeCall();
            childActions.Add(new JQuerySubmitEvent(handlers));
            return this;
        }

        public JQuery Live(string events, params JQuery[] handlers)
        {
            ValidateBeforeCall();
            childActions.Add(new JQueryLive(events, handlers));
            return this;
        }

        #endregion Events
    }
}
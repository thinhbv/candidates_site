﻿// Copyright 2009-2012 Josh Close
// This file is a part of CsvHelper and is licensed under the MS-PL
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html
// http://csvhelper.com
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CMSSolutions.IO.Csv.TypeConversion;

namespace CMSSolutions.IO.Csv
{
    /// <summary>
    /// Common reflection tasks.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets the first attribute of type T on property.
        /// </summary>
        /// <typeparam name="T">Type of attribute to get.</typeparam>
        /// <param name="property">The <see cref="PropertyInfo" /> to get the attribute from.</param>
        /// <param name="inherit">True to search inheritance tree, otherwise false.</param>
        /// <returns>The first attribute of type T, otherwise null.</returns>
        public static T GetAttribute<T>(PropertyInfo property, bool inherit) where T : Attribute
        {
            T attribute = null;
            var attributes = property.GetCustomAttributes(typeof(T), inherit).ToList();
            if (attributes.Count > 0)
            {
                attribute = attributes[0] as T;
            }
            return attribute;
        }

        /// <summary>
        /// Gets the attributes of type T on property.
        /// </summary>
        /// <typeparam name="T">Type of attribute to get.</typeparam>
        /// <param name="property">The <see cref="PropertyInfo" /> to get the attribute from.</param>
        /// <param name="inherit">True to search inheritance tree, otherwise false.</param>
        /// <returns>The attributes of type T.</returns>
        public static T[] GetAttributes<T>(PropertyInfo property, bool inherit) where T : Attribute
        {
            var attributes = property.GetCustomAttributes(typeof(T), inherit);
            return attributes.Cast<T>().ToArray();
        }

        /// <summary>
        /// Gets the <see cref="ITypeConverter"/> for the <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="property">The property to get the <see cref="ITypeConverter"/> from.</param>
        /// <returns>The <see cref="ITypeConverter"/> </returns>
        public static ITypeConverter GetTypeConverterFromAttribute(PropertyInfo property)
        {
            ITypeConverter typeConverter = null;
            var typeConverterAttribute = GetAttribute<TypeConverterAttribute>(property, false);
            if (typeConverterAttribute != null)
            {
                var typeConverterType = typeConverterAttribute.Type;
                if (typeConverterType != null)
                {
                    typeConverter = Activator.CreateInstance(typeConverterType) as ITypeConverter;
                }
            }
            return typeConverter;
        }

        /// <summary>
        /// Gets the constructor <see cref="NewExpression"/> from the give <see cref="Expression"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the object that will be constructed.</typeparam>
        /// <param name="expression">The constructor <see cref="Expression"/>.</param>
        /// <returns>A constructor <see cref="NewExpression"/>.</returns>
        /// <exception cref="System.ArgumentException">Not a constructor expression.;expression</exception>
        public static NewExpression GetConstructor<T>(Expression<Func<T>> expression)
        {
            var newExpression = expression.Body as NewExpression;
            if (newExpression == null)
            {
                throw new ArgumentException("Not a constructor expression.", "expression");
            }

            return newExpression;
        }

        /// <summary>
        /// Gets the property from the expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>The <see cref="PropertyInfo"/> for the expression.</returns>
        public static PropertyInfo GetProperty<TModel>(Expression<Func<TModel, object>> expression)
        {
            return (PropertyInfo)GetMemberExpression(expression).Member;
        }

        /// <summary>
        /// Gets the member expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private static MemberExpression GetMemberExpression<TModel, T>(Expression<Func<TModel, T>> expression)
        {
            // This method was taken from FluentNHibernate.Utils.ReflectionHelper.cs and modified.
            // http://fluentnhibernate.org/

            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", "expression");
            }

            return memberExpression;
        }
    }
}
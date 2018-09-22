﻿// Copyright 2009-2012 Josh Close
// This file is a part of CsvHelper and is licensed under the MS-PL
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html
// http://csvhelper.com
using System;
using System.Runtime.Serialization;

namespace CMSSolutions.IO.Csv.Configuration
{
    /// <summary>
    /// Represents configuration errors that occur.
    /// </summary>
    public class CsvConfigurationException : CsvHelperException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvConfigurationException"/> class.
        /// </summary>
        public CsvConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvConfigurationException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CsvConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvConfigurationException"/> class
        /// with a specified error message and a reference to the inner exception that
        /// is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public CsvConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !WINRT_4_5

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvConfigurationException"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public CsvConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

#endif
    }
}
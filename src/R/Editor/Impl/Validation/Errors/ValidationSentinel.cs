﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.R.Core.Parser;
using Microsoft.R.Core.Tokens;

namespace Microsoft.R.Editor.Validation.Errors
{
    [ExcludeFromCodeCoverage]
    public class ValidationSentinel : ValidationErrorBase
    {
        /// <summary>
        /// Constructs 'barrier' pseudo error that clears all messages for a given node.
        /// </summary>
        public ValidationSentinel(RToken token) :
            base(null, token, String.Empty, ErrorLocation.Token, ErrorSeverity.Error)
        {
        }
    }
}
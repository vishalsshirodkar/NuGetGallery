﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Web;
using Elmah;

namespace NuGetGallery
{
    internal static class QuietLog
    {
        public static void LogHandledException(Exception e)
        {
            try
            {
                if (HttpContext.Current != null)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                }
                else
                {
                    ErrorLog.GetDefault(null).Log(new Error(e));
                }
            }
            catch
            {
                // logging failed, don't allow exception to escape
            }
        }
    }
}
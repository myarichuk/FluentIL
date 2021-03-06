﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxy
{
    public interface IProxyMonitor
    {
        void BeforeExecute(string methodName, object[] p);
        void AfterExecute(string methodName, object result);
    }
}

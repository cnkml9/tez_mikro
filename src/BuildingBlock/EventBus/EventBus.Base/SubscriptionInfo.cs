﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base
{
    public class SubscriptionInfo
    {
   
        public Type HandlerType { get; set; }

        public SubscriptionInfo(Type handlerType)
        {
            HandlerType = handlerType;
        }

        public static SubscriptionInfo Type(Type handlerType)
        {
            return new SubscriptionInfo(handlerType);
        }

    }
}

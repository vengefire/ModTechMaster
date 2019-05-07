using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace Framework.Windsor.Selectors
{
    public class TypeExclusionSelector : IInterceptorSelector
    {
        private readonly Dictionary<Type, List<Type>> typeListToExcludePerInterceptor;

        public TypeExclusionSelector(Dictionary<Type, List<Type>> typeListToExcludePerInterceptor)
        {
            this.typeListToExcludePerInterceptor = typeListToExcludePerInterceptor;
        }

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            return interceptors.Where(i => CanIntercept(type, method, i)).ToArray();
        }

        public bool CanIntercept(Type type, MethodInfo methodInfo, IInterceptor interceptor)
        {
            List<Type> typesToExclude;

            if (!typeListToExcludePerInterceptor.TryGetValue(interceptor.GetType(), out typesToExclude))
            {
                return true;
            }

            if (typesToExclude.Any(type.IsSubclassOf))
            {
                return false;
            }

            return true;
        }
    }
}
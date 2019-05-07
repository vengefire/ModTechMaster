using System.Data.Entity.Validation;
using System.Text;
using Castle.DynamicProxy;

namespace Framework.Windsor.Interceptors
{
    public class EFDBEntityValidationExceptionAspect : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb, ex);
            }
        }
    }
}
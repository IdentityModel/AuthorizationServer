using System.Linq;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.Admin.Api
{
    public static class ModelStateDictionaryExtensions
    {
        public static object GetErrors(this System.Web.Http.ModelBinding.ModelStateDictionary model)
        {
            if (model != null)
            {
                var query =
                    from item in model.Values
                    where item.Errors.Count > 0
                    from err in item.Errors
                    select err.ErrorMessage;
                return new { errors = query.ToArray() };
            }

            return new { errors = new string[0] };
        }
    }
}
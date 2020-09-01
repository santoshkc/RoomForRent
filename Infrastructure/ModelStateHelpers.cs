using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoomForRent.Infrastructure
{
    public class ModelStateTransferValue
    {
        public string Key { get; set; }
        public string AttemptedValue { get; set; }
        public object RawValue { get; set; }
        public ICollection<string> ErrorMessages { get; set; } = new List<string>();
    }

    public static class ModelStateHelpers
    {
        public static string SerialiseModelState(ModelStateDictionary modelState)
        {
            var errorList = modelState
                .Select(kvp => new ModelStateTransferValue
                {
                    Key = kvp.Key,
                    AttemptedValue = kvp.Value.AttemptedValue,
                    RawValue = kvp.Value.RawValue,
                    ErrorMessages = kvp.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                }).ToList();

            return System.Text.Json.JsonSerializer.Serialize(errorList);
        }

        public static ModelStateDictionary DeserialiseModelState(string serialisedErrorList)
        {
            var errorList = System.Text.Json.JsonSerializer.Deserialize< List<ModelStateTransferValue>>(serialisedErrorList);
            var modelState = new ModelStateDictionary();

            foreach (var item in errorList)
            {
                modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                foreach (var error in item.ErrorMessages)
                {
                    modelState.AddModelError(item.Key, error);
                }
            }
            return modelState;
        }
    }

    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        protected const string Key = nameof(ModelStateTransfer);
    }

    public class ExportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Only export when ModelState is not valid
            if (!filterContext.ModelState.IsValid)
            {
                //Export if we are redirecting
                if (filterContext.Result is RedirectResult
                    || filterContext.Result is RedirectToRouteResult
                    || filterContext.Result is RedirectToActionResult)
                {
                    var controller = filterContext.Controller as Controller;
                    if (controller != null && filterContext.ModelState != null)
                    {
                        var modelState = ModelStateHelpers.SerialiseModelState(filterContext.ModelState);
                        controller.TempData[Key] = modelState;
                    }
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }

    public class ImportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            var serialisedModelState = controller?.TempData[Key] as string;

            if (serialisedModelState != null)
            {
                //Only Import if we are viewing
                if (filterContext.Result is ViewResult)
                {
                    var modelState = ModelStateHelpers.DeserialiseModelState(serialisedModelState);
                    filterContext.ModelState.Merge(modelState);
                }
                else
                {
                    //Otherwise remove it.
                    controller.TempData.Remove(Key);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}

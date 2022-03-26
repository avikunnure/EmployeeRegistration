using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

public class DateTimeModelBinder : IModelBinder
{
    public static readonly Type[] SUPPORTED_TYPES = new Type[] { typeof(DateTime), typeof(DateTime?) };

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        if (!SUPPORTED_TYPES.Contains(bindingContext.ModelType))
        {
            return Task.CompletedTask;
        }

        var modelName = GetModelName(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var dateToParse = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(dateToParse))
        {
            return Task.CompletedTask;
        }

        var dateTime = ParseDate(bindingContext, dateToParse);

        bindingContext.Result = ModelBindingResult.Success(dateTime);

        return Task.CompletedTask;
    }

    private DateTime? ParseDate(ModelBindingContext bindingContext, string dateToParse)
    {
        var attribute = GetDateTimeModelBinderAttribute(bindingContext);
        var dateFormat = attribute?.DateFormat;

        if (string.IsNullOrEmpty(dateFormat))
        {
            return Helper.ParseDateTime(dateToParse);
        }

        return Helper.ParseDateTime(dateToParse, new string[] { dateFormat });
    }

    private DateTimeModelBinderAttribute GetDateTimeModelBinderAttribute(ModelBindingContext bindingContext)
    {
        var modelName = GetModelName(bindingContext);

        var paramDescriptor = bindingContext.ActionContext.ActionDescriptor.Parameters
            .Where(x => x.ParameterType == typeof(DateTime?))
            .Where((x) =>
            {
                // See comment in GetModelName() on why we do this.
                var paramModelName = x.BindingInfo?.BinderModelName ?? x.Name;
                return paramModelName.Equals(modelName);
            })
            .FirstOrDefault();

        var ctrlParamDescriptor = paramDescriptor as ControllerParameterDescriptor;
        if (ctrlParamDescriptor == null)
        {
            return null;
        }

        var attribute = ctrlParamDescriptor.ParameterInfo
            .GetCustomAttributes(typeof(DateTimeModelBinderAttribute), false)
            .FirstOrDefault();

        return (DateTimeModelBinderAttribute)attribute;
    }

    private string GetModelName(ModelBindingContext bindingContext)
    {
        if (!string.IsNullOrEmpty(bindingContext.BinderModelName))
        {
            return bindingContext.BinderModelName;
        }

        return bindingContext.ModelName;
    }
}

public class DateTimeModelBinderAttribute : ModelBinderAttribute
{
    public string DateFormat { get; set; }

    public DateTimeModelBinderAttribute()
        : base(typeof(DateTimeModelBinder))
    {
    }
}


public class Helper
{
    public static DateTime? ParseDateTime(
        string dateToParse,
        string[] formats = null,
        IFormatProvider provider = null,
        DateTimeStyles styles = DateTimeStyles.None)
    {
        var CUSTOM_DATE_FORMATS = new string[]
            {
                //"yyyyMMddTHHmmssZ",
                //"yyyyMMddTHHmmZ",
                //"yyyyMMddTHHmmss",
                //"yyyyMMddTHHmm",
                //"yyyyMMddHHmmss",
                //"yyyyMMddHHmm",
                //"yyyyMMdd",
                //"yyyy-MM-ddTHH-mm-ss",
                //"yyyy-MM-dd-HH-mm-ss",
                //"yyyy-MM-dd-HH-mm",
                //"yyyy-MM-dd",
                //"MMddyyyyTHHmmssZ",
                //"MMddyyyyTHHmmZ",
                //"MMddyyyyTHHmmss",
                //"MMddyyyyTHHmm",
                //"MMddyyyyHHmmss",
                //"MMddyyyyHHmm",
                //"MMddyyyy",
                //"MM-dd-yyyyTHH-mm-ss",
                //"MM-dd-yyyy-HH-mm-ss",
                //"MM-dd-yyyy-HH-mm",
                //"MM-dd-yyyy",
                "ddMMyyyyTHHmmssZ",
                "ddMMyyyyTHHmmZ",
                "ddMMyyyyTHHmmss",
                "ddMMyyyyTHHmm",
                "ddMMyyyyHHmmss",
                "ddMMyyyyHHmm",
                "ddMMyyyy",
                "dd/MM/yyyyTHH-mm-ss",
                "dd/MM/yyyy-HH-mm-ss",
                "dd/MM/yyyy-HH-mm",
                "dd/MM/yyyy",
                "dd/MM/yyyy",
                "dd/MM/yyyyTHH-mm-ss",
                "dd/MM/yyyy-HH-mm-ss",
                "dd/MM/yyyy-HH-mm",
                "dd/MM/yyyy",
                "dd/MM/yyyy"
            };

        if (formats == null || !formats.Any())
        {
            formats = CUSTOM_DATE_FORMATS;
        }

        DateTime validDate;

        foreach (var format in formats)
        {
            if (format.EndsWith("Z"))
            {
                if (DateTime.TryParseExact(dateToParse, format,
                         provider,
                         DateTimeStyles.AssumeUniversal,
                         out validDate))
                {
                    return validDate;
                }
            }

            if (DateTime.TryParseExact(dateToParse, format,
                     provider, styles, out validDate))
            {
                return validDate;
            }
        }

        return null;
    }

    public static bool IsNullableType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
    }
}

public class DateTimeModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (DateTimeModelBinder.SUPPORTED_TYPES.Contains(context.Metadata.ModelType))
        {
            return new BinderTypeModelBinder(typeof(DateTimeModelBinder));
        }

        return null;
    }
}
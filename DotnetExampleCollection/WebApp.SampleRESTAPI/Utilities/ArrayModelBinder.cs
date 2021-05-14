using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApp.SampleRESTAPI.Utilities
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //check the binding Meta Data and the Model Type
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }


            //Get the incomming Value
            var modelValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            // if value is null return null
            if (string.IsNullOrWhiteSpace(modelValue))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            //if value is not null get the model converter

            var converter = TypeDescriptor.GetConverter(bindingContext.ModelType.GetTypeInfo()
                .GenericTypeArguments[0]);

            //convert each elemet to the type  of the IEnumerable 
            var values = modelValue.Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim())).ToArray();

            //Create an empty array of that Type
            var typedValus = Array.CreateInstance(bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0],
                values.Length);
            values.CopyTo(typedValus,0);


            bindingContext.Model = typedValus;
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            
            return Task.CompletedTask;
        }
    }
}

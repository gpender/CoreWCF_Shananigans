using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using dotConnected.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoreWCFServiceNet6.Models
{
	public interface ISetCase
	{
		string ToCase(string tmp);
	}
	public class SetCamelCase : ISetCase
	{
		public string ToCase(string tmp)
		{
			return tmp.ToCamelCase();
		}
	}
	public class SetTitleCase : ISetCase
	{
		public string ToCase(string tmp)
		{
			return tmp.ToTitleCase();
		}
	}
	public class SetGuyCase : ISetCase
	{
		public string ToCase(string tmp)
		{
			return $"{tmp}GUYGUY";
		}
	}
	public class SwaggerDataAttributeFilter : SwaggerDataAttributeFilter<SetGuyCase>
	{
	}
	public class SwaggerDataAttributeFilter<T> : ISchemaFilter where T : ISetCase, new()
	{
		public void Apply(OpenApiSchema schema, SchemaFilterContext schemaFilterContext)
		{
			if (schema.Properties.Count == 0)
				return;

			const BindingFlags bindingFlags = BindingFlags.Public |
			                                  BindingFlags.NonPublic |
			                                  BindingFlags.Instance;
			var memberList = schemaFilterContext.Type // In v5.3.3+ use Type instead
				.GetFields(bindingFlags).Cast<MemberInfo>()
				.Concat(schemaFilterContext.Type // In v5.3.3+ use Type instead
					.GetProperties(bindingFlags));


			var excludedList = memberList.Where(m =>
					((m.GetCustomAttribute<IgnoreDataMemberAttribute>()
					  != null)
					 ||
					 (m.GetCustomAttribute<JsonIgnoreAttribute>()
					  != null)))
				.Select(m =>

					(m.GetCustomAttribute<JsonPropertyAttribute>()
						 ?.PropertyName
					 ?? new T().ToCase(m.Name)));//m.Name.ToCamelCase()));

			foreach (var propType in schemaFilterContext.Type.GetProperties()
				         .Where(x => x.CustomAttributes != null && x.CustomAttributes.Any()))
			{
				var schemaProp = schema.Properties.FirstOrDefault(x => x.Key.Equals(propType.Name, StringComparison.InvariantCultureIgnoreCase));
				string newName = propType.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName;
				if (string.IsNullOrEmpty(newName))
					newName = new T().ToCase(propType.Name);
				//newName = propType.Name.ToCamelCase();

				var newSchemaProp = new KeyValuePair<string, OpenApiSchema>(newName, schemaProp.Value);
				schema.Properties.Remove(schemaProp);
				schema.Properties.Add(newSchemaProp);
			}

			foreach (var excludedName in excludedList)
			{
				if (schema.Properties.ContainsKey(excludedName))
					schema.Properties.Remove(excludedName);
			}
		}
	}
}

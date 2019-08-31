using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhotoFolder.Core.Domain;

namespace PhotoFolder.Infrastructure.Json
{
    public class HashConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			JToken.FromObject(value?.ToString()).WriteTo(writer);
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object existingValue,
			JsonSerializer serializer)
		{
			var valueString = JToken.Load(reader).ToString();
			return string.IsNullOrEmpty(valueString) ? default : Hash.Parse(valueString);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Hash);
		}
	}
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace JsonSolution
{
    // for sa question https://stackoverflow.com/questions/63544325/deserialize-a-json-string-when-there-is-no-field-name-in-c-sharp

    class Program
    {
        static void Main(string[] args)
        {
            // the json containing unknowns fields we want to map to a dictionary
            string json = @"
{
    ""attacks"":{
        ""114862720"":{
            ""code"": ""115dc2b990153c41c33d519b26cc302a""
        },
        ""114862829"": {
            ""code"": ""8bf08c8ceb9b72f05f40235310cd822e""
        }
    }
}
";

            JsonSerializerSettings camelCaseSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

            NewAttackClass obj = JsonConvert.DeserializeObject<NewAttackClass>(json, camelCaseSettings);

            Console.WriteLine(obj);

            Console.Read();
        }
    }

    public class Attacks // existing attack object
    {
        public string Id { get; set; }

        public string Code { get; set; }
    }

    public class NewAttackClass // the new class we want to 
    {
        [JsonConverter(typeof(AttacksConverter))]
        public Dictionary<string, AttackDetails> Attacks { get; set; }
    }

    public class AttackDetails
    {
        public string Code { get; set; }
    }

    class AttacksConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Attacks[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject attacks = serializer.Deserialize<JObject>(reader);
            Dictionary<string, AttackDetails> result = new Dictionary<string, AttackDetails>();

            foreach (JProperty property in attacks.Properties())
            {
                string attackKey = property.Name;
                Attacks attackValue = property.Value.ToObject<Attacks>();
                
                result.Add(attackKey, new AttackDetails()
                {
                    Code = attackValue.Code
                });
            }

            return result;
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
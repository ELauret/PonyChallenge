using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PonyChallengeCore.Infrastructure.MazeGameServices
{
    public class MazeContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            foreach (var property in properties)
            {
                switch (property.PropertyName)
                {
                    case "Width":
                        property.PropertyName = "maze-width";
                        break;
                    case "Height":
                        property.PropertyName = "maze-height";
                        break;
                    case "PlayerName":
                        property.PropertyName = "maze-player-name";
                        break;
                    case "Difficulty":
                        property.PropertyName = "difficulty";
                        break;
                    //case "":
                    //    property.PropertyName = "";
                    //    break;
                    default:
                        break;
                }
            }

            return properties;
        }
    }
}

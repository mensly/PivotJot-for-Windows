﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PivotJot
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Story
    {
        public enum Type { Feature, Bug, Chore, Release }

        [JsonProperty("story_type")]
        private string storyType;

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        public Type StoryType
        {
            get { return (Type)Enum.Parse(typeof(Type), storyType, true); }
            set { storyType = value.ToString().ToLowerInvariant(); }
        }

        public Story(string text, Type storyType = Type.Feature)
        {
            int split = text.IndexOf(':');
            if (0 <= split && split < text.Length - 1)
            {
                Name = text.Substring(0, split).Trim();
                Description = text.Substring(split + 1).Trim();
            }
            else
            {
                Name = text;
                Description = "";
            }
            StoryType = storyType;
        }


    }
}

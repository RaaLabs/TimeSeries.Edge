// Copyright (c) RaaLabs. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RaaLabs.Edge.Connectors.HealthMonitor
{
    public static class PrometheusParser
    {
        private static Regex _lineSplitMatch = new(@"^([\w\d_{}()=\"".,\/:]+)\s*({.*})?\s*(.*)\s*$");
        private static Regex _camelCasePartMatch = new(@"([a-zA-Z=.]+)|(\d+)|(_)");
        public static T Parse<T>(string payload) where T: new()
        {
            var lines = payload.Split('\n')
                .Select(_ => _.Trim())
                .Where(_ => !_.StartsWith("#"));

            var values = lines.ToDictionary(l => GetLabel(l), l => _lineSplitMatch.Match(l).Groups[3].Value);

            T parsed = new();

            foreach (var property in typeof(T).GetProperties())
            {
                if (values.ContainsKey(property.Name))
                {
                    var parsedValue = Convert.ChangeType(values[property.Name], property.PropertyType);
                    property.SetValue(parsed, parsedValue);
                }
            }

            return parsed;
        }

        private static string GetLabel(string line)
        {
            var label = _lineSplitMatch.Match(line).Groups[1].Value;

            var words = _camelCasePartMatch.Matches(label)
                .Select(m => Capitalize(m.Value))
                .Where(w => w != "_")
                .ToList();

            return string.Join("", words);
        }

        private static string Capitalize(string word)
        {
            var capitalized = char.ToUpper(word[0]).ToString();
            if (word.Length > 1)
            {
                capitalized += word.Substring(1);
            }

            return capitalized;
        }
    }
}
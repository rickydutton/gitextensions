using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NBug;

namespace GitExtUtils
{
    public static class JiraUtilities
    {
        private static List<string> GetJiraTags(string value)
        {
            var matchedTags = new Regex("([bB][aA][cC]-[\\d]{1,6})").Matches(value);
            var tags = new List<string>();
            foreach (var tag in matchedTags)
            {
                tags.Add(tag.ToString());
            }
            return tags;
        }

        public static List<string> GetJiraStoriesAsLinks(this string value)
        {
            var jiraUrl = "http://icheque.atlassian.net"; // Todo: fixthis
            jiraUrl += "/browse/";
            var links = GetJiraTags(value).Select(tag => "<a href=\"" + jiraUrl + tag + "\">[" + tag + "]</a>").ToList();
            return links;
        }
    }
}

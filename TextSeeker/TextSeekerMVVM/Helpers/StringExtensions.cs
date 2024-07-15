using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSeeker.TextSeekerMVVM.Helpers
{
    public static class StringExtensions
    {
        public static string RemoveEmptyLines(this string input)
        {
            var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var nonEmptyLines = lines.Where(line => !string.IsNullOrWhiteSpace(line));
            return string.Join("\n", nonEmptyLines);
        }
    }
}

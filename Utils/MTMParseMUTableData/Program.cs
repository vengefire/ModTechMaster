namespace MTMParseMUTableData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var sourceFile = args[0];
            var targetFile = args[1];

            var sourceContent = File.ReadAllText(sourceFile);
            int startIndex = 0;
            int tableStartIndex = 0;
            int tableEndIndex = 0;

            var tablesData = new List<Dictionary<string,List<string>>>();
            var rowData = new List<List<string>>();

            while ((tableStartIndex = FindTag("<table", tableStartIndex, ref sourceContent)) != -1)
            {
                tableEndIndex = FindTag("</table>", tableStartIndex, ref sourceContent);
                startIndex = FindTag("<tr>", tableStartIndex, ref sourceContent);
                var tableHeaderData = ReadRowData(startIndex,  tableEndIndex,"th", ref sourceContent);
                var tableData = new Dictionary<string, List<string>>();
                var dataColumns = tableHeaderData.Item2;

                // Lookup Column Headers by Index...
                var headerDictionary = new Dictionary<int, string>();

                // Skip columns (determined by empty header ID
                var columnIndexesToSkip = new List<int>();

                for (int headerIndex = 0; headerIndex < dataColumns.Count; headerIndex++)
                {
                    if (string.IsNullOrEmpty(dataColumns[headerIndex]))
                    {
                        columnIndexesToSkip.Add(headerIndex);
                        continue;
                    }

                    // Adjust for skipped headers...
                    var adjustedIndex = headerIndex - columnIndexesToSkip.Count;
                    headerDictionary.Add(adjustedIndex, dataColumns[headerIndex]);
                    tableData.Add(headerDictionary[adjustedIndex], new List<string>());
                }

                startIndex = tableHeaderData.Item1;
                startIndex = FindTag("<tbody>", startIndex, ref sourceContent);
                while (true)
                {
                    startIndex = FindTag("<tr", startIndex, ref sourceContent);
                    if (startIndex == -1 || startIndex > tableEndIndex)
                    {
                        break;
                    }

                    var rowEndIndex = FindTag("</tr>", startIndex, ref sourceContent);
                    var itemData = ReadRowData(startIndex, rowEndIndex, "td", ref sourceContent, columnIndexesToSkip);
                    var itemValues = itemData.Item2;
                    for (int itemIndex = 0; itemIndex < itemValues.Count; itemIndex++)
                    {
                        tableData[headerDictionary[itemIndex]].Add(itemValues[itemIndex]);
                    }
                    rowData.Add(itemValues);
                    startIndex = itemData.Item1;
                }
                tablesData.Add(tableData);

                tableStartIndex = tableEndIndex;
            }

            using (var writer = new StreamWriter(File.Create(targetFile)))
            {
                writer.WriteLine();
                foreach (var row in rowData)
                {
                    var lineData = new List<string>();
                    var dbId = -1;
                    var name = row[0];
                    var era = row[6];
                    var year = row[7];
                    var tech = row[4];
                    var chassis = "Vee";
                    var rules = row[4];
                    var tonnage = row[1];
                    var designer = "Catalyst Game Labs";
                    var bv = row[2];
                    var cost = -1;
                    var rating = -1;

                    lineData.Add(dbId.ToString());
                    lineData.Add(name);
                    lineData.Add(era);
                    lineData.Add(year);
                    lineData.Add(tech);
                    lineData.Add(chassis);
                    lineData.Add(rules);
                    lineData.Add(tonnage);
                    lineData.Add(designer);
                    lineData.Add(bv);
                    lineData.Add(cost.ToString());
                    lineData.Add(rating.ToString());

                    var line = string.Join(",", lineData);
                    writer.WriteLine(line);
                }
            }

            Console.WriteLine("Process complete, press any key to continue;");
            Console.ReadKey();
        }

        private static int ReadHead(int startIndex, ref string sourceContent)
        {
            var endIndex = sourceContent.IndexOf("</thead>", startIndex, StringComparison.Ordinal);
            return endIndex;
        }

        private static int FindTag(string tagName, int startIndex, ref string sourceContent)
        {
            return sourceContent.IndexOf(tagName, startIndex, StringComparison.Ordinal);
        }

        private static Tuple<int, List<string>> ReadRowData(int rowindex, int containerEndIndex, string columnTagName, ref string sourceContent, List<int> columnIndexesToSkip = null)
        {
            int startIndex = 0;
            int endIndex = 0;
            var columnData = new List<string>();
            var startColumnTag = $"<{columnTagName}";
            var endColumnTag = $"</{columnTagName}>";
            int columnIndex = 0;
            columnIndexesToSkip = columnIndexesToSkip ?? new List<int>();

            while (true)
            {
                startIndex = FindTag(startColumnTag, rowindex, ref sourceContent);

                // Stop if we don't find any more entries or the next entry is past the bounds of our parent
                if (startIndex == -1 || startIndex > containerEndIndex)
                {
                    break;
                }

                // Find the closing '>' of the opening tag...
                startIndex = sourceContent.IndexOf(">", startIndex, StringComparison.Ordinal) + 1;
                endIndex = FindTag(endColumnTag, startIndex, ref sourceContent);
                var contentLength = endIndex - startIndex;
                var content = sourceContent.Substring(startIndex, contentLength);

                // Find the innermost value
                int innerStartIndex = 0;
                int innerEndIndex = 0;
                if ((innerStartIndex = FindTag("<a", 0, ref content)) != -1)
                {
                    var closeOpenIndex = FindTag(">", innerStartIndex, ref content);
                    innerStartIndex = closeOpenIndex + 1;
                    innerEndIndex = FindTag("</a>", closeOpenIndex, ref content);
                    var innerContentLength = innerEndIndex - innerStartIndex;
                    content = content.Substring(innerStartIndex, innerContentLength);
                }

                if (!columnIndexesToSkip.Contains(columnIndex))
                {
                    columnData.Add(content.Trim());
                }

                rowindex = endIndex;
                columnIndex += 1;
            }

            return new Tuple<int, List<string>>(endIndex + 1, columnData);
        }
    }
}
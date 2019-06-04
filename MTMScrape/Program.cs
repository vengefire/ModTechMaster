namespace MTMScrape
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Framework.Utils.Directory;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class Program
    {
        private static int Main(string[] args)
        {
            var path = args[0];
            if (!DirectoryUtils.Exists(path))
            {
                Console.WriteLine($"Path [{path}] does not exist.");
                return -1;
            }

            // Stuff we're scraping...
            var uniqueTypes = new HashSet<string>();
            var invalidJsonFiles = new List<string>();

            void RecurseDirectories(DirectoryInfo di, int maxDepth, int depth = 0)
            {
                di.GetFiles("*.json").ToList().ForEach(
                                                       info =>
                                                       {
                                                           try
                                                           {
                                                               dynamic modConfig = JsonConvert.DeserializeObject(File.ReadAllText(info.FullName));
                                                               if (info.Name == "mod.json")
                                                               {
                                                                   if (modConfig.Manifest != null)
                                                                   {
                                                                       ((JArray)modConfig.Manifest)
                                                                           .Select(token => (dynamic)token).ToList().ForEach(token => { uniqueTypes.Add(token.Type.ToString()); });
                                                                   }
                                                               }
                                                           }
                                                           catch (Exception ex)
                                                           {
                                                               invalidJsonFiles.Add(info.FullName);
                                                               if (ex.Message.Contains("unexpected character"))
                                                               {
                                                                   var failed = false;
                                                                   while (true)
                                                                   {
                                                                       var sb = TryFixJson(ex, info);
                                                                       File.WriteAllText(info.FullName, sb.ToString());
                                                                       try
                                                                       {
                                                                           dynamic modConfig = JsonConvert.DeserializeObject(File.ReadAllText(info.FullName));
                                                                           break;
                                                                       }
                                                                       catch (Exception inner)
                                                                       {
                                                                           if (!inner.Message.Contains(
                                                                                   "unexpected character"))
                                                                           {
                                                                               Console.WriteLine($"Couldn't fix JSON {info.FullName} - {inner.Message}.");
                                                                               failed = true;
                                                                               break;
                                                                           }
                                                                       }
                                                                   }

                                                                   // Console.Write(sb.ToString());
                                                                   if (!failed) Console.WriteLine($"Fixed {info.FullName}");
                                                               }
                                                           }
                                                       });

                if (maxDepth == -1 ||
                    depth != maxDepth)
                {
                    di.GetDirectories().ToList().ForEach(subDi => RecurseDirectories(subDi, depth++, maxDepth));
                }
            }

            RecurseDirectories(new DirectoryInfo(path), -1);

            Console.WriteLine("Manifest Entry Types:");
            Console.WriteLine(string.Join(",\r\n", uniqueTypes.Select(s => s)));
            Console.WriteLine();
            Console.WriteLine("Invalid .json files:");
            Console.WriteLine(string.Join(",\r\n", invalidJsonFiles.Select(s => s)));
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            return 0;
        }

        private static StringBuilder TryFixJson(Exception ex, FileInfo info)
        {
            var lineStart = ex.Message.IndexOf("line ");
            var lineEnd = ex.Message.IndexOf(",", lineStart);
            var lineString = ex.Message.Substring(lineStart + 5, lineEnd - lineStart - 5);
            var lineValue = int.Parse(lineString);

            var posStart = ex.Message.IndexOf("position ");
            var posEnd = ex.Message.IndexOf(".", posStart);
            var posString = ex.Message.Substring(posStart + 9, posEnd - posStart - 9);
            var posValue = int.Parse(posString);

            int line = 0;
            var update = string.Empty;
            var sb = new StringBuilder();
            using (var reader = new StreamReader(File.Open(info.FullName, FileMode.Open)))
            {
                for (int i = 0; i < lineValue - 1; i++)
                {
                    var lineContent = reader.ReadLine();
                    sb.Append(lineContent);
                }

                for (int i = 0; i < posValue; i++)
                {
                    sb.Append(char.ConvertFromUtf32(reader.Read()));
                }

                sb.Append(',');
                sb.Append(reader.ReadToEnd());
            }

            return sb;
        }
    }
}
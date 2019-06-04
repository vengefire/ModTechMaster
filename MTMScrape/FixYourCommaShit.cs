namespace MTMScrape
{
    using System.IO;

    internal class FixYourCommaShit
    {
        public void FixTheFuckingCommas(string filename)
        {
            // Open the file
            // Find array openings [ (can be nested)
            // Find object closings }
            // If , continue
            // If not and next token is an item opening {
            // Add array item delimiter
            var content = File.ReadAllText(filename);
            var index = -1;
            var arrayDepth = 0;
            while ((index = content.IndexOf('[')) != -1)
            {
                arrayDepth += 1;
            }
        }
    }
}
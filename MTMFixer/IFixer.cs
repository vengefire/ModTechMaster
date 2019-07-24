namespace MTMFixer
{
    using Newtonsoft.Json.Linq;

    internal interface IFixer
    {
        bool Fix(ref JObject mechDefObject);
    }
}
namespace MTMFixer
{
    using System;

    using Newtonsoft.Json.Linq;

    internal class UINameFixer : IFixer
    {
        public bool Fix(ref JObject mechDefObject)
        {
            if (mechDefObject.ContainsKey("Description"))
            {
                var description = (JObject)mechDefObject["Description"];
                if (description.ContainsKey("Id") && !description.ContainsKey("UIName"))
                {
                    var id = description["Id"].ToString();
                    var idString = id;
                    id = id.Replace("mechdef_", string.Empty);
                    var model = id.Substring(0, id.IndexOf("_"));
                    id = id.Replace(model + "_", string.Empty);
                    model = $"{char.ToUpper(model[0])}{model.Substring(1)}";

                    var variantEndIndex = id.IndexOf('_');
                    var hasHeroName = true;
                    if (variantEndIndex == -1)
                    {
                        variantEndIndex = id.Length;
                        hasHeroName = false;
                    }

                    var variant = id.Substring(0, variantEndIndex);
                    var heroName = string.Empty;
                    if (hasHeroName)
                    {
                        id = id.Replace(variant + "_", string.Empty);
                        heroName = id.Replace("_", " ");
                    }

                    var uiName = $"{model} {variant}";
                    if (hasHeroName)
                    {
                        uiName = $"{uiName} {heroName}";
                    }

                    Console.WriteLine($"Inferred UIName [{uiName}] for [{idString}]");
                    description.Add("UIName", uiName);

                    return true;
                }
            }

            return false;
        }
    }
}
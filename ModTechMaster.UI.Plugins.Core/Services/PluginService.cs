namespace ModTechMaster.UI.Plugins.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using ModTechMaster.UI.Plugins.Core.Interfaces;

    public class PluginService
    {
        public List<IPlugin> GetPlugins(string path)
        {
            var pluginList = new List<IPlugin>();
            var pluginType = typeof(IPlugin);
            var di = new DirectoryInfo(path);
            di.GetFiles("*.dll").ToList().ForEach(
                fi =>
                    {
                        var assembly = Assembly.LoadFile(fi.FullName);
                        var pluginTypes = assembly.GetTypes().Where(
                            type => !type.IsInterface && !type.IsAbstract && pluginType.IsAssignableFrom(type));
                        foreach (var type in pluginTypes)
                        {
                            var plugin = (IPlugin)Activator.CreateInstance(type);
                            pluginList.Add(plugin);
                        }
                    });
            return pluginList;
        }
    }
}
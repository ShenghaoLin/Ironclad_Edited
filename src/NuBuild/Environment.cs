﻿
namespace NuBuild
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Reflection;

    using YamlDotNet.Dynamic;

    public static class Environment
    {
        private const string RootPathSentinel = "IRONROOT.sentinel";
        private const string ConfigDotYamlRelativePath = ".nubuild/config.yaml";
        
        public static dynamic ConfigDotYaml { get; private set; }

        static Environment()
        {
            loadConfigDotYaml();            
        }

        public static string getDefaultIronRoot()
        {
            string exepath = Path.GetDirectoryName(getAssemblyPath());
            exepath = Path.GetFullPath(exepath);
            string[] parts = exepath.Split(new char[] { '\\' });
            for (int i = parts.Length; i > 0; i--)
            {
                string proposal = String.Join("\\", parts.Take(i));
                if (File.Exists(Path.Combine(proposal, RootPathSentinel)))
                {
                    return proposal;
                }
            }

            return null;
        }

        public static string getAssemblyPath()
        {
            string assyUri = Assembly.GetExecutingAssembly().CodeBase;
            string assyPath = new Uri(assyUri).LocalPath;
            return assyPath;
        }

        static private void loadConfigDotYaml()
        {
            string p = Path.Combine(getDefaultIronRoot(), ConfigDotYamlRelativePath);
            if (File.Exists(p))
            {
                ConfigDotYaml = new DynamicYaml(YamlDoc.LoadFromFile(p));
            }
            else
            {
                Logger.WriteLine(string.Format("Unable to find {0}; assuming empty document.", ConfigDotYamlRelativePath));
                ConfigDotYaml = new DynamicYaml(YamlDoc.LoadFromString("---\n"));
            }
        }
    }
}
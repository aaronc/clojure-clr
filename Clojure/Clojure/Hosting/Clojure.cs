using System;
using System.Dynamic;

namespace clojure.lang.Hosting
{
    /// <summary>
    /// Convenience class for interfacing with Clojure from other .NET languages.
    /// Provides an extremely simple way to retreive Clojure vars and invoke Clojure
    /// functions.
    /// </summary>
    public class Clojure
    {
        private const string CLOJURE_LOAD_PATH = "CLOJURE_LOAD_PATH";
        private static readonly Var REQUIRE = RT.var("clojure.core", "require");

        /// <summary>
        /// Calls (require nsname) on the namespace name provided by
        /// <see cref="nsname"/>.
        /// </summary>
        /// <param name="nsname">The name of the namespace to require, ex.:
        /// clojure.string</param>
        public static void Require(string nsname)
        {
            if (!nsname.Equals("clojure.core"))
                REQUIRE.invoke(Symbol.intern(nsname));
        }

        /// <summary>
        /// Returns the Var object refered to in the namespace called
        /// <see cref="nsname"/> with the name <see cref="varName"/>.
        /// </summary>
        /// <example>
        /// var myVar = Clojure.GetVar("mynamespace", "my-var");
        /// myVar.invoke("Hello World");
        /// </example>
        /// <param name="nsname">The namespace name</param>
        /// <param name="varName">The name of the Var</param>
        /// <returns>The requested Var</returns>
        public static Var GetVar(string nsname, string varName)
        {
            Require(nsname);
            return RT.var(nsname, varName);
        }

        /// <summary>
        /// Allows a namespace to be loaded from a directory whose path corresponds to
        /// only part of the namespace name.  This can be used to load .clj files properly
        /// from the repl that are being stored as embedded resources in mixed language .NET
        /// DLL's (for example existing C# and Visual Basic projects).
        /// </summary>
        /// <param name="namespaceBase"></param>
        /// <param name="directory"></param>
        /// <remarks>
        /// Adding a load mapping from "MyCompany.MyProject" to "MyProject" allows the namespace "A.B.C" to be
        /// stored in "MyProject/C.clj" on the file system.  Assuming the default namespace
        /// (configurable in a Visual Studio project's properties) is "MyCompany.MyProject",
        /// "C.clj" can be stored as an embedded resource in "MyProject.dll" and still loaded
        /// properly by the Clojure namespace loader.  This is because embedded resources receive a virtual filename
        /// in a DLL based on both the DLL's default namespace and the file's actual path relative to the project.
        /// In order for the namespace to be loaded from the repl, a load mapping must be created.
        /// <example>
        /// AddNamespaceLoadMapping("MyCompany.MyProject", "MySolutionDir/MyProject");
        /// </example>
        /// </remarks>
        public static void AddNamespaceLoadMapping(string namespaceBase, string directory)
        {
            RT.var("clojure.core", "add-ns-load-mapping").invoke(namespaceBase, directory);
        }

        /// <summary>
        /// Adds a path to the CLOJURE_LOAD_PATH environment variable used to load Clojure files from the disk.
        /// </summary>
        /// <param name="path">The path to add to CLOJURE_LOAD_PATH.  Use ; to separate multiple paths.</param>
        public static void AddToLoadPath(string path)
        {
            var cljLoadPath = Environment.GetEnvironmentVariable(CLOJURE_LOAD_PATH);
            cljLoadPath += ";" + path;
            Environment.SetEnvironmentVariable(CLOJURE_LOAD_PATH, cljLoadPath);
        }
    }
}

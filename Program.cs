using System;
using System.IO;
using System.Linq;
using static System.Console;
using static System.AppDomain;
namespace CustomAppDomains
{
    class Program
    {
        static void Main()
        {
            WriteLine("***** Fun with Custom App Domains *****\n");

            // Show all loaded assemblies in default app domain.
            AppDomain defaultAD = CurrentDomain;
            defaultAD.ProcessExit += (o, s) =>
                WriteLine("Default AD unloaded!");

            ListAllAssembliesInAppDomain(defaultAD);

            MakeNewAppDomain();
            ReadLine();
        }

        static void MakeNewAppDomain()
        {
            // Make a new AppDomain in the current process.
            AppDomain newAD = CreateDomain("SecondAppDomain");
            newAD.DomainUnload += (o, s) =>
                WriteLine("The second app domain has been unloaded!");

            try
            {
                // Now load CarLibrary.dll into this new domain.
                newAD.Load("CarLibrary");
            }
            catch (FileNotFoundException ex)
            {
                WriteLine(ex.Message);
            }

            // List all assemblies. 
            ListAllAssembliesInAppDomain(newAD);

            // Now tear down this app domain.
            Unload(newAD);
        }

        static void ListAllAssembliesInAppDomain(AppDomain ad)
        {
            // Now get all loaded assemblies in the default app domain. 
            var loadedAssemblies = from a in ad.GetAssemblies()
                                   orderby a.GetName().Name
                                   select a;

            WriteLine($"***** Here are the assemblies loaded in {ad.FriendlyName} *****\n");
            foreach (var a in loadedAssemblies)
            {
                WriteLine($"-> Name: {a.GetName().Name}");
                WriteLine($"-> Version: {a.GetName().Version}\n");
            }
        }
    }
}
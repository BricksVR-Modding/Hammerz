using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(Ham.BuildInfo.Description)]
[assembly: AssemblyDescription(Ham.BuildInfo.Description)]
[assembly: AssemblyCompany(Ham.BuildInfo.Company)]
[assembly: AssemblyProduct(Ham.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + Ham.BuildInfo.Author)]
[assembly: AssemblyTrademark(Ham.BuildInfo.Company)]
[assembly: AssemblyVersion(Ham.BuildInfo.Version)]
[assembly: AssemblyFileVersion(Ham.BuildInfo.Version)]
[assembly: MelonInfo(typeof(Ham.HamMod), Ham.BuildInfo.Name, Ham.BuildInfo.Version, Ham.BuildInfo.Author, Ham.BuildInfo.DownloadLink)]
[assembly: MelonColor()]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("BricksVR", "BricksVR")]
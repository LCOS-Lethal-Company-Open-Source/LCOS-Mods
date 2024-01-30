using System;
using System.Diagnostics;
using System.Linq;

// CONSTANTS //
const string LETHAL_LOCATION = @"C:\Program Files (x86)\Steam\steamapps\common\Lethal Company";
const string PLUGINS_LOCATION = LETHAL_LOCATION + @"\BepInEx\plugins";

// SANITY CHECKS //
if(!Directory.Exists(LETHAL_LOCATION)) {
    Console.Error.WriteLine(":: ERROR ::");
    Console.Error.WriteLine("Please download Lethal Company before attempting to mod it!");
    Environment.Exit(1);
}

if(!Directory.Exists(PLUGINS_LOCATION)) {
    Console.Error.WriteLine(":: ERROR ::");
    Console.Error.WriteLine("Please install BepInEx before building your mods!");
    Environment.Exit(1);
}

// --DEV //
if(args[0] == "--dev") {
    // Copy compilation output to lethal company //
    Console.WriteLine($":: LOG :: - Running copy from {args[1]}");

    // TODO: better error handling

    var mods = Directory.GetFiles(args[1], "*.dll", SearchOption.AllDirectories)
        .Where(x => File.Exists(Path.ChangeExtension(x, ".pdb")));

    var outputLocs = new[] {
        Directory.CreateDirectory(PLUGINS_LOCATION),
        Directory.CreateDirectory(Path.Combine(args[2], "build"))
    };

    foreach(var mod in mods) {
        foreach(var outputLoc in outputLocs) {
            var newfile = Path.Combine(outputLoc.FullName, Path.GetFileName(mod));
            if(File.Exists(newfile)) {
                Console.WriteLine($":: LOG :: - Removing old version of {newfile}");
                File.Delete(newfile);
            }

            Console.WriteLine($":: LOG :: - Copying to {newfile}");
            File.Copy(mod, newfile);
        }
    }

    Console.WriteLine($":: LOG :: - Completed copying to plugin location");

    // Run lethal company if requested //
    if(args.Length > 3 && args[3] == "--run") {
        new Process {
            StartInfo = new ProcessStartInfo {
                FileName = Path.Combine(LETHAL_LOCATION, "Lethal Company.exe"),
                UseShellExecute = true
            }
        }.Start();
    }
}
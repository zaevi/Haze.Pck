# Haze.Pck

[![Nuget](https://img.shields.io/nuget/v/Haze.Pck)](https://www.nuget.org/packages/Haze.Pck)

Haze.Pck is a .NET class library for packing and unpacking Godot Engine PCK files.

Remark: This library is designed to improve the workflow of your own project (such as merging multiple PCK, adding additional files). You shouldn't unpack and steal other people's game assets.

## Samples

```C#
// open a pck file
var pck = PckFile.Open("packed.pck");
foreach (var entry in pck.Entries)
{
    // print size and md5
    Console.WriteLine($"{entry.Path} size: {entry.Size:#,000} md5: {entry.GetMD5String()}");

    // open as stream
    //var stream = entry.Open();
}
pck.ExtractToDirectory("path/to/extract"); // extract all file

// create pck packer
var packer = PckFile.Create("packed2.pck");
packer.Add(pck.Entries.First()); // add another pck file to packer
packer.Add("res://a.txt", "This is a text", Encoding.UTF8); // add text to packer
packer.Add("res://b.dat", new byte[10] ); // add bytes to packer
packer.Add("res://file.png", "path/to/file.png"); // add file to packer

packer.ComputeMD5 = true; // calculate files' MD5. The default is false.
packer.Pack(); // pack
```

## License

MIT
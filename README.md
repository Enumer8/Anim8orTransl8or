# Anim8or Transl8or
Anim8or Transl8or converts ANIM8OR (\*.an8) files into COLLADA (\*.dae) files.

(For more information on Anim8or, please visit http://www.anim8or.com/)

(For more information on COLLADA, please visit https://www.khronos.org/collada/)

## Prerequisites
Anim8or Transl8or requires .NET Framework 4.0. It is most likely already installed on your system if you are using Windows XP or later. Otherwise, it can be installed from here: https://www.microsoft.com/en-us/download/details.aspx?id=17851.

Anim8or Transl8or may also run on other operating systems using Mono (https://www.mono-project.com/), but this is not tested.

## Graphical User Interface (GUI)
1. Double-click Anim8orTransl8or.Gui.exe
1. Browse for the input ANIM8OR (\*.an8) file
1. Choose the path for the output COLLADA (\*.dae) file
1. Click Convert

## Command Line Interface (CLI)
~~~
Anim8orTransl8or.Cli.exe input.an8 output.dae
~~~

Note: "input.an8" is the name of the input ANIM8OR (\*.an8) file and "output.dae" is the name of the output COLLADA (\*.dae) file.

## Dynamically Linked Library (DLL)
~~~
using Anim8orTransl8or;
using Anim8orTransl8or.An8;
using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Dae.V141;
using System;
using System.IO;
using System.Xml.Serialization;

namespace User
{
   class Program
   {
      static void Main(String[] args)
      {
         String inFile = args[0];
         String outFile = args[1];

         ANIM8OR an8;

         using ( Stream stream = File.Open(inFile, FileMode.Open) )
         {
            An8Serializer deserializer = new An8Serializer(typeof(ANIM8OR));
            an8 = (ANIM8OR)deserializer.Deserialize(stream);
         }

         COLLADA dae = FormatConverter.Convert(an8);

         using ( Stream stream = File.Create(outFile) )
         {
            XmlSerializer serializer = new XmlSerializer(typeof(COLLADA));
            serializer.Serialize(stream, dae);
         }
      }
   }
}
~~~

Note: Just add a reference to Anim8orTransl8or.dll to your .NET project.

## Features
 * Parsing ANIM8OR (\*.an8) files that conform to version 1.00 of the specification
 * Parsing/generating COLLADA (\*.dae) files that conform to version 1.4.1 of the specification
 * ANIM8OR "header" converts to COLLADA "author"
 * ANIM8OR "description" converts to COLLADA "comments"
 * ANIM8OR "mesh", "sphere", "cylinder", and "cube" convert to COLLADA "polylist"
 * ANIM8OR "figure" converts to COLLADA "controller"

## Not Yet Supported
 * ANIM8OR "environment"
 * ANIM8OR "texture"
 * ANIM8OR "material"
 * ANIM8OR "subdivision", "pathcom", "textcom", "modifier", and "image"
 * ANIM8OR "sphere", "cylinder", and "cube" skinning
 * ANIM8OR "sequence"
 * ANIM8OR "scene"
 * Configuration/optimization. Everything will be converted as faithfully as possible.
 * Error handling. The program will just crash if it does not like something.
 * Generating ANIM8OR (\*.an8) files.
 * Converting COLLADA (\*.dae) to ANIM8OR (\*.an8).
 * Converting ANIM8OR (\*.an8) to other formats besides COLLADA (\*.dae).

## Limitations
There are a handful of things that Anim8or calculates internally that are somewhat challenging to reproduce. For instance, Anim8or supports sphere, cylinder, and cube objects defined by a few parameters. We, however, need to know the exact object vertices when creating the COLLADA file. Anim8or Transl8or has preliminary support for generating them, but if there are issues, you can select them in Anim8or and click Build->Convert to Mesh. This forces Anim8or to output the exact vertices.

Also, object normals are usually automatically calculated by Anim8or and not stored in the file. Anim8or Transl8or has preliminary support for generating them, but if there are issues, you can click Options->Debug->Output Normals in Anim8or. This forces Anim8or to output the exact normals.

Finally, Anim8or supports weighting vertices using bone influences. Anim8or Transl8or does not yet support calculating weights from the influence envelopes. To force Anim8or to output the exact weights, you can double-click all figures that are weighted and choose Weights instead of Bone Influences.

## Report Problems
Anim8or Transl8or is very immature, and the ANIM8OR (\*.an8) format is not completely documented, so there will be problems and incompatibilities. Please enter issues on GitHub (https://github.com/Enumer8/Anim8orTransl8or/issues). Please don't enter issues about things that are [not supported yet](#not-supported-yet). For the best chance at fixing the issue, please attach or link to the ANIM8OR (\*.an8) file that causes the problem.

## Contribute
Anim8or Transl8or is open source software. User contributions are appreciated. Please create a pull request on GitHub (https://github.com/Enumer8/Anim8orTransl8or/pulls). Please focus on developing things that are [not supported yet](#not-supported-yet) and be sure to test your changes. Visual Studio 2017 is recommended for development.

## Acknowledgements
 * Thanks, R. Steven Glanville, for making Anim8or!
 * Thanks, texel3d, for a great reference for ANIM8OR files (https://sourceforge.net/projects/liban8/)
 * Thanks, ThinMatrix, for a great reference for COLLADA files (https://www.youtube.com/watch?v=z0jb1OBw45I)

## Change log
 * Anim8orTransl8or v0.3.0
   * Added conversion for ANIM8OR "figure"
   * Better parsing of ANIM8OR "weights"
   * Added automatic normal calculation
 * Anim8orTransl8or v0.2.0
   * Added conversion for ANIM8OR "sphere", "cylinder", "cube", and "group"
   * Better parsing of ANIM8OR "sphere", "pathcom", "modifier", and "image"
 * Anim8orTransl8or v0.1.0
   * Added conversion for ANIM8OR "mesh"

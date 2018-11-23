using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Dae.V141;
using System;
using System.IO;

namespace Anim8orTransl8or.Cli
{
   class Program
   {
      /// <summary>
      /// args[0] - the name of the input .an8 file
      /// args[1] - the name of the output .dae file
      /// </summary>
      /// <param name="args"></param>
      static void Main(String[] args)
      {
         An8Loader an8Loader = new An8Loader();
         ANIM8OR an8 = an8Loader.Load(File.Open(args[0], FileMode.Open));

         COLLADA dae = Converter.Convert(an8);

         DaeLoader daeLoader = new DaeLoader();
         daeLoader.Save(File.Create(args[1]), dae);
      }
   }
}

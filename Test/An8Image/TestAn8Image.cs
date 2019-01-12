using Anim8orTransl8or.An8.V100;
using Anim8orTransl8or.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anim8orTransl8or.Test
{
   [TestClass]
   public class TestAn8Image : TestBase
   {
      [TestMethod]
      public void EdgeCases()
      {
         // Make sure nulls do not cause a crash
         An8Image.Calculate(null);
         An8Image.Calculate(new image(), null);
      }
   }
}

using System.Windows.Forms;

namespace Anim8orTransl8or.Gui
{
   public partial class MainForm : Form
   {
      public MainForm()
      {
         InitializeComponent();

         Text = Text + " v" + GetType().Assembly.GetName().Version.ToString(3);

#if DEBUG
         Text = Text + "-Debug";
#endif
      }
   }
}

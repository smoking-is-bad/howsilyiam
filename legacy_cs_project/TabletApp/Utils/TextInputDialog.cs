using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabletApp.Utils
{
   public partial class TextInputDialog : Form
   {
      public Label Prompt
      {
         get
         {
            return this.promptText;
         }
      }

      public TextBox TextBox
      {
         get
         {
            return this.textBox;
         }
      }

      public TextInputDialog()
      {
         InitializeComponent();
      }
   }
}

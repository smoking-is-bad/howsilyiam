using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
   /// <summary>
   /// Partial class for AProbe to persist scroll/zoom info.
   /// </summary>
   public partial class AProbe
   {
      public int Zoom { get; set; }
      public int Scroll { get; set; }
   }
}

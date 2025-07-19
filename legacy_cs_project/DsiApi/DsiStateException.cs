using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiApi
{
   public class DsiStateException : Exception
   {
      public DsiStateException(string message) : base(message)
      {
      }
   }
}

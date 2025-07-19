// Copyright (c) 2015 Sensor Networks, Inc.
// 
// All rights reserved. No part of this publication may be reproduced,
// distributed, or transmitted in any form or by any means, including
// photocopying, recording, or other electronic or mechanical methods, without
// the prior written permission of Sensor Networks, Inc., except in the case of
// brief quotations embodied in critical reviews and certain other noncommercial
// uses permitted by copyright law.
// 
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiExtensions
{
   public static class AExceptionExtensions
   {
      /// <summary>
      /// Get an aggregated message string for all the inner exceptions in the given Exception
      /// </summary>
      /// <param name="ae">The Exception</param>
      /// <returns>A space-separated string with all of the Messages from all inner exceptions</returns>
      static public string AggregateMessage(this Exception exception)
      {
         Exception ex = exception;
         Exception inner;
         string exceptionString = (null != (inner = ex.InnerException) ? "" : exception.Message);

         while (null != (inner = ex.InnerException))
         {
            exceptionString += " " + inner.Message;
            ex = inner;
         }

         return exceptionString;
      }
   }
}


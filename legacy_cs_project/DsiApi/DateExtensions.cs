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

namespace DsiApi
{
   public static class DateExtensions
   {
      public static string ToIsoTimestamp(this DateTime time)
      {
         return time.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
      }
   }
}

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

namespace ModBus
{
   /// <summary>
   /// Exception thrown if Modbus device command returns an error.
   /// </summary>
   class AModBusException : Exception
   {
      public AModBusException(ExceptionCode exceptionCode, APacket commandPacket)
      : base(String.Format(DsiApi.ApiResources.ErrorModBusException, exceptionCode,
         commandPacket.ToString()))
      {
 
      }
   }
}


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
using Model;

namespace DsiApi
{
   /// <summary>
   /// Specifies which probes will fire on a DSI. Used by ADsiNetwork.PerformMeasurements.
   /// </summary>
   public class AMeasurementParam
   {
      /// <summary>
      /// The address of the DSI on which to fire the setup.
      /// </summary>
      public byte fDsiAddress;

      /// <summary>
      /// The probes to fire. The first setup for each probe is fired.
      /// </summary>
      public AProbe[] fProbes;
   }
}

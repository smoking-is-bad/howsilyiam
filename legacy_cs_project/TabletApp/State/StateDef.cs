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
using System.Xml.Serialization;

namespace TabletApp.State
{
   /// <summary>
   /// Root state definition data element, which contains a series of States
   /// </summary>
   [XmlRoot("statedef")]
   public class AStateDef
   {
      [XmlElement("state")]
      public List<AState> States { get; set; }
   }
}


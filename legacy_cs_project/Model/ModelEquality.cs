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
using System.Linq;

//
// This file conains definitions for equality of the model objects.
//

namespace Model
{
   public partial class AAsset
   {
      public static bool operator ==(AAsset a, AAsset b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(AAsset a, AAsset b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return this.Equals(other as AAsset);
      }

      public bool Equals(AAsset other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         // Exact type? Should AMarshaledNnnn == ANnnn
         if (this.GetType() != other.GetType())
            return false;

         return (this.name == other.name);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }

   public partial class ACollectionPoint
   {
      public static bool operator ==(ACollectionPoint a, ACollectionPoint b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(ACollectionPoint a, ACollectionPoint b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return this.Equals(other as ACollectionPoint);
      }

      public bool Equals(ACollectionPoint other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         // Exact type? Should AMarshaledNnnn == ANnnn
         if (this.GetType() != other.GetType())
            return false;

         return (this.description == other.description) &&
            (this.name == other.name) &&
            (this.location == other.location);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }


   public partial class ACompany
   {
      public static bool operator ==(ACompany a, ACompany b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(ACompany a, ACompany b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return this.Equals(other as ACompany);
      }

      public bool Equals(ACompany other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         // Exact type? Should AMarshaledNnnn == ANnnn
         if (this.GetType() != other.GetType())
            return false;

         return (this.id == other.id) &&
            (this.location == other.location) &&
            (this.name == other.name) &&
            (this.phone == other.phone);
      }

      public override int GetHashCode()
      {
 	       return base.GetHashCode();
      }
   }


   public partial class ADsiInfo
   {
      public static bool operator ==(ADsiInfo a, ADsiInfo b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(ADsiInfo a, ADsiInfo b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return this.Equals(other as ADsiInfo);
      }

      public bool Equals(ADsiInfo other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         // Exact type? Should AMarshaledNnnn == ANnnn
         if (this.GetType() != other.GetType())
            return false;

         return (this.baudRate == other.baudRate) &&
            (this.dsiCount == other.dsiCount) &&
            (this.firmware == other.firmware) &&
            (this.location == other.location) &&
            (this.modbusAddress == other.modbusAddress) &&
            (this.packetVersion == other.packetVersion) &&
            (this.parityAndStopBits == other.parityAndStopBits) &&
            (this.probeCount == other.probeCount) &&
            (this.serialNumber == other.serialNumber) &&
            (this.tag == other.tag) &&
            (this.description == other.description) &&
            (this.shotTimeInterval == other.shotTimeInterval) &&
            (this.transmitTimeInterval == other.transmitTimeInterval) &&
            (this.ascanTxInterval == other.ascanTxInterval) &&
            (this.dsiModel == other.dsiModel)

#if qExpandedDsiInfoFields
            &&
            (this.cellProvider == other.cellProvider)


            &&
            (this.cellNetworkUserName == other.cellNetworkUserName) &&
            (this.cellNetworkPassword == other.cellNetworkPassword) &&
            (this.gsmAccessPoint == other.gsmAccessPoint) &&
            (this.cloudAppUserName == other.cloudAppUserName) &&
            (this.cloudAppPassword == other.cloudAppPassword) &&
            (this.cloudAppUrl == other.cloudAppUrl);

#else
;

#endif
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }


   public partial struct AFirmwareVersion
   {
      public static bool operator ==(AFirmwareVersion a, AFirmwareVersion b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(AFirmwareVersion a, AFirmwareVersion b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return (other is AFirmwareVersion) && this.Equals((AFirmwareVersion)other);
      }

      public bool Equals(AFirmwareVersion other)
      {
         return (this.fpga == other.fpga) && (this.micro == other.micro);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }


   public partial class AMarshaledGate
   {
      public static bool operator ==(AMarshaledGate a, AMarshaledGate b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(AMarshaledGate a, AMarshaledGate b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return this.Equals(other as AMarshaledGate);
      }

      public bool Equals(AMarshaledGate other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         // Exact type? Should AMarshaledNnnn == ANnnn
         if (this.GetType() != other.GetType())
            return false;

         return (this.start == other.start) &&
            (this.width == other.width) &&
            (this.threshold == other.threshold) &&
            (this.mode == other.mode) &&
            (this.tof == other.tof) &&
            (this.amplitude == other.amplitude);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }


   public partial class AGate
   {
      public static bool operator ==(AGate a, AGate b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(AGate a, AGate b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         if (other is AMarshaledGate)
         {
            return this.Equals(other as AMarshaledGate);
         }
         return this.Equals(other as AGate);
      }

      public bool Equals(AGate other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         // Exact type? Should AMarshaledNnnn == ANnnn
         if (this.GetType() != other.GetType())
            return false;

         return (this.num == other.num);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }

   }


   public partial struct AGpsCoordinate
   {
      public static bool operator ==(AGpsCoordinate a, AGpsCoordinate b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(AGpsCoordinate a, AGpsCoordinate b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return (other is AGpsCoordinate) && this.Equals((AGpsCoordinate)other);
      }

      public bool Equals(AGpsCoordinate other)
      {
         return (this.coordinates == other.coordinates);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }


   public partial class APlant
   {
      public static bool operator ==(APlant a, APlant b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(APlant a, APlant b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return this.Equals(other as APlant);
      }

      public bool Equals(APlant other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         // Exact type? Should AMarshaledNnnn == ANnnn
         if (this.GetType() != other.GetType())
            return false;

         return (this.name == other.name);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }


   public partial class APostalAddress
   {
       public static bool operator ==(APostalAddress a, APostalAddress b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(APostalAddress a, APostalAddress b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return this.Equals(other as APostalAddress);
      }

      public bool Equals(APostalAddress other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         if (this.GetType() != other.GetType())
            return false;

         return (this.address1 == other.address1) &&
            (this.address2 == other.address2) &&
            (this.city == other.city) &&
            (this.country == other.country) &&
            (this.postalCode == other.postalCode) &&
            (this.state == other.state);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }

   public partial class AProbe
   {
      public static bool operator ==(AProbe a, AProbe b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(AProbe a, AProbe b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         if (other is AMarshaledProbe)
         {
            return this.Equals(other as AMarshaledProbe);
         }

         return this.Equals(other as AProbe);
      }

      public bool Equals(AProbe other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

          return base.Equals(other) && (this.num == other.num);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }

   public partial class AMarshaledProbe
   {
      public static bool operator ==(AMarshaledProbe a, AMarshaledProbe b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(AMarshaledProbe a, AMarshaledProbe b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return this.Equals(other as AMarshaledProbe);
      }

      public bool Equals(AMarshaledProbe other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         return (this.description == other.description) &&
            (this.location == other.location) &&
            (this.minimumThickness == other.minimumThickness) &&
            (this.model == other.model) &&
            (this.nominalThickness == other.nominalThickness) &&
            (this.numSetups == other.numSetups) &&
            (this.type == other.type) &&
            (this.velocity == other.velocity) &&
            (this.calZeroCount == other.calZeroCount);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }

   public partial class ASetup
   {
      public static bool operator ==(ASetup a, ASetup b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(ASetup a, ASetup b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         if (other is AMarshaledSetup)
         {
            return this.Equals(other as AMarshaledSetup);
         }
         return this.Equals(other as ASetup);
      }

      public bool Equals(ASetup other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         if (this.GetType() != other.GetType())
            return false;     
     
         // Ignore gates and ascan data.
         return (this.num == other.num);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }

   public partial class AMarshaledSetup
   {
      public static bool operator ==(AMarshaledSetup a, AMarshaledSetup b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(AMarshaledSetup a, AMarshaledSetup b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return this.Equals(other as ASetup);
      }

      public bool Equals(AMarshaledSetup other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         // Ignore calculatedThickness and timestamp which will be modified during the course of a shot.
         return (this.ConstFieldsEquals(other) &&
           (this.calculatedThickness == other.calculatedThickness) &&
           (this.lastThickness == other.lastThickness) &&
           (this.lastTimestamp == other.lastTimestamp) &&
           (this.timestamp == other.timestamp));
      }

      /// <summary>
      /// Test equality of fields which won't change during the course of a shot.
      /// </summary>
      public bool ConstFieldsEquals(AMarshaledSetup other)
      {
         return (this.ascanStart == other.ascanStart) &&
           (this.dsiTemp == other.dsiTemp) &&
           (this.gain == other.gain) &&
           (this.materialTemp == other.materialTemp) &&
           (this.muxSwitchSettings == other.muxSwitchSettings) &&
           (this.pulserWidth == other.pulserWidth) &&
           (this.status == other.status) &&
           (this.switchSettings == other.switchSettings);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }

   public partial class ASite
   {
      public static bool operator ==(ASite a, ASite b)
      {
         return Object.Equals(a, b);
      }

      public static bool operator !=(ASite a, ASite b)
      {
         return !(a == b);
      }

      public override bool Equals(object other)
      {
         return this.Equals(other as ASite);
      }

      public bool Equals(ASite other)
      {
         // Check for null, the same object, then finally for equal fields.
         if (Object.ReferenceEquals(other, null))
            return false;

         if (Object.ReferenceEquals(this, other))
            return true;

         // Exact type? Should AMarshaledNnnn == ANnnn
         if (this.GetType() != other.GetType())
            return false;

         return (this.name == other.name);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }
}

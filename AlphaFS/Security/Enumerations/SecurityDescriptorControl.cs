/* Copyright (c) 2008-2012 Peter Palotas, Alexandr Normuradov
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy 
 *  of this software and associated documentation files (the "Software"), to deal 
 *  in the Software without restriction, including without limitation the rights 
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 *  copies of the Software, and to permit persons to whom the Software is 
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 *  THE SOFTWARE. 
 */
using System;

namespace Alphaleonis.Win32.Security
{
   /// <summary>The SECURITY_DESCRIPTOR_CONTROLdata type is a set of bit flags that qualify the meaning of a security descriptor or its components.
   /// Each security descriptor has a Control member that stores the SECURITY_DESCRIPTOR_CONTROL bits.</summary>
   [Flags]
   internal enum SecurityDescriptorControl
   {
      /// <summary></summary>
      None = 0x20,

      /// <summary>SE_OWNER_DEFAULTED (0x0001) - Indicates that the SID of the owner of the security descriptor was provided by a default mechanism.
      /// This flag can be used by a resource manager to identify objects whose owner was set by a default mechanism. To set this flag, use the SetSecurityDescriptorOwner function. 
      /// </summary>
      OwnerDefaulted = 1,

      /// <summary>SE_GROUP_DEFAULTED (0x0002) - Indicates that the security identifier (SID) of the security descriptor group was provided by a default mechanism.
      /// This flag can be used by a resource manager to identify objects whose security descriptor group was set by a default mechanism.
      /// To set this flag, use the SetSecurityDescriptorGroup function.
      /// </summary>
      GroupDefaulted = 2,

      /// <summary>SE_DACL_PRESENT (0x0004) - Indicates a security descriptor that has a DACL. If this flag is not set, or if this flag is set and the DACL is NULL,
      /// the security descriptor allows full access to everyone. This flag is used to hold the security information specified by a caller until the security descriptor
      /// is associated with a securable object. After the security descriptor is associated with a securable object, the SE_DACL_PRESENT flag is always set in the
      /// security descriptor control. To set this flag, use the SetSecurityDescriptorDacl function.
      /// </summary>
      DaclPresent = 4,

      /// <summary>SE_SACL_DEFAULTED (0x0008) - A default mechanism, rather than the original provider of the security descriptor, provided the SACL.
      /// This flag can affect how the system treats the SACL, with respect to ACE inheritance.
      /// The system ignores this flag if the SE_SACL_PRESENT flag is not set. To set this flag, use the SetSecurityDescriptorSacl function.
      /// </summary>
      SaclDefaulted = 8,

      /// <summary>SE_SACL_PRESENT (0x0010) - Indicates a security descriptor that has a SACL. To set this flag, use the SetSecurityDescriptorSacl function.</summary>
      SaclPresent = 16,

      /// <summary>SE_DACL_AUTO_INHERIT_REQ (0x0100) - Indicates a required security descriptor in which the discretionary access control list (DACL) is set up
      /// to support automatic propagation of inheritable access control entries (ACEs) to existing child objects. For access control lists (ACLs) that support auto inheritance,
      /// this bit is always set. Protected servers can call the ConvertToAutoInheritPrivateObjectSecurity function to convert a security descriptor and set this flag. 
      /// </summary>
      DaclAutoInheritReq = 256,

      /// <summary>SE_SACL_AUTO_INHERIT_REQ (0x0200) - Indicates a required security descriptor in which the system access control list (SACL) is set up
      /// to support automatic propagation of inheritable ACEs to existing child objects. The system sets this bit when it performs the automatic inheritance algorithm
      /// for the object and its existing child objects. To convert a security descriptor and set this flag, protected servers can call the
      /// ConvertToAutoInheritPrivateObjectSecurity function.
      /// </summary>
      SaclAutoInheritReq = 512,

      /// <summary>SE_DACL_AUTO_INHERITED (0x0400) - Indicates a security descriptor in which the discretionary access control list (DACL) is set up to support
      /// automatic propagation of inheritable access control entries (ACEs) to existing child objects. For access control lists (ACLs) that support auto inheritance,
      /// this bit is always set. Protected servers can call the ConvertToAutoInheritPrivateObjectSecurity function to convert a security descriptor and set this flag. 
      /// </summary>
      DaclAutoInherited = 1024,

      /// <summary>SE_SACL_AUTO_INHERITED (0x0800) - Indicates a security descriptor in which the system access control list (SACL) is set up
      /// to support automatic propagation of inheritable ACEs to existing child objects. The system sets this bit when it performs the automatic inheritance algorithm
      /// for the object and its existing child objects. To convert a security descriptor and set this flag, protected servers can call the
      /// ConvertToAutoInheritPrivateObjectSecurity function. 
      /// </summary>
      SaclAutoInherited = 2048,

      /// <summary>SE_DACL_PROTECTED (0x1000) - Prevents the DACL of the security descriptor from being modified by inheritable ACEs.
      /// To set this flag, use the SetSecurityDescriptorControl function.
      /// </summary>
      DaclProtected = 4096,

      /// <summary>SE_SACL_PROTECTED (0x2000) - Prevents the SACL of the security descriptor from being modified by inheritable ACEs.
      /// To set this flag, use the SetSecurityDescriptorControl function.
      /// </summary>
      SaclProtected = 8192,

      /// <summary>SE_RM_CONTROL_VALID (0x4000) - Indicates that the resource manager control is valid.</summary>
      RmControlValid = 16384,

      /// <summary>SE_SELF_RELATIVE (0x8000) - Indicates a self-relative security descriptor.
      /// If this flag is not set, the security descriptor is in absolute format. For more information, see Absolute and Self-Relative Security Descriptors.
      /// </summary>
      SelfRelative = 32768
   }
}
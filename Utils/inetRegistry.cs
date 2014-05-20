using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;

namespace kioskplus
{
    public class inetRegistry
    {
        // Standard Definition
        private RegistryKey mRegistryKey = Registry.LocalMachine;
        private string subKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\kioskplus";

        public string SubKey
		{
			get { return subKey; }
			set	{ subKey = value; }
		}

        public RegistryKey BaseRegistryKey
        {
            get { return mRegistryKey; }
            set { mRegistryKey = value; }
        }

      
        public inetRegistry()
        {

        
        }

        private RegistrySecurity getAccessRight1()
        {
             // Create a security object that grants no access.
               RegistrySecurity rs = new RegistrySecurity();

               try
               {
                   WindowsIdentity wid = WindowsIdentity.GetCurrent();
                   IdentityReferenceCollection irc = wid.Groups.Translate(typeof(NTAccount));

                   WindowsPrincipal wp = new WindowsPrincipal(wid);

                   //SecurityIdentifier sd = wid.User.AccountDomainSid


                   SecurityIdentifier domainSid = wid.User.AccountDomainSid;///new SecurityIdentifier(WellKnownSidType.AccountDomainAdminsSid, null);
                   SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.AccountDomainAdminsSid, domainSid);

                   
                  NTAccount account = sid.Translate(typeof(NTAccount)) as NTAccount;

                   // Get ACL from Windows
                   using (RegistryKey rk = mRegistryKey.OpenSubKey(subKey))
                   {

                      // RegistrySecurity rs = new RegistrySecurity();

                       // Creating registry access rule for 'Everyone' NT account
                       RegistryAccessRule rar = new RegistryAccessRule(
                           account.ToString(),
                           RegistryRights.FullControl,
                           InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                           PropagationFlags.None,
                           AccessControlType.Allow);

                       rs.AddAccessRule(rar);
                       rk.SetAccessControl(rs);
                   }

               }
               catch (System.Security.SecurityException ex)
               {
                   Console.WriteLine(ex.Message);
                  /* throw new InstallException(
                       String.Format("An exception in GrantAllAccessPermission, security exception! {0}", key),
                       ex);
                   * */
               }
               catch (UnauthorizedAccessException ex)
               {
                   Console.WriteLine(ex.Message);
                   /***throw new InstallException(
                       String.Format("An exception in GrantAllAccessPermission, access denied! {0}", key),
                       ex);***/
               }
 
                return rs;
        }


        public string ReadAsStringNoEncrypt(string asKeyName)
        {
            RegistryKey rk = mRegistryKey;
            RegistryKey sk1 = rk.OpenSubKey(subKey);

            if (sk1 == null)
            {
                return "";
            }
            else
            {
                try
                {
                    string lsTmp = "";
                    lsTmp = (string)sk1.GetValue(asKeyName,"",RegistryValueOptions.DoNotExpandEnvironmentNames);

                    if (lsTmp == null || lsTmp.Equals("null"))
                        lsTmp = "";
                    

                    return lsTmp;
                }
                catch (Exception e)
                {

                    //ShowErrorMessage(e, "Reading registry " + asKeyName.ToUpper());
                    return "";
                }
            }
        }

        public string ReadAsString(string asKeyName)
		{
			
			RegistryKey rk = mRegistryKey ;
			RegistryKey sk1 = rk.OpenSubKey(subKey,RegistryKeyPermissionCheck.ReadWriteSubTree,RegistryRights.FullControl);

			if ( sk1 == null )
			{
				return "";
			}
			else
			{
				try 
				{
                    string lsTmp = "";
                    lsTmp = (string)sk1.GetValue(asKeyName);

                    if (lsTmp == null || lsTmp.Equals("null"))
                        lsTmp = "";
                    else
                        lsTmp = inetBase64.decodeString(lsTmp);

					return lsTmp;
				}
				catch (Exception e)
				{
					//ShowErrorMessage(e, "Reading registry " + asKeyName.ToUpper());
					return "";
				}
			}
		}


        public String[] ReadValueNames(string asSubKey)
        {
            String[] tmp = null;
            RegistryKey rk = mRegistryKey;
            RegistryKey sk1 = rk.OpenSubKey(asSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            
            if (sk1 == null)
            {
                return tmp;
            }
            else
            {
                return sk1.GetValueNames();
            }
        }

        public uint ReadAsDWORD(string asKeyName)
        {
            String lsValue = ReadAsString(asKeyName);

            try
            {
                if (lsValue.Equals(""))
                    lsValue = "0";

                return UInt32.Parse(lsValue);
            }
            catch (Exception e)
            {

                //ShowErrorMessage(e, "Reading registry " + asKeyName.ToUpper());
                return 0;
            }
        }

        public uint ReadAsDWORDNoEncrypt(string asKeyName)
        {
            String lsValue = ReadAsStringNoEncrypt(asKeyName);

            try
            {

                if (lsValue.Equals(""))
                    lsValue = "0";

                return UInt32.Parse(lsValue);
            }
            catch (Exception e)
            {

                //ShowErrorMessage(e, "Reading registry " + asKeyName.ToUpper());
                return 0;
            }
        }
            //RegistryKey rk = mRegistryKey;

            //RegistryKey sk1 = rk.OpenSubKey(subKey);

            //if (sk1 == null)
            //{
            //    return 0;
            //}
            //else
            //{
            //    try
            //    {
            //        loReturn = sk1.GetValue(asKeyName);
            //        if (loReturn.ToString() == null || loReturn.ToString().Equals("null"))
            //            loReturn = "0";

            //        return UInt32.Parse(loReturn.ToString());
            //    }
            //    catch (Exception e)
            //    {

            //        //ShowErrorMessage(e, "Reading registry " + asKeyName.ToUpper());
            //        return 0;
            //    }
            //}
        //}

        public byte[] ReadAsByte(string asKeyName)
        {
            object loReturn;

            RegistryKey rk = mRegistryKey;

            RegistryKey sk1 = rk.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    loReturn = sk1.GetValue(asKeyName);
                    return (byte[])loReturn;
                }
                catch 
                {

                    //ShowErrorMessage(e, "Reading registry " + asKeyName.ToUpper());
                    return null;
                }
            }
        }


        /// <summary>
        /// Renames a subkey of the passed in registry key since 
        /// the frame work totally forgot to include such a handy feature.
        /// </summary>
        /// <param name="regKey">The RegistryKey that contains the subkey 
        /// you want to rename (must be writeable)</param>
        /// <param name="subKeyName">The name of the subkey that you want to rename</param>
        /// <param name="newSubKeyName">The new name of the RegistryKey</param>
        /// <returns>True if succeeds</returns>
        public bool RenameSubKey(string subKeyName, string newSubKeyName)
        {
            //20100130
            try
            {
                RegistryKey rk = mRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
                RegistryKey tmp = sk1.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

                if (tmp == null)
                {
                    return false;
                }
                tmp.Close();
                RegistryKey parentKey = sk1;

                CopyKey(parentKey, subKeyName, newSubKeyName);
                parentKey.DeleteSubKeyTree(subKeyName);
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Copy a registry key.  The parentKey must be writeable.
        /// </summary>
        /// <param name="parentKey"></param>
        /// <param name="keyNameToCopy"></param>
        /// <param name="newKeyName"></param>
        /// <returns></returns>
        public bool CopyKey(RegistryKey parentKey, string keyNameToCopy, string newKeyName)
        {
            try
            {
                //Create new key
                RegistryKey destinationKey = parentKey.CreateSubKey(newKeyName,RegistryKeyPermissionCheck.ReadWriteSubTree);

                //Open the sourceKey we are copying from
                RegistryKey sourceKey = parentKey.OpenSubKey(keyNameToCopy, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

                RecurseCopyKey(sourceKey, destinationKey);
            }
            catch (Exception ee)
            { Console.WriteLine(ee.Message); }
            return true;
        }

        private void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
        {
            //copy all the values
            foreach (string valueName in sourceKey.GetValueNames())
            {
                object objValue = sourceKey.GetValue(valueName);
                RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
                destinationKey.SetValue(valueName, objValue, valKind);
            }

            //For Each subKey 
            //Create a new subKey in destinationKey 
            //Call myself 
            foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
            {
                RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
                RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName,RegistryKeyPermissionCheck.ReadWriteSubTree);
                RecurseCopyKey(sourceSubKey, destSubKey);
            }
        }


        public bool Write(string asKeyName, object Value)
		{
			try
			{
				RegistryKey rk = mRegistryKey ;
				
				RegistryKey sk1 = rk.CreateSubKey(subKey,RegistryKeyPermissionCheck.ReadWriteSubTree);

				// Save the value
                String lsTmp;
                lsTmp = Value.ToString();
                lsTmp = inetBase64.encodeAsString(lsTmp);

				sk1.SetValue(asKeyName, lsTmp);

				return true;
			}
			catch (Exception e)
			{
				
				return false;
			}
		}

        public bool WriteNoCrypt(string asKeyName, object Value)
        {
            try
            {

                RegistryKey rk = mRegistryKey;
                RegistryKey sk1 = rk.CreateSubKey(subKey,RegistryKeyPermissionCheck.ReadWriteSubTree);
           
                // Save the value
                sk1.SetValue(asKeyName, Value);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }



        public bool DeleteKey(string asKeyName)
		{
			try
			{
				RegistryKey rk = mRegistryKey ;
				RegistryKey sk1 = rk.CreateSubKey(subKey,RegistryKeyPermissionCheck.ReadWriteSubTree);
                
                if (sk1 == null || sk1.GetValue(asKeyName) == null)
					return true;
				else
					sk1.DeleteValue(asKeyName);

				return true;
			}
			catch (Exception e)
			{
				
				return false;
			}
		}


        public bool DeleteSubKeyTree()
		{
			try
			{
				// Setting
				RegistryKey rk = mRegistryKey ;
                RegistryKey sk1 = rk.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
                
                // If the RegistryKey exists, I delete it
				if ( sk1 != null )
					rk.DeleteSubKeyTree(subKey);

				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}


        public int GetSubKeyCount()
        {
            try
            {
                RegistryKey rk = mRegistryKey;
                RegistryKey sk1 = rk.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
                if (sk1 != null)
                    return sk1.SubKeyCount;
                else
                    return 0;
            }
            catch (Exception e)
            {
                return 0;
            }
        }


    }
}

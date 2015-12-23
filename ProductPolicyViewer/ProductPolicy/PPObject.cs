using Microsoft.Win32;
using System.Collections.Generic;

namespace ProductPolicyViewer.ProductPolicy
{
   public class PPObject
   {
      private const string REG_KEY = @"SYSTEM\CurrentControlSet\Control\ProductOptions";
      private const string REG_VALUE = "ProductPolicy";

      public PPHeader Header { get; private set; }
      public PPRecord[] Policies { get; private set; }

      public PPObject()
      {
         initializeData();
      }

      private unsafe void initializeData()
      {
         List<PPRecord> policies = new List<PPRecord>();

         byte[] data;
         using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(REG_KEY))
         {
            data = rk.GetValue(REG_VALUE) as byte[];
         }

         fixed (byte* dataPtr = data)
         {
            Header = *(PPHeader*)dataPtr;

            uint position = (uint)sizeof(PPHeader);
            while ((data.Length - Header.EndMarker) > position)
            {
               uint startPosition = position;
               PPRecord policy = new PPRecord();

               PPValue* policyVal = (PPValue*)(dataPtr + position);
               policy.Record = *policyVal;
               position += (uint)sizeof(PPValue);

               char* policyName = (char*)(dataPtr + position);
               policy.Name = new string(policyName, 0, (policyVal->NameSize / sizeof(char)));
               position += policyVal->NameSize;

               switch (policyVal->DataType)
               {
                  case PPDataType.Binary:
                     {
                        byte[] valueData = new byte[policyVal->DataSize];
                        for (int i = 0; i < policyVal->DataSize; i++)
                        {
                           valueData[i] = *(dataPtr + position + i);
                        }
                        policy.Data = valueData;
                        break;
                     }
                  case PPDataType.Integer:
                     {
                        policy.Data = *((int*)(dataPtr + position));
                        break;
                     }
                  case PPDataType.String:
                  case PPDataType.MultiString: // just guessing here
                     {
                        char* recordDataPtr = (char*)(dataPtr + position);
                        policy.Data = new string(recordDataPtr, 0, (policyVal->DataSize / sizeof(char))).TrimEnd('\0');
                        break;
                     }
               }

               position = startPosition += policyVal->Size;
               policies.Add(policy);
            }
         }

         Policies = policies.ToArray();
      }
   }
}

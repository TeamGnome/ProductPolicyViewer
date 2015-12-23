using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductPolicyViewer.ProductPolicy
{
   public class PPRecord
   {
      public PPValue Record { get; set; }
      public string Name { get; set; }
      public object Data { get; set; }

      public string DataString
      {
         get
         {
            byte[] dataBytes = Data as byte[];
            if (dataBytes != null)
            {
               StringBuilder sb = new StringBuilder();
               for (int i = 0; i < dataBytes.Length; i++)
               {
                  sb.Append(dataBytes[i].ToString("X2"));
               }
               return sb.ToString();
            }
            else
            {
               return Data?.ToString();
            }
         }
      }
   }
}

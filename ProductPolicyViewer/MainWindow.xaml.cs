using Microsoft.Win32;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ProductPolicyViewer
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      private unsafe void Window_Loaded(object sender, RoutedEventArgs e)
      {
         List<ProductPolicyObject> PPOs = new List<ProductPolicyObject>();
         using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\ProductOptions"))
         {
            byte[] data = rk.GetValue("ProductPolicy") as byte[];

            fixed (byte* dataPtr = data)
            {
               TProductPolicyHeader* tpph = (TProductPolicyHeader*)dataPtr;

               uint pos = (uint)sizeof(TProductPolicyHeader);
               while ((data.Length - tpph->cbEndMarker) > pos)
               {
                  uint startPos = pos;
                  ProductPolicyObject ppo = new ProductPolicyObject();

                  byte* recordPtr = dataPtr + pos;
                  TProductPolicyValue* tppv = (TProductPolicyValue*)recordPtr;
                  ppo.Record = *tppv;

                  pos += (uint)sizeof(TProductPolicyValue);
                  char* recordNamePtr = (char*)(dataPtr + pos);
                  ppo.Name = new string(recordNamePtr, 0, (tppv->cbName / sizeof(char)));

                  pos += tppv->cbName;
                  switch (tppv->SlDatatype)
                  {
                     case LicensingDataType.Binary:
                        {
                           byte[] byteData = new byte[tppv->cbData];

                           for (int i = 0; i < tppv->cbData; i++)
                           {
                              byteData[i] = *(dataPtr + pos + i);
                           }
                           ppo.Data = byteData;
                           break;
                        }
                     case LicensingDataType.Integer:
                        {
                           ppo.Data = *((int *)(dataPtr + pos));
                           break;
                        }
                     case LicensingDataType.String:
                        {
                           char* recordDataPtr = (char*)(dataPtr + pos);
                           ppo.Data = new string(recordDataPtr, 0, (tppv->cbData / sizeof(char))).TrimEnd('\0');
                           break;
                        }
                  }

                  pos = startPos += tppv->cbSize;
                  PPOs.Add(ppo);
               }
            }
         }

         dgOutput.ItemsSource = PPOs;
      }
   }

   public struct TProductPolicyHeader
   {
      public uint cbSize { get; set; }
      public uint cbDataSize { get; set; }
      public uint cbEndMarker { get; set; }
      public uint Unknown1 { get; set; }
      public uint Unknown2 { get; set; }
   }

   public struct TProductPolicyValue
   {
      public ushort cbSize { get; set; }
      public ushort cbName { get; set; }
      public LicensingDataType SlDatatype { get; set; }
      public ushort cbData { get; set; }
      public uint Unknown1 { get; set; }
      public uint Unknown2 { get; set; }
   }

   public enum LicensingDataType : short
   {
      None = -1,
      String = 1,
      Binary = 3,
      Integer = 4,
      MultiString = 7,
      Sum = 100
   }

   public class ProductPolicyObject
   {
      public TProductPolicyValue Record { get; set; }
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
               for(int i = 0; i < dataBytes.Length; i++)
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

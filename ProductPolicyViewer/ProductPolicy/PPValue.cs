namespace ProductPolicyViewer.ProductPolicy
{
   public struct PPValue
   {
      public ushort Size { get; set; }
      public ushort NameSize { get; set; }
      public PPDataType DataType { get; set; }
      public ushort DataSize { get; set; }
      public uint Unknown1 { get; set; }
      public uint Unknown2 { get; set; }
   }
}

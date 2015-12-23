namespace ProductPolicyViewer.ProductPolicy
{
   public struct PPHeader
   {
      public uint Size { get; set; }
      public uint DataSize { get; set; }
      public uint EndMarker { get; set; }
      public uint Unknown1 { get; set; }
      public uint Unknown2 { get; set; }
   }
}
